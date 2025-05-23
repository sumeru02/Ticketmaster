using Ticketmaster.Models;

namespace Ticketmaster.Interfaces
{
    /// <summary>
    /// Interface for managing ticket reservations and sales
    /// </summary>
    public interface ITicketService
    {
        /// <summary>
        /// Reserves a ticket for the specified event and user
        /// </summary>
        /// <param name="eventId">The unique identifier of the concert event</param>
        /// <param name="userId">The unique identifier of the user making the reservation</param>
        /// <param name="idempotencyKey">A unique key to ensure the reservation is processed only once</param>
        /// <returns>The result of the reservation attempt</returns>
        TicketReservationResult ReserveTicket(Guid eventId, Guid userId, string idempotencyKey);

        /// <summary>
        /// Purchases a previously reserved ticket for the specified user
        /// </summary>
        /// <param name="ticketId">The unique identifier of the reserved ticket</param>
        /// <param name="userId">The unique identifier of the user completing the purchase</param>
        /// <returns>The result of the ticket purchase attempt</returns>
        TicketPurchaseResult PurchaseTicket(Guid ticketId, Guid userId);

        /// <summary>
        /// Cancels a reserved or purchased ticket for the specified user
        /// </summary>
        /// <param name="ticketId">The unique identifier of the ticket to cancel</param>
        /// <param name="userId">The unique identifier of the user requesting the cancellation</param>
        /// <returns>The canceled ticket if successful; otherwise, null</returns>
        Ticket? CancelTicket(Guid ticketId, Guid userId);

        /// <summary>
        /// Retrieves the number of tickets still available for a given event
        /// </summary>
        /// <param name="eventId">The unique identifier of the concert event</param>
        /// <returns>The number of tickets that are still available</returns>
        int GetAvailableTickets(Guid eventId);
    }
}