namespace Ticketmaster.Models
{
    /// <summary>
    /// Represents a concert event with basic details and ticket information
    /// </summary>
    public class ConcertEvent
    {
        /// <summary>
        /// Unique identifier for the concert event
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the concert or performance
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Name or location of the venue where the concert will be held
        /// </summary>
        public string Venue { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the concert is scheduled
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Total seating capacity available for the event
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// Dictionary of ticket types and their corresponding prices (e.g., "VIP" : 150.00)
        /// </summary>
        public Dictionary<string, decimal> TicketTypes { get; set; } = new();
    }
}
