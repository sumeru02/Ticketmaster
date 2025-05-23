using Microsoft.AspNetCore.Mvc;
using Ticketmaster.Helper;
using Ticketmaster.Interfaces;
using Ticketmaster.Models;

namespace Ticketmaster.Controllers
{
    /// <summary>
    /// Controller for managing ticket reservations, purchases, and cancellations
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("tickets")]
    [Produces("application/json")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        /// <summary>
        /// Constructor that injects the ticket service dependency
        /// </summary>
        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        /// <summary>
        /// Reserves a ticket for the specified event
        /// </summary>
        /// <param name="eventId">ID of the event</param>
        /// <param name="idempotencyKey">Idempotency key to prevent duplicate requests</param>
        /// <returns>Reservation result or error response</returns>
        [HttpPost("{eventId}/reserve")]
        public IActionResult ReserveTicket(
            Guid eventId,
            [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
        {
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                return BadRequest(new { Message = "Idempotency key is required" });

            Guid userId = TokenHelper.GetUserFromToken("JWT");
            TicketReservationResult result = _ticketService.ReserveTicket(eventId, userId, idempotencyKey);

            return result.Status switch
            {
                ReservationStatus.Success => Ok(result.Ticket),
                ReservationStatus.EventNotFound => NotFound(new { Message = "Event does not exist." }),
                ReservationStatus.CapacityFull => Conflict(new { Message = "Event capacity has been reached." }),
                ReservationStatus.DuplicateRequest => Conflict(new { Message = "Duplicate reservation request." }),
                ReservationStatus.NotAuthorized => StatusCode(403, new { Message = "User is not authorized to reserve this ticket." }),
                _ => StatusCode(500, new { Message = "Unexpected error occurred." })
            };
        }

        /// <summary>
        /// Purchases a previously reserved ticket
        /// </summary>
        /// <param name="ticketId">ID of the reserved ticket</param>
        /// <returns>Purchase result or error response</returns>
        [HttpPost("{ticketId}/purchase")]
        public IActionResult PurchaseTicket(Guid ticketId)
        {
            Guid userId = TokenHelper.GetUserFromToken("JWT");
            TicketPurchaseResult result = _ticketService.PurchaseTicket(ticketId, userId);

            return result.Status switch
            {
                PurchaseStatus.Success => Ok(result.Ticket),
                PurchaseStatus.TicketNotFound => NotFound(new { Message = "Ticket not found." }),
                PurchaseStatus.AlreadyPurchased => Conflict(new { Message = "Ticket has already been purchased." }),
                PurchaseStatus.NotReserved => Conflict(new { Message = "Ticket is not in reserved state." }),
                PurchaseStatus.NotAuthorized => StatusCode(403, new { Message = "User is not authorized to purchase this ticket." }),
                _ => StatusCode(500, new { Message = "Unexpected error occurred." })
            };
        }

        /// <summary>
        /// Cancels a reserved ticket
        /// </summary>
        /// <param name="ticketId">ID of the ticket to cancel</param>
        /// <returns>Canceled ticket or error response</returns>
        [HttpPost("{ticketId}/cancel")]
        public IActionResult CancelTicket(Guid ticketId)
        {
            Guid userId = TokenHelper.GetUserFromToken("JWT");
            Ticket ticket = _ticketService.CancelTicket(ticketId, userId);

            if (ticket == null)
                return BadRequest(new { Message = "Cancellation failed. Ticket may not be in reserved state, not belong to the user, or may not exist." });

            return Ok(ticket);
        }

        /// <summary>
        /// Retrieves the number of available tickets for a given event
        /// </summary>
        /// <param name="eventId">ID of the event</param>
        /// <returns>Count of available tickets</returns>
        [HttpGet("{eventId}/availability")]
        public IActionResult GetAvailability(Guid eventId)
        {
            int available = _ticketService.GetAvailableTickets(eventId);
            return Ok(new { Available = available });
        }
    }
}