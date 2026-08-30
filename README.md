# ForgeFlow

Learning project for Autodesk authentication, ACC model sync, model versioning and Design Automation.
This repo currently contains only the scaffold: a Vue 3 frontend and an ASP.NET Core Web API backend on SQL Server.

## Layout

| Path | What |
| --- | --- |
| `frontend/` | Vue 3 + Vite + Vuetify + Vue Router + Pinia + Axios |
| `backend/` | .NET 10 Web API + EF Core (SQL Server) + ASP.NET Core Identity + OpenAPI/Swagger UI |

## Routes

| Route | Page | Access |
| --- | --- | --- |
| `/login` | Login (single "Login with Autodesk" button, `console.log` placeholder) | public |
| `/` | Dashboard | requires `auth.isAuthenticated` |

The router guard sends every non-public route to `/login`. Sign-in state is the placeholder
Pinia store `src/stores/auth.js` (backed by `sessionStorage`); the Autodesk OAuth flow will
call `signIn()` once implemented. To reach `/` before then, run
`sessionStorage.setItem('forgeflow.authenticated', 'true')` in the browser console.

## Prerequisites

- Node.js 22.18+ or 24.12+ (npm 11+)
- .NET SDK 10
- SQL Server LocalDB (`(localdb)\MSSQLLocalDB`) with a `ForgeFlow` database

## Backend

```bash
cd backend/ForgeFlow.Api
dotnet run --launch-profile http
```

Use `--launch-profile https` for the HTTPS endpoint the frontend talks to.

- API endpoints: `https://localhost:7243/api/...` (http: `http://localhost:5213/api/...`)
- Swagger UI: https://localhost:7243/swagger (`/` redirects here in Development)
- OpenAPI document: https://localhost:7243/openapi/v1.json
- Health (includes a SQL Server connectivity check): https://localhost:7243/health
- `GET /api/auth/token` — **Development only.** Triggers the two-legged Autodesk token request
  and returns the token plus its expiry. Returns 404 outside Development. Exists to inspect the
  flow while learning; the browser must never receive this token (see the note below).

Controllers inherit `ApiControllerBase`, which applies `[ApiController]` and `[Route("api/[controller]")]`,
so every endpoint lands under `/api`. URLs are generated lowercase.
`/health` and `/swagger` sit outside the `/api` prefix.

Configuration:

- `ConnectionStrings:ForgeFlowDb` — required; the app fails at startup if it is empty.
  The local development value lives in `appsettings.Development.json`.
  Outside development, supply it through environment variables or user secrets.
- `Cors:AllowedOrigins` — origins allowed to call the API (development: `http://localhost:5173`).
- `Autodesk:ClientId` / `Autodesk:ClientSecret` — Autodesk Platform Services (Forge) app
  credentials, bound to `AutodeskOptions` and injected as `IOptions<AutodeskOptions>`.
  Empty in `appsettings.json`; **never commit real values**. Set them locally with user secrets:

```bash
cd backend/ForgeFlow.Api
dotnet user-secrets set "Autodesk:ClientId" "<your client id>"
dotnet user-secrets set "Autodesk:ClientSecret" "<your client secret>"
```

  In other environments use environment variables: `Autodesk__ClientId`, `Autodesk__ClientSecret`.
- `Autodesk:BaseUrl` / `Autodesk:Scopes` — APS host and the space-separated scopes requested
  for two-legged tokens (default `data:read`).

`IAutodeskTokenProvider` ([Services/AutodeskTokenProvider.cs](backend/ForgeFlow.Api/Services/AutodeskTokenProvider.cs))
performs the client-credentials exchange against `authentication/v2/token`, sending
`Basic base64(clientId:clientSecret)`. It is registered as a singleton and caches the token
in memory until 60s before expiry, so injecting it and calling `GetAccessTokenAsync()` is cheap.

Create the database and apply the Identity schema:

```bash
cd backend/ForgeFlow.Api
dotnet ef database update
```

`dotnet ef` creates the database if it does not exist.

### Seeded users (Development only)

On startup in Development, [IdentitySeeder](backend/ForgeFlow.Api/Data/Seeding/IdentitySeeder.cs)
creates the users listed under `IdentitySeed:Users` in `appsettings.Development.json`, along with
their roles. Existing users are skipped, so restarts never rewrite a password.

Passwords are **not committed**. Leave `Password` out and the seeder generates a unique 16-character
one per user and logs it once at startup — that is the only time it is visible, since only the hash
is stored. Grab it from the console:

```
warn: Seeded admin@forgeflow.local with generated password: <generated>
```

To pin a password instead, set it through user secrets rather than the JSON file:

```bash
dotnet user-secrets set "IdentitySeed:Users:0:Password" "<your password>"
```

Passwords must satisfy the default Identity policy (8+ chars here, upper, lower, digit, symbol).

`InitialIdentity` (in `Data/Migrations`) creates the ASP.NET Core Identity tables.
There are no business entities yet.

## Frontend

```bash
cd frontend
npm install
npm run dev
```

- App: http://localhost:5173

Configuration: copy `.env.example` to `.env.local` to override `VITE_API_BASE_URL`.
The committed `.env.development` points at `https://localhost:7243/api`, so Axios calls
are written prefix-free — `httpClient.get('/projects')` hits `https://localhost:7243/api/projects`.
Run the backend on the `https` profile, and trust the dev certificate once with
`dotnet dev-certs https --trust`, or the browser blocks the requests.

Scripts: `npm run dev`, `npm run build`, `npm run preview`, `npm run lint`, `npm run format`.

### Theme

Single dark theme, `forgeflowDark`, in [src/plugins/vuetify.js](frontend/src/plugins/vuetify.js):
`#111827` background, `#1F2937` surface, `#22C55E` primary, `#F5F5F5` text, `#374151` borders.
Global `defaults` give every card a bordered surface look and every button flat-case labels.

Use theme tokens (`color="surface"`, `color="primary"`, `text-medium-emphasis`) instead of
hard-coded hex, so a light theme later stays a one-file change.
