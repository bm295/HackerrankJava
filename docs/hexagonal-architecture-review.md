# Restaurant Management System Architecture Review

## 1. Repository overview

- The repository contains two unrelated code areas:
  - A C# solution (`HackerrankJava.sln`) implementing an FnB API.
  - Legacy Java practice files under `/java`.
- The FnB implementation is split into four projects under `/src`:
  - `HackerrankJava.Domain`
  - `HackerrankJava.Application`
  - `HackerrankJava.Infrastructure`
  - `HackerrankJava.Presentation`
- Global build settings target `.NET 10` (`net10.0`) and `C# 14` (`LangVersion 14.0`).
- Exposed API endpoints currently cover:
  - restaurant profile
  - table list
  - available menu items
  - open orders
  - upcoming reservations
  - create order
  - create reservation
  - integrate external food-app order

## 2. Architecture evaluation

### What is good

- There is clear project-level layering and separation:
  - Domain: core entities/value-like records and enums.
  - Application: orchestration service and port interfaces.
  - Infrastructure: in-memory repository implementing application ports.
  - Presentation: minimal API endpoints and composition root (DI registrations).
- Controllers/endpoints call an application service (`FnbManagementService`) rather than directly using repository internals.

### What is lacking

- The repository does not follow the expected explicit hexagonal folder layout (`/src/Adapters`, `/src/Api` etc.).
- Use cases are grouped in one service rather than distinct application use-case handlers.
- Adapter boundaries are coarse (single in-memory adapter; no explicit payment/inventory/reporting adapters).

## 3. Hexagonal architecture compliance

### Compliant points

- Ports exist in the application layer (`IFnbQueryPort`, `IFnbCommandPort`).
- An adapter implements those ports (`InMemoryFnbRepository`).
- Dependency direction is mostly inward toward domain.
- Domain models are framework-agnostic and do not reference ASP.NET/infrastructure types.

### Non-compliant / partial points

- Missing explicit adapter categories required for restaurant operations:
  - persistence adapter(s) beyond in-memory storage
  - payment gateway adapter
  - inventory adapter
  - reporting adapter
- Ports are synchronous only; no async contract surface.
- The implementation does not provide full command/query separation (single broad service).

## 4. Domain model evaluation

- Domain includes: `RestaurantProfile`, `DiningTable`, `MenuItem`, `ServiceOrder`, `Reservation` and related enums.
- Seat-capacity target is represented via profile range `60..80` and seeded tables total 78 seats, which fits the stated restaurant size.
- However, domain behavior is mostly anemic:
  - no aggregate methods for lifecycle transitions
  - minimal business rule enforcement in domain itself
  - business invariants largely enforced in application service/repository
- Missing core domain concepts for requirements:
  - `Payment`
  - `InventoryItem` / stock movement
  - `KitchenTicket` / dispatch workflow
  - reporting models

## 5. FnB functionality coverage

### Implemented

- Create order for a table.
- Create reservation.
- List open orders, available menu, tables, and upcoming reservations.
- Basic external food-app ingestion.

### Missing versus required flow

Required realistic flow:
1. Create order for a table ✅
2. Add/remove items ❌ (no update endpoint/use case for existing order lines)
3. Send order to kitchen ❌ (no explicit use case/state transition)
4. Process payment ❌ (no payment model/port/adapter)
5. Deduct inventory ❌ (no stock model/rules/adapter)
6. Close order ❌ (no close-order use case)

Also missing:
- Basic reporting (sales, table turnover, product performance).

## 6. Dependency direction analysis

- Current compile-time dependency flow is:
  - `Application -> Domain`
  - `Infrastructure -> Application`
  - `Presentation -> Application + Infrastructure`
- This is directionally acceptable for hexagonal architecture.
- No direct controller-to-repository access found in endpoints; endpoints use `FnbManagementService`.
- Domain layer remains isolated from infrastructure/framework concerns.

## 7. Code quality review

### Strengths

- Clean readable C# style with records and minimal API mapping.
- DI setup is straightforward and clear at composition root.
- Guard clauses exist in service methods for common invalid input.

### Weaknesses

- No asynchronous programming usage in ports/use cases/endpoints.
- No test project observed for domain/application/API behavior.
- In-memory repository is non-durable and unsuitable for realistic restaurant operations.
- Detected correctness/build issue: `PriceVnd` is referenced in food-app integration but `MenuItem` exposes `Price`.

## 8. Identified architectural violations

1. **Requirement mismatch:** Required payment, inventory, order-close, and reporting capabilities are not implemented.
2. **Partial hexagonal implementation:** Ports exist, but adapters are not fully decomposed by responsibility.
3. **Async requirement violation:** End-to-end synchronous contracts only.
4. **Domain-rule leakage:** Important workflow/business transitions are not modeled as domain behavior.
5. **Potential build defect:** Inconsistent menu price property naming in integration mapping.

## 9. Recommended refactoring

1. **Restructure explicitly around hexagonal concepts**
   - `/src/Domain`
   - `/src/Application` (commands/queries/use cases + inbound/outbound ports)
   - `/src/Adapters/Api`
   - `/src/Adapters/Persistence`
   - `/src/Adapters/External`
   - `/src/Infrastructure` (framework/host wiring)

2. **Create dedicated use cases (one class/handler each)**
   - `CreateOrder`
   - `AddOrderItem`
   - `RemoveOrderItem`
   - `SendOrderToKitchen`
   - `ProcessPayment`
   - `DeductInventory`
   - `CloseOrder`
   - reporting queries

3. **Introduce outbound ports and adapters**
   - `IPaymentGatewayPort`
   - `IInventoryPort`
   - `IKitchenDispatchPort`
   - `IReportingQueryPort`

4. **Adopt async-first contracts**
   - `Task`/`ValueTask` return types and `CancellationToken` across API -> application -> adapters.

5. **Strengthen domain model**
   - Move lifecycle rules into aggregates/entities.
   - Introduce explicit domain events for kitchen dispatch, stock deduction, payment confirmation.

6. **Fix integration property mismatch**
   - Replace `menuItem.PriceVnd` with `menuItem.Price` or create a clear money value object naming convention.

7. **Add test coverage**
   - Domain invariant unit tests.
   - Application use-case tests with mocked ports.
   - API integration tests for complete order-to-payment-to-close flow.

## 10. Overall verdict

**PARTIAL** — The repository now implements the core FnB flow (order creation/editing, kitchen send, payment, inventory deduction, close order, and basic reporting) with inward dependencies and async ports, but still only partially realizes explicit hexagonal packaging (for example, missing dedicated /Adapters project segmentation and richer infrastructure adapters).
