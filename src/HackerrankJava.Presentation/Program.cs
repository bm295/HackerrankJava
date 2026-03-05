using HackerrankJava.Application;
using HackerrankJava.Domain;
using HackerrankJava.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IFnbRepository, InMemoryFnbRepository>();
builder.Services.AddSingleton<FnbManagementService>();

var app = builder.Build();

app.MapGet("/", (FnbManagementService service) =>
{
    var profile = service.GetProfile();
    return Results.Ok(new
    {
        profile.Name,
        profile.Location,
        CapacityRange = $"{profile.SeatCapacityMinimum}-{profile.SeatCapacityMaximum} seats",
        Endpoints = new[]
        {
            "/tables",
            "/menu",
            "/orders/open",
            "/reservations/upcoming"
        }
    });
});

app.MapGet("/tables", (FnbManagementService service) => Results.Ok(service.GetTables()));
app.MapGet("/menu", (FnbManagementService service) => Results.Ok(service.GetAvailableMenuItems()));
app.MapGet("/orders/open", (FnbManagementService service) => Results.Ok(service.GetOpenOrders()));
app.MapGet("/reservations/upcoming", (FnbManagementService service) =>
    Results.Ok(service.GetUpcomingReservations(DateTimeOffset.UtcNow)));

app.MapPost("/orders", (CreateOrderRequest request, FnbManagementService service) =>
{
    try
    {
        var lines = request.Lines.Select(line => new OrderLine(line.MenuItemId, line.Quantity, line.UnitPrice)).ToArray();
        var order = service.CreateOrder(request.TableId, lines);
        return Results.Created($"/orders/{order.Id}", order);
    }
    catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/reservations", (CreateReservationRequest request, FnbManagementService service) =>
{
    try
    {
        var reservation = service.CreateReservation(
            request.GuestName,
            request.PartySize,
            request.ReservedFor,
            request.ContactPhone,
            request.Notes);

        return Results.Created($"/reservations/{reservation.Id}", reservation);
    }
    catch (ArgumentOutOfRangeException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();

public sealed record CreateOrderRequest(Guid TableId, IReadOnlyCollection<CreateOrderLineRequest> Lines);

public sealed record CreateOrderLineRequest(Guid MenuItemId, int Quantity, decimal UnitPrice);

public sealed record CreateReservationRequest(
    string GuestName,
    int PartySize,
    DateTimeOffset ReservedFor,
    string ContactPhone,
    string? Notes);
