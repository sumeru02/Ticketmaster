namespace Ticketmaster.Models
{
    public class TicketPurchaseResult
    {
        public PurchaseStatus Status { get; set; }
        public Ticket? Ticket { get; set; }
    }
}
