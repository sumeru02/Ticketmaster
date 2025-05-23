using System;
using System.Text.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ticketmaster.Models;
using Ticketmaster.Services;

namespace Ticketmaster.Tests
{
    public abstract class TestCaseBase
    {
        protected EventService EventService;
        protected TicketService TicketService;
        protected Guid TestEventId;
        protected Guid TestUserId;

        [TestInitialize]
        public void Setup()
        {
            EventService = new EventService();
            TicketService = new TicketService(EventService);

            TestEventId = Guid.NewGuid();
            TestUserId = Guid.NewGuid();

            EventService.CreateOrUpdateEvent(new ConcertEvent
            {
                Id = TestEventId,
                Name = "Test Event",
                Venue = "Venue",
                Date = DateTime.UtcNow,
                Capacity = 5
            });
        }

        protected Ticket CancelTicket(Guid ticketId)
        {
            Ticket ticket = TicketService.CancelTicket(ticketId, TestUserId);
            ticket.Log("Ticket Cancelled");
            return ticket;
        }

        protected TicketPurchaseResult PurchaseTicket(Guid ticketId)
        {
            TicketPurchaseResult result = TicketService.PurchaseTicket(ticketId, TestUserId);
            result.Log("TicketPurchaseResult");
            return result;
        }

        protected Ticket ReserveTicket(string idempotencyKey = "key")
        {
            TicketReservationResult result = TicketService.ReserveTicket(TestEventId, TestUserId, idempotencyKey);
            result.Log("TicketReservationResult");
            return result.Ticket;
        }
    }

    public static class ObjectExtensions
    {
        public static void Log(this object obj, string label = null, JsonSerializerOptions options = null)
        {
            options ??= new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var json = JsonSerializer.Serialize(obj, options);

            if (!string.IsNullOrWhiteSpace(label))
            {
                Console.WriteLine(label);
                Console.WriteLine(json);
            }
            else
            {
                Console.WriteLine(json);
            }
        }
    }
}