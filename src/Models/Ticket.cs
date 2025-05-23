namespace Ticketmaster.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime ReservedAt { get; set; }
    }   
}
