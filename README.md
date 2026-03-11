# Hemispheres FnB Management Application

This repository now contains a layered C# application for **Hemispheres Steak & Seafood Grill (Sheraton Hanoi)**.

## Restaurant scope

- Venue: Hemispheres Steak & Seafood Grill — Sheraton Hanoi
- Capacity target: approximately **60–80 seats**

## Tech stack

- **C# 14**
- **.NET 10** (`net10.0`)
- Layered architecture:
  - Presentation (Minimal API)
  - Application (use-case service + ports)
  - Infrastructure (in-memory adapters)
  - Domain (entities and value objects)

## Features implemented

- Restaurant profile and seating capacity range
- Dining table management overview
- Menu management overview with availability filtering
- Full order lifecycle operations:
  - create order
  - add/remove items
  - send order to kitchen
  - process payment
  - deduct inventory on close
  - close order
- Reservation creation and upcoming reservation listing
- Inventory snapshot endpoint
- Basic sales report endpoint
- Food-app order integration endpoint

## Run

```bash
dotnet run --project src/HackerrankJava.Presentation/HackerrankJava.Presentation.csproj
```

The API starts locally and exposes endpoints such as:

- `GET /`
- `GET /tables`
- `GET /menu`
- `GET /orders/open`
- `POST /orders`
- `POST /orders/{orderId}/items`
- `DELETE /orders/{orderId}/items`
- `POST /orders/{orderId}/send-to-kitchen`
- `POST /orders/{orderId}/payments`
- `POST /orders/{orderId}/close`
- `GET /inventory`
- `GET /reports/sales`
- `GET /reservations/upcoming`
- `POST /reservations`
- `POST /integrations/food-app/orders`
