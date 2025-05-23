using System.Collections.Concurrent;
using Ticketmaster.Interfaces;
using Ticketmaster.Models;

namespace Ticketmaster.Services
{
    /// <summary>
    /// Service for managing ticket reservations, purchases, and cancellations
    /// </summary>
    public class TicketService : ITicketService
    {
        private readonly IEventService _eventService;

        // Stores all tickets by their unique ID
        private readonly ConcurrentDictionary<Guid, Ticket> _tickets = new();

        // Stores idempotency keys to prevent duplicate reservations
        private readonly ConcurrentDictionary<string, Ticket> _idempotencyKeys = new();

        // Time window within which a reserved ticket must be purchased
        private static readonly TimeSpan TicketReservationTTL = TimeSpan.FromMinutes(10);

        public TicketService(IEventService eventService)
        {
            _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        }

        /// <summary>
        /// Reserves a ticket for the given event and user with idempotency support
        /// </summary>
        public TicketReservationResult ReserveTicket(Guid eventId, Guid userId, string idempotencyKey)
        {
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                throw new ArgumentException("Idempotency key is required", nameof(idempotencyKey));

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required", nameof(userId));

            var cacheKey = $"reserve:{idempotencyKey}";

            // Check for duplicate reservation by idempotency key
            if (_idempotencyKeys.TryGetValue(cacheKey, out Ticket? existingTicket))
            {
                if (!IsExpired(existingTicket))
                {
                    return new TicketReservationResult
                    {
                        Status = ReservationStatus.DuplicateRequest,
                        Ticket = existingTicket
                    };
                }
            }

            // Check if event exists
            var ev = _eventService.GetEvent(eventId);
            if (ev == null)
            {
                return new TicketReservationResult
                {
                    Status = ReservationStatus.EventNotFound
                };
            }

            // Check if event has reached capacity
            var reservedCount = _tickets.Values.Count(t =>
                t.EventId == eventId &&
                t.Status != TicketStatus.Cancelled &&
                !IsExpired(t));

            if (reservedCount >= ev.Capacity)
            {
                return new TicketReservationResult
                {
                    Status = ReservationStatus.CapacityFull
                };
            }

            // Check if the user already reserved a ticket for this event
            var alreadyReserved = _tickets.Values.Any(t =>
                t.EventId == eventId &&
                t.UserId == userId &&
                t.Status == TicketStatus.Reserved &&
                !IsExpired(t));

            if (alreadyReserved)
            {
                return new TicketReservationResult
                {
                    Status = ReservationStatus.DuplicateRequest
                };
            }

            // Create and store new reserved ticket
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                UserId = userId,
                Status = TicketStatus.Reserved,
                ReservedAt = DateTime.UtcNow
            };

            _tickets[ticket.Id] = ticket;
            _idempotencyKeys[cacheKey] = ticket;

            return new TicketReservationResult
            {
                Status = ReservationStatus.Success,
                Ticket = ticket
            };
        }

        /// <summary>
        /// Purchases a reserved ticket after verifying user and ticket validity
        /// </summary>
        public TicketPurchaseResult PurchaseTicket(Guid ticketId, Guid userId)
        {
            if (!_tickets.TryGetValue(ticketId, out Ticket? ticket) || ticket == null || IsExpired(ticket))
            {
                return new TicketPurchaseResult
                {
                    Status = PurchaseStatus.TicketNotFound
                };
            }

            if (ticket.UserId != userId)
            {
                return new TicketPurchaseResult
                {
                    Status = PurchaseStatus.NotAuthorized
                };
            }

            if (ticket.Status == TicketStatus.Purchased)
            {
                return new TicketPurchaseResult
                {
                    Status = PurchaseStatus.AlreadyPurchased,
                    Ticket = ticket
                };
            }

            if (ticket.Status != TicketStatus.Reserved)
            {
                return new TicketPurchaseResult
                {
                    Status = PurchaseStatus.NotReserved
                };
            }

            // Stubbed payment check
            bool paymentSuccess = true;

            if (!paymentSuccess)
            {
                return new TicketPurchaseResult
                {
                    Status = PurchaseStatus.PaymentFailed
                };
            }

            ticket.Status = TicketStatus.Purchased;
            _tickets[ticket.Id] = ticket;

            return new TicketPurchaseResult
            {
                Status = PurchaseStatus.Success,
                Ticket = ticket
            };
        }

        /// <summary>
        /// Cancels a reserved ticket for the given user
        /// </summary>
        public Ticket? CancelTicket(Guid ticketId, Guid userId)
        {
            if (!_tickets.TryGetValue(ticketId, out Ticket? ticket) || ticket == null || ticket.Status != TicketStatus.Reserved || IsExpired(ticket))
                return null;

            if (ticket.UserId != userId)
                return null;

            ticket.Status = TicketStatus.Cancelled;
            _tickets[ticket.Id] = ticket;

            return ticket;
        }

        /// <summary>
        /// Returns the number of available tickets for the given event
        /// </summary>
        public int GetAvailableTickets(Guid eventId)
        {
            var ev = _eventService.GetEvent(eventId);
            if (ev == null) return 0;

            var activeTickets = _tickets.Values
                .Where(t => t.EventId == eventId && t.Status != TicketStatus.Cancelled && !IsExpired(t))
                .Count();

            return ev.Capacity - activeTickets;
        }

        /// <summary>
        /// Determines whether a ticket reservation has expired
        /// </summary>
        private bool IsExpired(Ticket ticket)
        {
            return ticket.Status == TicketStatus.Reserved &&
                   DateTime.UtcNow - ticket.ReservedAt > TicketReservationTTL;
        }
    }
}