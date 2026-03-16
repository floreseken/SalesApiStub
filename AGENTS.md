# AGENTS.md

## Purpose
- This is a .NET 10 ASP.NET Core **stub API** for sales workflows (token issuance, products, orders, caller identity), with all data stored in memory.

## Architecture You Need First
- `Program.cs` composes everything: controllers, JWT bearer auth, authorization, OpenAPI transformers, and singleton services.
- Request flow is: controller (`Controllers/*.cs`) -> service (`Services/*.cs`) -> DTO/model (`Models/*.cs`) -> JSON response.
- `AuthController` (`POST /oauth/token`) is the token issuer; all other API controllers are protected by `[Authorize]`.
- OpenAPI security metadata is centralized in `Transformers/OpenApiTransformers.cs` (`SecurityDocumentTransformer`, `AuthorizeOperationTransformer`).
- State is process-local: `ProductService` and `OrderService` are singletons with in-memory collections and locking.

## Service Boundaries and Data Rules
- `ProductService` is inventory + pricing source of truth; it reserves stock atomically in `TryReserveStock(...)` under a lock.
- `OrderService` validates order items, calls stock reservation, computes totals, and stores created orders in `_orders`.
- Pricing behavior is fixed in `OrderService`: subtotal/tax rounding uses `MidpointRounding.AwayFromZero`; tax is hard-coded at `0.08m`.
- Mixed-currency orders are rejected (`"Order contains products with mixed currencies..."`), so preserve currency consistency rules when extending products.

## Auth and Claims Contract
- OAuth config lives in `appsettings.json` under `OAuth` (`Issuer`, `Audience`, `SecretKey`, `TokenExpirationSeconds`, `Clients`).
- Token endpoint only supports `grant_type=client_credentials`; invalid cases return RFC-style OAuth errors (`Models/OAuthError.cs`).
- Issued JWT claims include `sub`, `jti`, `client_id`, and a space-delimited `scope` claim (`AuthController.IssueToken`).
- In `AuthController.Token`, an empty/whitespace `scope` request currently means "grant all scopes allowed for that client".
- `MyInfoController` expects those exact claim names when building `MyInfoResponse`.
- `AuthorizeOperationTransformer` currently adds `sales:read` to secured operations in OpenAPI; keep this aligned with what tokens can receive.

## Conventions Specific to This Repo
- Public API routes are explicit and stable: `/oauth/token`, `/api/sales/products`, `/api/sales/orders`, `/api/sales/my-info`.
- Controllers use `[ApiController]` + data annotations for request validation (`CreateOrderRequest`, `OrderItemRequest`).
- Current order create payload contract is `CreateOrderRequest { shippingAddressId, items[] }` where each item is `{ productId, quantity }`.
- Non-OAuth business failures typically return `BadRequest(new { message = ... })` or `NotFound(new { message = ... })`.
- XML docs are used heavily for OpenAPI descriptions (`GenerateDocumentationFile` enabled in `SalesApiStub.csproj`).

## Developer Workflows (verified)
- Build: `dotnet build`
- Test: `dotnet test` (currently no dedicated test project; command succeeds as build validation)
- Run local API: `dotnet run` (profiles in `Properties/launchSettings.json`: HTTP `5234`, HTTPS `7065`)
- Interactive docs in Development only: `/docs` (Scalar UI) and `/openapi/v1.json`
- Use `SalesApiStub.http` as the canonical smoke flow: request token -> call authorized endpoints with `Bearer` token.

## Guardrails for Future Agent Changes
- If you add secured endpoints, decorate with `[Authorize]` so OpenAPI transformers attach OAuth2 requirements automatically.
- If you change token claims or OAuth config keys, update both `AuthController` and `MyInfoController` (and docs/examples in `SalesApiStub.http`).
- Keep thread-safety semantics when mutating inventory/orders (preserve locking around shared mutable collections).
- Do not introduce persistence assumptions unless you also redesign service lifetimes and storage boundaries explicitly.

## Cross-File Sync Checklist
- Token claim shape changes: update both `Controllers/AuthController.cs` and `Controllers/MyInfoController.cs`.
- Scope model changes: update `Controllers/AuthController.cs`, `Transformers/OpenApiTransformers.cs`, and token examples in `SalesApiStub.http`.
- Order request DTO changes: update `Models/CreateOrderRequest.cs`, `Controllers/OrdersController.cs`, and request samples in `SalesApiStub.http`.
- Route/auth metadata changes: verify `[Authorize]` usage and confirm OpenAPI output at `/openapi/v1.json`.

## Known Drift Watchouts
- `SalesApiStub.http` currently shows order fields (`customerId`, `customerEmail`, `shippingAddress`) that do not match `Models/CreateOrderRequest.cs`; reconcile examples before calling the smoke flow "verified".

