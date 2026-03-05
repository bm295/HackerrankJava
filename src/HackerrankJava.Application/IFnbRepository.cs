using HackerrankJava.Domain;

namespace HackerrankJava.Application;

public interface IFnbRepository
{
    RestaurantProfile GetRestaurantProfile();
    IReadOnlyCollection<DiningTable> GetTables();
    IReadOnlyCollection<MenuItem> GetMenuItems();
    IReadOnlyCollection<ServiceOrder> GetOrders();
    IReadOnlyCollection<Reservation> GetReservations();
    ServiceOrder AddOrder(Guid tableId, IReadOnlyCollection<OrderLine> lines);
    Reservation AddReservation(string guestName, int partySize, DateTimeOffset reservedFor, string contactPhone, string? notes);
}
