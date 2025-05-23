namespace Ticketmaster.Models
{
    public enum PurchaseStatus
    {
        Success,
        TicketNotFound,
        AlreadyPurchased,
        NotAuthorized,
        NotReserved,
        PaymentFailed,
        ReservationExpired,
        DuplicateRequest
    }
}
