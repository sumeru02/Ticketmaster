using Ticketmaster.Models;

namespace Ticketmaster.Interfaces
{
    /// <summary>
    /// Interface for managing concert events
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Creates a new concert event or updates an existing one
        /// </summary>
        /// <param name="ev">The event to create or update</param>
        /// <returns>The created or updated event with a unique identifier</returns>
        ConcertEvent CreateOrUpdateEvent(ConcertEvent ev);

        /// <summary>
        /// Retrieves a specific concert event by its unique identifier
        /// </summary>
        /// <param name="id">The unique ID of the event</param>
        /// <returns>The matching concert event if found, otherwise null</returns>
        ConcertEvent? GetEvent(Guid id);

        /// <summary>
        /// Retrieves all concert events currently stored
        /// </summary>
        /// <returns>An enumerable collection of all concert events</returns>
        IEnumerable<ConcertEvent> GetAllEvents();
    }
}