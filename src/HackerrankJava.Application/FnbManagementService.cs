using HackerrankJava.Domain;

namespace HackerrankJava.Application;

public interface IFnbQueryPort
{
    RestaurantProfile GetRestaurantProfile();
    IReadOnlyCollection<DiningTable> GetTables();
    IReadOnlyCollection<MenuItem> GetMenuItems();
    IReadOnlyCollection<ServiceOrder> GetOrders();
    IReadOnlyCollection<Reservation> GetReservations();
}

public interface IFnbCommandPort
{
    ServiceOrder AddOrder(Guid tableId, IReadOnlyCollection<OrderLine> lines);
    Reservation AddReservation(string guestName, int partySize, DateTimeOffset reservedFor, string contactPhone, string? notes);
}

public sealed class FnbManagementService(IFnbQueryPort queryPort, IFnbCommandPort commandPort)
{
    public RestaurantProfile GetProfile() => queryPort.GetRestaurantProfile();

    public IReadOnlyCollection<DiningTable> GetTables() => queryPort.GetTables();

    public IReadOnlyCollection<MenuItem> GetAvailableMenuItems() => queryPort
        .GetMenuItems()
        .Where(item => item.IsAvailable)
        .OrderBy(item => item.Category, StringComparer.Ordinal)
        .ThenBy(item => item.Name, StringComparer.Ordinal)
        .ToArray();

    public IReadOnlyCollection<ServiceOrder> GetOpenOrders() => queryPort
        .GetOrders()
        .Where(order => order.Status is OrderStatus.Open or OrderStatus.SentToKitchen)
        .OrderByDescending(order => order.CreatedAt)
        .ToArray();

    public IReadOnlyCollection<Reservation> GetUpcomingReservations(DateTimeOffset now) => queryPort
        .GetReservations()
        .Where(reservation => reservation.ReservedFor >= now && reservation.Status is not ReservationStatus.Cancelled)
        .OrderBy(reservation => reservation.ReservedFor)
        .ToArray();

    public ServiceOrder CreateOrder(Guid tableId, IReadOnlyCollection<OrderLine> lines)
    {
        if (lines.Count == 0)
        {
            throw new ArgumentException("An order must contain at least one line item.", nameof(lines));
        }

        return commandPort.AddOrder(tableId, lines);
    }

    public Reservation CreateReservation(string guestName, int partySize, DateTimeOffset reservedFor, string contactPhone, string? notes)
    {
        if (partySize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(partySize), "Party size must be greater than zero.");
        }

        return commandPort.AddReservation(guestName, partySize, reservedFor, contactPhone, notes);
    }

    public FoodAppIntegrationResult IntegrateFoodAppOrder(FoodAppOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SourceApp))
        {
            throw new ArgumentException("Source app is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.ExternalOrderId))
        {
            throw new ArgumentException("External order id is required.", nameof(request));
        }

        if (request.Items.Count == 0)
        {
            throw new ArgumentException("At least one food app item is required.", nameof(request));
        }

        var table = queryPort.GetTables()
            .SingleOrDefault(x => string.Equals(x.Code, request.TableCode, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Table code '{request.TableCode}' was not found.");

        var menuItemsById = queryPort.GetMenuItems().ToDictionary(item => item.Id, item => item);

        var lines = request.Items
            .Select(item =>
            {
                if (!menuItemsById.TryGetValue(item.MenuItemId, out var menuItem))
                {
                    throw new InvalidOperationException($"Menu item '{item.MenuItemId}' was not found.");
                }

                if (!menuItem.IsAvailable)
                {
                    throw new InvalidOperationException($"Menu item '{menuItem.Name}' is not currently available.");
                }

                if (item.Quantity <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(request), "Each item quantity must be greater than zero.");
                }

                return new OrderLine(menuItem.Id, item.Quantity, menuItem.PriceVnd);
            })
            .ToArray();

        var order = CreateOrder(table.Id, lines);

        return new FoodAppIntegrationResult(
            request.SourceApp,
            request.ExternalOrderId,
            order.Id,
            table.Code,
            order.CreatedAt,
            order.Lines.Sum(line => line.Quantity * line.UnitPrice));
    }
}

public sealed record FoodAppOrderRequest(
    string SourceApp,
    string ExternalOrderId,
    string TableCode,
    IReadOnlyCollection<FoodAppOrderItemRequest> Items);

public sealed record FoodAppOrderItemRequest(Guid MenuItemId, int Quantity);

public sealed record FoodAppIntegrationResult(
    string SourceApp,
    string ExternalOrderId,
    Guid InternalOrderId,
    string TableCode,
    DateTimeOffset SyncedAt,
    decimal TotalVnd);
