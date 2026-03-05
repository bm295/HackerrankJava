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
  - Application (use-case service)
  - Infrastructure (in-memory repository)
  - Domain (entities and value objects)

## Features implemented

- Restaurant profile and seating capacity range
- Dining table management overview
- Menu management overview with availability filtering
- Open order listing and order creation
- Upcoming reservation listing and reservation creation

## Run

```bash
dotnet run --project src/HackerrankJava.Presentation/HackerrankJava.Presentation.csproj
```

The API starts locally and exposes endpoints such as:

- `GET /`
- `GET /tables`
- `GET /menu`
- `GET /orders/open`
- `GET /reservations/upcoming`
- `POST /orders`
- `POST /reservations`
