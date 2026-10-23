# Copilot Instructions

EShop is a .NET 10 / Aspire 13 distributed app: a Minimal API backend plus a Blazor Server frontend. The goal is CRUD for Products, Shopping Cart and Orders; only Products exist so far. All code lives under `src\`, and the solution is `src\EShop.sln`.

## Build and run

There are no test projects or linters yet.

```powershell
dotnet build src\EShop.sln
dotnet build src\EShop.ApiService          # build a single project
dotnet run --project src\EShop.AppHost     # start the whole app (API + web) via Aspire
```

Always run via `EShop.AppHost` so service discovery and the `/health` checks work.

## Aspire

- Prefer `aspire run` (from `src\`) to run the distributed app. `src\aspire.config.json` points the Aspire CLI at `EShop.AppHost`. It starts all resources and the Aspire dashboard (logs, traces, metrics).
- Register new services and resources in `AppHost.cs` with `AddProject<Projects.X>(...)`. Wire dependencies with `WithReference(...)` and `WaitFor(...)`, and add `WithHttpHealthCheck("/health")`.
- Respect `AddServiceDefaults()`: every service must call `builder.AddServiceDefaults()` before building and `app.MapDefaultEndpoints()` after. Do not remove or bypass it.
- Reach other services by Aspire resource name (e.g. `https+http://apiservice`), never by hard-coded host or port.

## Architecture

- **EShop.AppHost** (`AppHost.cs`) orchestrates the apps. `webfrontend` references `apiservice` and waits for it (`WithReference` + `WaitFor`). Only the web frontend gets external HTTP endpoints.
- **EShop.ServiceDefaults** is referenced by every service. Each service's `Program.cs` must call `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()` (OpenTelemetry, health checks, resilience, service discovery). Keep any new service wired the same way.
- **EShop.ApiService** is a layered Minimal API:
  - `Models\` holds domain classes (`Product`).
  - `Services\` holds singleton in-memory stores (`ProductService`: a `List<Product>` seeded in the constructor, guarded by a `Lock`). There is no database.
  - `Endpoints\` holds static classes exposing `Map<Entity>Endpoints(this IEndpointRouteBuilder)` extension methods, which `Program.cs` calls after registering the service.
- **EShop.Web** is Blazor Server with interactive server render mode and Bootstrap. It is expected to call the API by Aspire service name (`apiservice`), not by hard-coded URL.

## Conventions

- Services return `null` or `false` on a miss and never know about HTTP. Endpoints translate that into `TypedResults.NotFound()` and use `Results<...>` union return types.
- Endpoints are grouped with `MapGroup("/api/<entity>").WithTags(...)`, and each route has `.WithName(...)`. `POST` returns `CreatedAtRoute` pointing at the `GetById` route name.
- A new entity follows the same shape: `Models\X.cs`, `Services\XService.cs` (registered with `AddSingleton`), `Endpoints\XEndpoints.cs`, plus examples in `EShop.ApiService.http`.
- Target `net10.0` for all projects, with nullable reference types and implicit usings enabled.

### Styling and best practices

- Use C# 12+ features. Prefer collection expressions (`[]`, `[.. items]`, `List<T> x = [new() { ... }]`) over `new List<T>()`, `.ToList()` or `.ToArray()`. Also use primary constructors and `System.Threading.Lock` for locking.
- Minimal API structure:
  - Don't put route handlers in `Program.cs`; put them in `Endpoints\<Entity>Endpoints.cs`.
  - Define routes with `MapGroup("/api/<entity>")` and map relative routes (`"/"`, `"/{id:int}"`) on the group.
  - Name the extension method `Map<Entity>Endpoints(this IEndpointRouteBuilder app)`, return `app` and call it from `Program.cs`.
  - Return `TypedResults` with `Results<T1, T2>` union types so OpenAPI metadata is inferred.
  - Inject services as handler parameters; don't resolve them manually.
  - Keep `Program.cs` to service registration and the `Map*` calls.
- Manual API testing uses `src\EShop.ApiService\EShop.ApiService.http`. Its host port must match `launchSettings.json`.
