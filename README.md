# ForgeFlow

Learning project for Autodesk authentication, ACC model sync, model versioning and Design Automation.
This repo currently contains only the scaffold: a Vue 3 frontend and an ASP.NET Core Web API backend on SQL Server.

## Layout

| Path | What |
| --- | --- |
| `frontend/` | Vue 3 + Vite + Vuetify + Vue Router + Pinia + Axios |
| `backend/` | .NET 10 Web API + EF Core (SQL Server) + OpenAPI/Swagger UI |

## Prerequisites

- Node.js 22.18+ or 24.12+ (npm 11+)
- .NET SDK 10
- SQL Server (local default instance) with a `ForgeFlow` database

## Backend

```bash
cd backend/ForgeFlow.Api
dotnet run --launch-profile http
```

- API: http://localhost:5213
- Swagger UI: http://localhost:5213/swagger
- OpenAPI document: http://localhost:5213/openapi/v1.json
- Health (includes a SQL Server connectivity check): http://localhost:5213/health

Configuration:

- `ConnectionStrings:ForgeFlowDb` — required; the app fails at startup if it is empty.
  The local development value lives in `appsettings.Development.json`.
  Outside development, supply it through environment variables or user secrets.
- `Cors:AllowedOrigins` — origins allowed to call the API (development: `http://localhost:5173`).

Create the database once:

```sql
CREATE DATABASE [ForgeFlow];
```

There are no entities or migrations yet.

## Frontend

```bash
cd frontend
npm install
npm run dev
```

- App: http://localhost:5173

Configuration: copy `.env.example` to `.env.local` to override `VITE_API_BASE_URL`
(the committed `.env.development` already points at the local API).

Scripts: `npm run dev`, `npm run build`, `npm run preview`, `npm run lint`, `npm run format`.
