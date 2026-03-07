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

    public RestaurantProfile GetRestaurantProfile() => _profile;

    public IReadOnlyCollection<DiningTable> GetTables() => _tables.AsReadOnly();

    public IReadOnlyCollection<MenuItem> GetMenuItems() => _menuItems.AsReadOnly();

    public IReadOnlyCollection<ServiceOrder> GetOrders() => _orders.AsReadOnly();

    public IReadOnlyCollection<Reservation> GetReservations() => _reservations.AsReadOnly();

    public ServiceOrder AddOrder(Guid tableId, IReadOnlyCollection<OrderLine> lines)
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
        return order;
    }

    public Reservation AddReservation(string guestName, int partySize, DateTimeOffset reservedFor, string contactPhone, string? notes)
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
        return reservation;
    }
}
