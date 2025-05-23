using Microsoft.AspNetCore.Mvc;
using Ticketmaster.Interfaces;
using Ticketmaster.Models;

namespace Ticketmaster.Controllers
{
    /// <summary>
    /// Controller for managing concert events
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("events")]
    [Produces("application/json")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        /// <summary>
        /// Constructor for injecting event service
        /// </summary>
        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Creates or updates a concert event
        /// </summary>
        [HttpPost]
        public IActionResult CreateOrUpdate([FromBody] ConcertEvent ev)
        {
            if (ev == null)
                return BadRequest(new { Message = "Event data is required" });

            if (string.IsNullOrWhiteSpace(ev.Name))
                return BadRequest(new { Message = "Event name is required" });

            if (string.IsNullOrWhiteSpace(ev.Venue))
                return BadRequest(new { Message = "Venue is required" });

            if (ev.Date == default)
                return BadRequest(new { Message = "Event date must be specified" });

            if (ev.Capacity <= 0)
                return BadRequest(new { Message = "Capacity must be greater than zero" });

            var result = _eventService.CreateOrUpdateEvent(ev);
            return Ok(result);
        }

        /// <summary>
        /// Partially updates an existing concert event
        /// </summary>
        [HttpPatch("{id}")]
        public IActionResult Patch(Guid id, [FromBody] ConcertEventPatch patch)
        {
            if (patch == null)
                return BadRequest(new { Message = "Event Patch data is required" });

            var existing = _eventService.GetEvent(id);
            if (existing == null)
                return NotFound(new { Message = "Event not found" });

            // Apply only non-null fields
            if (patch.Name != null)
                existing.Name = patch.Name;

            if (patch.Venue != null)
                existing.Venue = patch.Venue;

            if (patch.Date.HasValue)
                existing.Date = patch.Date.Value;

            if (patch.Capacity.HasValue)
                existing.Capacity = patch.Capacity.Value;

            if (patch.TicketTypes != null)
                existing.TicketTypes = patch.TicketTypes;

            var updated = _eventService.CreateOrUpdateEvent(existing);
            return Ok(updated);
        }

        /// <summary>
        /// Retrieves all concert events
        /// </summary>
        [HttpGet]
        public IActionResult GetAll()
        {
            var events = _eventService.GetAllEvents();
            return Ok(events);
        }

        /// <summary>
        /// Retrieves available ticket count for a specific event
        /// </summary>
        [HttpGet("{id}/availability")]
        public IActionResult GetAvailability(Guid id, [FromServices] ITicketService ticketService)
        {
            var eventExists = _eventService.GetEvent(id);
            if (eventExists == null)
                return NotFound(new { Message = "Event not found" });

            var available = ticketService.GetAvailableTickets(id);
            return Ok(new { Available = available });
        }
    }
}