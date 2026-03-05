using HackerrankJava.Domain;

namespace HackerrankJava.Application;

public sealed class FnbManagementService(IFnbRepository repository)
{
    public RestaurantProfile GetProfile() => repository.GetRestaurantProfile();

    public IReadOnlyCollection<DiningTable> GetTables() => repository.GetTables();

    public IReadOnlyCollection<MenuItem> GetAvailableMenuItems() => repository
        .GetMenuItems()
        .Where(item => item.IsAvailable)
        .OrderBy(item => item.Category, StringComparer.Ordinal)
        .ThenBy(item => item.Name, StringComparer.Ordinal)
        .ToArray();

    public IReadOnlyCollection<ServiceOrder> GetOpenOrders() => repository
        .GetOrders()
        .Where(order => order.Status is OrderStatus.Open or OrderStatus.SentToKitchen)
        .OrderByDescending(order => order.CreatedAt)
        .ToArray();

    public IReadOnlyCollection<Reservation> GetUpcomingReservations(DateTimeOffset now) => repository
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

        return repository.AddOrder(tableId, lines);
    }

    public Reservation CreateReservation(string guestName, int partySize, DateTimeOffset reservedFor, string contactPhone, string? notes)
    {
        if (partySize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(partySize), "Party size must be greater than zero.");
        }

        return repository.AddReservation(guestName, partySize, reservedFor, contactPhone, notes);
    }
}
