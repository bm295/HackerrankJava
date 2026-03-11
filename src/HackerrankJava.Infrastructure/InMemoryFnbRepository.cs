using HackerrankJava.Application;
using HackerrankJava.Domain;

namespace HackerrankJava.Infrastructure;

public sealed class InMemoryFnbRepository : IFnbQueryPort, IFnbCommandPort
{
    private readonly RestaurantProfile _profile = new(
        "Hemispheres Steak & Seafood Grill",
        "Sheraton Hanoi",
        60,
        80);

    private readonly List<DiningTable> _tables =
    [
        new(Guid.NewGuid(), "T01", 4, TableStatus.Available),
        new(Guid.NewGuid(), "T02", 4, TableStatus.Available),
        new(Guid.NewGuid(), "T03", 4, TableStatus.Reserved),
        new(Guid.NewGuid(), "T04", 4, TableStatus.Occupied),
        new(Guid.NewGuid(), "T05", 6, TableStatus.Available),
        new(Guid.NewGuid(), "T06", 6, TableStatus.Available),
        new(Guid.NewGuid(), "T07", 6, TableStatus.Reserved),
        new(Guid.NewGuid(), "T08", 8, TableStatus.Available),
        new(Guid.NewGuid(), "T09", 8, TableStatus.Occupied),
        new(Guid.NewGuid(), "T10", 8, TableStatus.Available),
        new(Guid.NewGuid(), "P01", 10, TableStatus.Available),
        new(Guid.NewGuid(), "P02", 10, TableStatus.Reserved)
    ];

    private readonly List<MenuItem> _menuItems =
    [
        new(Guid.NewGuid(), "USDA Prime Ribeye", "Steak", 1350000m, true),
        new(Guid.NewGuid(), "Grilled Lobster Tail", "Seafood", 1490000m, true),
        new(Guid.NewGuid(), "Pan-Seared Salmon", "Seafood", 690000m, true),
        new(Guid.NewGuid(), "Wagyu Beef Carpaccio", "Starter", 480000m, true),
        new(Guid.NewGuid(), "Caesar Salad", "Starter", 260000m, true),
        new(Guid.NewGuid(), "Chocolate Lava Cake", "Dessert", 240000m, true),
        new(Guid.NewGuid(), "Seasonal Oyster Platter", "Seafood", 820000m, false)
    ];

    private readonly List<ServiceOrder> _orders = [];
    private readonly List<Reservation> _reservations =
    [
        new(Guid.NewGuid(), "Nguyen Minh Anh", 4, DateTimeOffset.UtcNow.AddHours(2), "0901234567", "Window-side table", ReservationStatus.Confirmed),
        new(Guid.NewGuid(), "Tran Hoang Long", 6, DateTimeOffset.UtcNow.AddHours(4), "0912345678", null, ReservationStatus.Pending)
    ];

    private readonly List<Payment> _payments = [];
    private readonly List<StockMovement> _stockMovements = [];
    private readonly List<InventoryItem> _inventory;

    public InMemoryFnbRepository()
    {
        _inventory = _menuItems
            .Select(item => new InventoryItem(Guid.NewGuid(), item.Id, item.Name, 100, "portion"))
            .ToList();
    }

    public Task<RestaurantProfile> GetRestaurantProfileAsync(CancellationToken cancellationToken = default) => Task.FromResult(_profile);

    public Task<IReadOnlyCollection<DiningTable>> GetTablesAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyCollection<DiningTable>>(_tables.AsReadOnly());

    public Task<IReadOnlyCollection<MenuItem>> GetMenuItemsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyCollection<MenuItem>>(_menuItems.AsReadOnly());

    public Task<IReadOnlyCollection<ServiceOrder>> GetOrdersAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyCollection<ServiceOrder>>(_orders.AsReadOnly());

    public Task<IReadOnlyCollection<Reservation>> GetReservationsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyCollection<Reservation>>(_reservations.AsReadOnly());

    public Task<IReadOnlyCollection<InventoryItem>> GetInventoryAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyCollection<InventoryItem>>(_inventory.AsReadOnly());

