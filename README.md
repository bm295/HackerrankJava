# HackerrankJava

This repository uses a C# layered architecture to index and display the original HackerRank Java solutions.

## Prerequisites

- .NET 9 SDK installed
- Git

This repo targets `net9.0` and uses `LangVersion=preview` (see `Directory.Build.props`).

## Setup

1. Clone the repository:

```bash
git clone <your-repo-url>
cd HackerrankJava
```

2. Verify your SDK includes .NET 9:

```bash
dotnet --list-sdks
```

3. Restore dependencies:

```bash
dotnet restore HackerrankJava.sln
```

4. Build:

```bash
dotnet build HackerrankJava.sln
```

## Run

Run the console app from the repository root:

```bash
dotnet run --project src/HackerrankJava.Presentation/HackerrankJava.Presentation.csproj
```

Expected behavior:
- Prints `Hackerrank solutions (layered architecture demo):`
- Lists `.java` files discovered under the `java/` directory

## Repository layout

- `java/`: original HackerRank Java source files
- `src/HackerrankJava.Domain`: core domain models
- `src/HackerrankJava.Application`: use cases and abstractions
- `src/HackerrankJava.Infrastructure`: file-system implementations
- `src/HackerrankJava.Presentation`: console entrypoint

## Layer dependency direction

- `Presentation -> Application -> Domain`
- `Infrastructure -> Application`
- `Domain` has no dependency on other layers
