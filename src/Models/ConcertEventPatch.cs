namespace Ticketmaster.Models
{
    /// <summary>
    /// DTO for partially updating a concert event
    /// </summary>
    public class ConcertEventPatch
    {
        public string? Name { get; set; }
        public string? Venue { get; set; }
        public DateTime? Date { get; set; }
        public int? Capacity { get; set; }
        public Dictionary<string, decimal>? TicketTypes { get; set; }
    }
}
