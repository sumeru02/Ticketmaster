using System.Collections.Concurrent;
using Ticketmaster.Helper;
using Ticketmaster.Interfaces;
using Ticketmaster.Models;

/// <summary>
/// Service for managing concert events in an in-memory store.
/// </summary>
public class EventService : IEventService
{
    // Stores all concert events by their unique ID
    private readonly ConcurrentDictionary<Guid, ConcertEvent> _events;

    /// <summary>
    /// Initializes the event service with preloaded sample events.
    /// </summary>
    public EventService()
    {
        _events = EventHelper.LoadSampleEvents();
    }

    /// <summary>
    /// Creates a new event or updates an existing one.
    /// If the event has no ID, a new one is generated.
    /// </summary>
    /// <param name="ev">The event to create or update</param>
    /// <returns>The created or updated event</returns>
    public ConcertEvent CreateOrUpdateEvent(ConcertEvent ev)
    {
        if (ev.Id == Guid.Empty)
            ev.Id = Guid.NewGuid();

        _events[ev.Id] = ev;
        return ev;
    }

    /// <summary>
    /// Retrieves all events currently stored.
    /// </summary>
    public IEnumerable<ConcertEvent> GetAllEvents() => _events.Values;

    /// <summary>
    /// Retrieves a specific event by its unique ID.
    /// </summary>
    /// <param name="id">The ID of the event</param>
    /// <returns>The matching event, or null if not found</returns>
    public ConcertEvent? GetEvent(Guid id) =>
        _events.TryGetValue(id, out var ev) ? ev : null;
}