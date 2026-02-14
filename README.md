# HackerrankJava

This repository now uses a **C# 14 (preview)** solution layout and follows a layered architecture.

## Repository layout

- `java/` keeps all original HackerRank Java source files.
- `src/HackerrankJava.Domain` contains core domain models.
- `src/HackerrankJava.Application` contains use cases and abstractions.
- `src/HackerrankJava.Infrastructure` contains file-system based implementations.
- `src/HackerrankJava.Presentation` contains the console entrypoint.

## Language and runtime

- `Directory.Build.props` sets `LangVersion` to `preview` for C# 14 features.
- Target framework is `net10.0` (preview).

## Layer dependency direction

- `Presentation -> Application -> Domain`
- `Infrastructure -> Application`
- `Domain` has no dependency on other layers.
