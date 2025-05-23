# Ticketmaster

An in-memory ticket reservation system built using ASP.NET Core Web API. The application allows users to browse events, reserve tickets with idempotency support, purchase tickets, and cancel reservations.

## Features

- Create or update concert events
- Reserve tickets using idempotent keys
- Purchase and cancel tickets
- Memory-based store with extensibility for Redis or database backends
- Clean architecture with separation of concerns via interfaces

## Project Structure

```
Ticketmaster/
├── Controllers/           # API endpoints (EventController, TicketsController)
├── Data/                  # Sample event loader
├── Helper/                # Utility classes like EventHelper, TokenHelper
├── Interfaces/            # IEventService, ITicketService contracts
├── Models/                # Core entities: ConcertEvent, Ticket, etc.
├── Services/              # In-memory services: EventService, TicketService
├── Program.cs             # Entry point with WebApplication configuration
├── appsettings.json       # App config (can extend for Redis, DB)
```

## Purchase Flow Design

```
[Client App]
   ↓
[TicketsController]
   ↓
[TicketService]
   ↓
 Reservation Flow:
   1. Validate idempotencyKey
   2. Check existing ticket
   3. Fetch Event from IEventService
   4. Check capacity
   5. Check duplicate reserve
   6. Create Ticket
   7. Cache with TTL
   ↓
 Purchase Flow:
   1. Retrieve Ticket
   2. Check expiration
   3. Validate UserId
   4. Check Already Purchased
   5. Simulate Payment
   6. Mark as Purchased
   ↓
[ConcurrentDictionary<Guid, Ticket> _tickets]
   ↑
[ConcurrentDictionary<string, Ticket> _idempotencyKeys]
   ↑
[EventService (via IEventService)]
```

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### Run Locally

```bash
git clone <repo-url>
cd Ticketmaster
dotnet run
```

Purchasing a Ticket is a 2 step process:
- Reserve a ticket
- Purchase a ticket

## Sample API Calls

Step 1: Reserve a ticket

```http
POST /tickets/{eventId}/reserve
Headers: Idempotency-Key: some-unique-key

Response:
{
  "Status": "Success",
  "Ticket": {
    "Id": "d7e0ee3d-612c-4158-bd32-b28ae26bec1f",
    "EventId": "8160c611-e9f2-4e5a-9ee0-29fc6162f851",
    "UserId": "7005d506-94b8-4e73-9d6d-ac3a385dc006",
    "Status": "Reserved",
    "ReservedAt": "2025-05-23T01:29:23.1070392Z"
  }
}

```
The returned ticketId is required for the purchase call. <br><br>
Step 2: Purchase a ticket

```http

POST /tickets/{ticketId}/purchase

Response:
{
  "Status": "Success",
  "Ticket": {
    "Id": "d7e0ee3d-612c-4158-bd32-b28ae26bec1f",
    "EventId": "8160c611-e9f2-4e5a-9ee0-29fc6162f851",
    "UserId": "7005d506-94b8-4e73-9d6d-ac3a385dc006",
    "Status": "Purchased",
    "ReservedAt": "2025-05-23T01:33:48.8734626Z"
  }
}
```

Cancel a ticket:
```http
DELETE /tickets/{ticketId}
```

## Unit Tests

Unit tests are located in the `Tests/` folder. To run them:

```bash
dotnet test
```

## Extending the App

- Add Redis caching by replacing in-memory dictionaries in `TicketService`
- Persist events and tickets to a real ACID compliant database such as Azure SQL
- Add authentication and role-based access (via JWT)