    public Task<IReadOnlyCollection<Payment>> GetPaymentsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyCollection<Payment>>(_payments.AsReadOnly());

    public Task<ServiceOrder> AddOrderAsync(Guid tableId, IReadOnlyCollection<OrderLine> lines, CancellationToken cancellationToken = default)
    {
        var table = _tables.SingleOrDefault(x => x.Id == tableId)
            ?? throw new InvalidOperationException("Table not found.");

        if (table.Status is TableStatus.Available)
        {
            var tableIndex = _tables.IndexOf(table);
            _tables[tableIndex] = table with { Status = TableStatus.Occupied };
        }

        var order = new ServiceOrder(Guid.NewGuid(), tableId, DateTimeOffset.UtcNow, OrderStatus.Open, lines);
        _orders.Add(order);
        return Task.FromResult(order);
    }

    public Task<ServiceOrder> UpdateOrderLinesAsync(Guid orderId, IReadOnlyCollection<OrderLine> lines, CancellationToken cancellationToken = default)
    {
        var order = _orders.SingleOrDefault(x => x.Id == orderId)
            ?? throw new InvalidOperationException("Order not found.");

        var updatedOrder = order with { Lines = lines };
        _orders[_orders.IndexOf(order)] = updatedOrder;
        return Task.FromResult(updatedOrder);
    }

    public Task<ServiceOrder> UpdateOrderStatusAsync(Guid orderId, OrderStatus status, CancellationToken cancellationToken = default)
    {
        var order = _orders.SingleOrDefault(x => x.Id == orderId)
            ?? throw new InvalidOperationException("Order not found.");

        var updatedOrder = order with { Status = status };
        _orders[_orders.IndexOf(order)] = updatedOrder;

        if (status is OrderStatus.Completed)
        {
            var table = _tables.Single(x => x.Id == order.TableId);
            _tables[_tables.IndexOf(table)] = table with { Status = TableStatus.Available };
        }

        return Task.FromResult(updatedOrder);
    }

    public Task<Reservation> AddReservationAsync(string guestName, int partySize, DateTimeOffset reservedFor, string contactPhone, string? notes, CancellationToken cancellationToken = default)
    {
        var reservation = new Reservation(
            Guid.NewGuid(),
            guestName,
            partySize,
            reservedFor,
            contactPhone,
            notes,
            ReservationStatus.Pending);

        _reservations.Add(reservation);
        return Task.FromResult(reservation);
    }

    public Task<Payment> AddPaymentAsync(Guid orderId, decimal amount, PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        var order = _orders.SingleOrDefault(x => x.Id == orderId)
            ?? throw new InvalidOperationException("Order not found.");

        var payment = new Payment(
            Guid.NewGuid(),
            orderId,
            amount,
            paymentMethod,
            PaymentStatus.Settled,
            DateTimeOffset.UtcNow);

        _payments.Add(payment);
        return Task.FromResult(payment);
    }

    public Task<IReadOnlyCollection<StockMovement>> DeductInventoryForOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = _orders.SingleOrDefault(x => x.Id == orderId)
            ?? throw new InvalidOperationException("Order not found.");

        var movements = new List<StockMovement>();

        foreach (var line in order.Lines)
        {
            var inventoryItem = _inventory.SingleOrDefault(x => x.MenuItemId == line.MenuItemId)
                ?? throw new InvalidOperationException("Inventory item mapping not found.");

            var remaining = inventoryItem.StockQuantity - line.Quantity;
            if (remaining < 0)
            {
                throw new InvalidOperationException($"Insufficient inventory for '{inventoryItem.Name}'.");
            }

            var movement = new StockMovement(
                Guid.NewGuid(),
                inventoryItem.Id,
                orderId,
                -line.Quantity,
                DateTimeOffset.UtcNow,
                "Order Closed");

            movements.Add(movement);
            _stockMovements.Add(movement);

            _inventory[_inventory.IndexOf(inventoryItem)] = inventoryItem with { StockQuantity = remaining };
        }

        return Task.FromResult<IReadOnlyCollection<StockMovement>>(movements.AsReadOnly());
    }
}
