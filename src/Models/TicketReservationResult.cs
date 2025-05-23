namespace Ticketmaster.Models
{
    public class TicketReservationResult
    {
        public ReservationStatus Status { get; set; }
        public Ticket? Ticket { get; set; }
    }
}
