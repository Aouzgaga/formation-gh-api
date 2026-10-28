# Copilot Instructions

## Commands

This is a .NET 10 solution (`FormationGhApi.slnx`).

```powershell
dotnet restore .\FormationGhApi.slnx
dotnet build .\FormationGhApi.slnx
dotnet run --project .\src\FormationGhApi.Api\FormationGhApi.Api.csproj
```

The API development profile listens on `http://localhost:5215` (and, when using the HTTPS profile, `https://localhost:7132`). No test projects or lint/format configuration are currently present, so there is no full-suite or single-test command yet.

## Architecture

- `FormationGhApi.Api` is the HTTP boundary. Its controllers bind contract records, send application requests through `IMediator`, and map feature result statuses to HTTP responses. Do not inject `CongesDbContext` into controllers.
- `FormationGhApi.Services` contains the application and persistence code. Each use case lives in `Features/<area>/<use-case>/` as a request record and an `IRequestHandler` implementation. Handlers use `CongesDbContext`, project domain entities into DTO records, and accept/pass through `CancellationToken`.
- The repository uses its own reflection-based mediator, not MediatR. Every new `IRequest<TResponse>` handler must be explicitly registered in `Extensions\ServiceCollectionExtensions.AddCongesServices`; otherwise `Mediator.SendAsync` throws at runtime.
- EF Core maps `Utilisateur`, `CompteurConges`, and `DemandeConges` to SQLite. `CongesDbContext` keeps computed properties (`JoursSolde`, `NombreJours`) out of persistence. The API calls `DatabaseSeeder.Seed` at startup, which creates the schema and seeds three users with 25 days for the current UTC year only when no users exist.

## Repository Conventions

- Keep API contracts in `Api\Contracts` and application response shapes in `Services\Dtos`; use immutable positional records for commands, queries, request contracts, and DTOs.
- Keep feature outcomes explicit. Mutating handlers return a result record with a French status enum when the controller needs to distinguish business failures (for example, insufficient balance) from success; controllers own the HTTP-status mapping.
- Query handlers use `AsNoTracking()` and stable projections/orderings. Mutation handlers load tracked entities and call `SaveChangesAsync(cancellationToken)`.
- The leave-request duration is inclusive calendar days: `DateFin - DateDebut + 1`, including weekends and holidays. Preserve this rule when changing `DemandeConges` or its DTOs.
- The configured `Data Source=conges.db` is a file-backed, relative SQLite database. It is intentionally untracked (`*.db`); account for persisted local state when manually exercising endpoints.

## Données de test

Utilise principalement l'utilisateur de test dont l'id est 1