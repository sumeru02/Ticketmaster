using System.Collections.Concurrent;
using Ticketmaster.Models;

namespace Ticketmaster.Helper
{
    public static class EventHelper
    {
        public static ConcurrentDictionary<Guid, ConcertEvent> LoadSampleEvents()
        {
            var ev1 = new ConcertEvent
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Rock Night 2025",
                Venue = "Seattle Dome",
                Date = new DateTime(2025, 6, 15, 20, 0, 0, DateTimeKind.Utc),
                Capacity = 5000,
                TicketTypes = new Dictionary<string, decimal>
                {
                    { "General", 50.00m },
                    { "VIP", 120.00m }
                }
            };

            var ev2 = new ConcertEvent
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "Jazz Evening",
                Venue = "Portland Theater",
                Date = new DateTime(2025, 7, 10, 19, 30, 0, DateTimeKind.Utc),
                Capacity = 1500,
                TicketTypes = new Dictionary<string, decimal>
                {
                    { "Balcony", 40.00m },
                    { "Front Row", 80.00m }
                }
            };

            return new ConcurrentDictionary<Guid, ConcertEvent>
            {
                [ev1.Id] = ev1,
                [ev2.Id] = ev2
            };
        }
    }
}
