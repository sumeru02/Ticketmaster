namespace Ticketmaster.Models
{
    public enum ReservationStatus
    {
        Success,
        EventNotFound,
        CapacityFull,
        DuplicateRequest,
        NotAuthorized
    }
}
