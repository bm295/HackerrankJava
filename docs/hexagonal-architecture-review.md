# Restaurant Management System Architecture Review

## 1. Repository overview

- Repository is a mixed codebase with legacy Java exercise files and a C# solution focused on FnB management (`HackerrankJava.sln`).
- C# implementation is split into four projects:
  - `HackerrankJava.Domain`
  - `HackerrankJava.Application`
  - `HackerrankJava.Infrastructure`
  - `HackerrankJava.Presentation`
- Global build configuration sets `.NET 10` (`net10.0`) and `C# 14` (`LangVersion 14.0`).
- Current API surface supports profile/tables/menu/open orders/upcoming reservations plus order/reservation creation and one food-app integration endpoint.

## 2. Architecture evaluation

### Positive observations

- Layering is present and directionally clean:
  - Domain contains only business types (records/enums).
  - Application declares ports and a use-case-style service.
  - Infrastructure implements persistence adapter via in-memory repository.
  - Presentation maps HTTP endpoints and uses DI.
- Constructor injection and DI registration are used.

### Limitations

- The solution does not have explicit `Adapters` and `Api` folder/project boundaries matching the required conceptual structure.
- Application service is currently a broad facade (`FnbManagementService`) rather than separated use-case handlers.
- Architecture is close to layered clean architecture but only partially explicit as hexagonal (ports/adapters terminology exists, but adapter boundaries are coarse).

## 3. Hexagonal architecture compliance

### Compliant aspects

- Ports are defined in the application layer (`IFnbQueryPort`, `IFnbCommandPort`).
- Infrastructure implements ports (`InMemoryFnbRepository : IFnbQueryPort, IFnbCommandPort`).
- API layer calls application service, not repository directly.
- Domain layer has no infrastructure/framework references.

### Non-compliant / partial aspects

- Port contracts are synchronous only; no async I/O abstractions for realistic persistence/external integrations.
- Only one persistence adapter is present, and it is in-memory with no database/infrastructure abstraction richness.
- No explicit outbound adapter abstractions for payment, kitchen dispatch, inventory events, or reporting sinks.

## 4. Domain model evaluation

- Domain objects are mostly immutable records (`ServiceOrder`, `Reservation`, `DiningTable`, `MenuItem`, `RestaurantProfile`).
- Domain model is anemic:
  - Few behavioral methods and no aggregate invariants beyond simple checks in application service.
  - Critical business states/actions are absent (payment lifecycle, kitchen ticket states beyond enum value, order closing rules, inventory consumption rules).
- Seat-capacity profile (60–80) is represented and table inventory sums to 78 seats, fitting target operations at a static modeling level.

## 5. FnB functionality coverage

### Covered (partially)

- Order creation
- Menu lookup/availability filtering
- Table listing/status visibility
- Reservation creation/listing

### Missing or insufficient against required flows

- Add/remove line items to an existing order.
- Explicit “send order to kitchen” use case/state transition endpoint.
- Payment processing (authorization/capture/settlement/refund, payment entity/port/adapter).
- Inventory tracking and stock deduction tied to order lifecycle.
- Close-order workflow.
- Basic reporting module (sales, table turnover, item performance).

## 6. Dependency direction analysis

- Dependency graph is inward and mostly clean:
  - `Application -> Domain`
  - `Infrastructure -> Application`
  - `Presentation -> Application + Infrastructure`
- Domain has no outgoing dependency to higher layers.
- No direct controller-to-repository usage detected.

## 7. Code quality review

### Strengths

- Simple and readable modern C# style with records and minimal API.
- DI registration is straightforward and testable at composition-root level.
- Guard clauses exist for invalid requests.

### Risks / quality issues

- Compile-time defect likely present: `menuItem.PriceVnd` is referenced but `MenuItem` defines `Price`.
- No asynchronous programming in ports/services/endpoints despite requirement to use async where appropriate.
- No automated tests observed for domain/application flows.
- In-memory persistence limits realism for concurrency, durability, auditability, and reporting.

## 8. Identified architectural violations

1. Requirement mismatch: missing core FnB modules (payments, inventory, reporting, close-order lifecycle).
2. Partial hexagonal separation: adapters are not explicitly organized by type (API/persistence/external) and use cases are not granular command/query handlers.
3. Async requirement not met (synchronous interfaces end-to-end).
4. Build integrity risk due to inconsistent property naming in food-app integration mapping.

## 9. Recommended refactoring

1. **Restructure to explicit hexagonal boundaries**
   - `/src/Domain`
   - `/src/Application` (use cases + ports)
   - `/src/Adapters` (Api, Persistence, External)
   - `/src/Infrastructure` (EF Core, messaging, configuration)
2. **Split use cases**
   - `CreateOrder`, `AddOrderItem`, `RemoveOrderItem`, `SendOrderToKitchen`, `ProcessPayment`, `DeductInventory`, `CloseOrder`, `GetSalesReport`.
3. **Introduce async ports and handlers**
   - `Task<T>` / `ValueTask<T>` with cancellation tokens.
4. **Model missing domain concepts**
   - `Payment`, `InventoryItem`, `StockMovement`, `KitchenTicket`, `OrderLifecyclePolicy`.
5. **Add adapters**
   - Persistence (EF Core repository implementations)
   - Payment gateway adapter
   - Kitchen printer/queue adapter
   - Reporting query adapter.
6. **Enforce invariants inside domain aggregates**
   - Keep business transitions in domain methods, not endpoint/service glue.
7. **Add tests**
   - Domain unit tests for order/payment/inventory rules.
   - Application tests for use-case orchestration with mocked ports.
   - API integration tests for end-to-end flows.
8. **Fix compile issue**
   - Align `PriceVnd` reference with `MenuItem.Price` (or introduce explicit currency value object and naming convention).

## 10. Overall verdict

**PARTIAL** — The repository demonstrates a clean layered baseline with port/repository abstractions, but it does not yet satisfy the full restaurant-management requirements nor fully realize hexagonal architecture for production-grade FnB operations.
