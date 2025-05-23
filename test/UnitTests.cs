using Ticketmaster.Models;
using Ticketmaster.Services;

namespace Ticketmaster.Tests
{
    [TestClass]
    public class UnitTests : TestCaseBase
    {
        [TestMethod]
        public void ReserveTicket_Success()
        {
            TicketReservationResult result = TicketService.ReserveTicket(TestEventId, TestUserId, "idem-key");
            result.Log("TicketReservationResult");

            Console.WriteLine($"Expected Reservation Status: {ReservationStatus.Success}");
            Console.WriteLine($"Actual Reservation Status: {result.Status}");

            Assert.AreEqual(ReservationStatus.Success, result.Status);
            Assert.IsNotNull(result.Ticket);
            Assert.AreEqual(TicketStatus.Reserved, result.Ticket.Status);
        }

        [TestMethod]
        public void ReserveTicket_DuplicateIdempotencyKey_ReturnsDuplicate()
        {
            ReserveTicket("dup-key");
            TicketReservationResult result = TicketService.ReserveTicket(TestEventId, TestUserId, "dup-key");
            Assert.AreEqual(ReservationStatus.DuplicateRequest, result.Status);
        }

        [TestMethod]
        public void ReserveTicket_OverCapacity_ReturnsCapacityFull()
        {
            // Reserve max tickets
            for (int i = 0; i < 5; i++)
            {
                var userId = Guid.NewGuid();
                TicketService.ReserveTicket(TestEventId, userId, $"key-{i}");
            }

            var result = TicketService.ReserveTicket(TestEventId, TestUserId, "over-capacity");

            Assert.AreEqual(ReservationStatus.CapacityFull, result.Status);
        }

        [TestMethod]
        public void PurchaseTicket_Success_ChangesStatusToPurchased()
        {
            Ticket ticket = ReserveTicket("buy-key");
            TicketPurchaseResult result = PurchaseTicket(ticket.Id);

            Assert.AreEqual(PurchaseStatus.Success, result.Status);
            Assert.AreEqual(TicketStatus.Purchased, result.Ticket.Status);
        }

        [TestMethod]
        public void PurchaseTicket_AlreadyPurchased_ReturnsAlreadyPurchased()
        {
            Ticket ticket = ReserveTicket("repurchase");
            TicketPurchaseResult result = PurchaseTicket(ticket.Id);
            result = PurchaseTicket(ticket.Id);
            Assert.AreEqual(PurchaseStatus.AlreadyPurchased, result.Status);
        }

        [TestMethod]
        public void PurchaseTicket_InvalidTicket_ReturnsNotFound()
        {
            // Random tickerId should yield PurchaseStatus TicketNotFound
            Guid ticketId = Guid.NewGuid();

            TicketPurchaseResult result = PurchaseTicket(ticketId);
            Assert.AreEqual(PurchaseStatus.TicketNotFound, result.Status);
        }

        [TestMethod]
        public void CancelTicket_Success_ChangesStatusToCancelled()
        {
            Ticket ticket = ReserveTicket("cancel-key");
            Ticket result = CancelTicket(ticket.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(TicketStatus.Cancelled, result.Status);
        }

        [TestMethod]
        public void ExpiredTicket_IsNotCountedInReservation()
        {
            Ticket ticket = ReserveTicket("expire-soon");

            // Simulate expiration by modifying ReservedAt
            var expiredField = typeof(Ticket).GetProperty(nameof(Ticket.ReservedAt))!;
            expiredField.SetValue(ticket, DateTime.UtcNow.AddMinutes(-11));

            int available = TicketService.GetAvailableTickets(TestEventId);
            Assert.AreEqual(5, available);
        }

        [TestMethod]
        public void PurchaseTicket_UnauthorizedUser_ReturnsNotAuthorized()
        {
            Ticket ticket = ReserveTicket("unauth-key");

            // UserId mismatch should yield PurchaseStatus NotAuthorized
            Guid userId = Guid.NewGuid();
            var result = TicketService.PurchaseTicket(ticket.Id, userId);

            Assert.AreEqual(PurchaseStatus.NotAuthorized, result.Status);
        }

        [TestMethod]
        public void GetAvailableTickets_DecrementsAfterReservation()
        {
            ReserveTicket();
            int available = TicketService.GetAvailableTickets(TestEventId);
            Console.WriteLine($"Available: {available}. Capacity was 5");
            Assert.AreEqual(4, available);
        }
    }
}
