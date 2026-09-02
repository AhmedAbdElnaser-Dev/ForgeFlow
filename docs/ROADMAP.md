# ForgeFlow — Feature Checklist

Tracks what is built and what each remaining feature needs. Scopes are the Autodesk
values that must appear in `Autodesk:Scopes` before the call will be accepted.

Legend: `[x]` done · `[ ]` not started · **2L** two-legged · **3L** three-legged

---

## Foundation

- [x] Vue 3 + Vite + Vuetify + Vue Router + Pinia + Axios
- [x] ASP.NET Core Web API (.NET 10) with `/api` route prefix
- [x] EF Core + SQL Server (LocalDB), connection string from configuration
- [x] ASP.NET Core Identity, `InitialIdentity` migration applied
- [x] Seeded development users with roles (`Admin`, `Engineer`, `Viewer`)
- [x] Cookie authentication — `POST /api/auth/login`, `POST /api/auth/logout`, `GET /api/auth/me`
- [x] Login page: email + password, plus placeholder Autodesk button
- [x] Route guard, session restore on page load
- [x] Sidebar navigation, profile page
- [x] Swagger / OpenAPI, health check with database probe
- [x] Mapperly for model → DTO mapping

## Autodesk authentication

- [x] **2L** Client credentials token service, cached until 60s before expiry
- [x] **2L** Credentials in configuration, real values in user secrets
- [x] **2L** Development endpoint to inspect the token
- [ ] **2L** Scope-aware tokens — one cache per scope, so `viewables:read` and `code:all` coexist
  - [x] `AutodeskScope` flags enum with wire-format conversion
  - [x] Token service requests scopes as flags, parsed from configuration
  - [ ] Cache and lock per scope, and a scope argument on the service
  - Blocks every feature below that needs a scope other than `data:read`
- [ ] **2L** Short-lived viewer token endpoint for the browser (`viewables:read`, ~10 min, behind Identity login)
- [ ] **3L** Authorization-code login — makes the "Login with Autodesk" button real
  - Needs a callback URL registered on the APS app
- [ ] **3L** Per-user token storage and refresh (`AspNetUserTokens`)
- [ ] Link an Autodesk identity to a ForgeFlow user (`AspNetUserLogins`)

## Model pipeline — no user required

- [ ] **2L** Create and list OSS buckets — `bucket:create bucket:read`
- [ ] **2L** Upload a model to OSS (signed S3 upload) — `data:write data:create`
- [ ] **2L** Download / signed URLs — `data:read`
- [ ] **2L** Translate to viewable via Model Derivative — `data:write data:read`
- [ ] **2L** Poll or webhook for translation status
- [ ] **2L** Read manifest, metadata, element properties — `data:read`
- [ ] **2L** Persist extracted properties to SQL Server
- [ ] **2L** Model thumbnails

## Viewer

- [ ] Load APS Viewer in a Vue component
- [ ] Fetch the viewer token from the API (never the full app token)
- [ ] Select an element, show its properties
- [ ] Handle loading, empty and failed-translation states

## Design Automation

- [ ] **2L** Register AppBundle and Activity — `code:all`
- [ ] **2L** Submit a WorkItem against an uploaded model
- [ ] **2L** Receive the completion webhook
- [ ] **2L** Store the result file back in OSS
- [ ] Surface job status in the UI (queued / running / succeeded / failed)

## ACC integration

- [ ] **3L** List the signed-in user's hubs and projects — `data:read`
- [ ] **3L** Browse folders, items and versions
- [ ] **3L** Download a model from ACC into the pipeline
- [ ] **3L** Create a new version of a file in ACC — `data:write data:create`
- [ ] Sync ACC model versions into SQL Server

### Alternative: ACC without a signed-in user

Either route lets a **2L** token reach ACC project data, and both need an ACC account
admin to act — they are not code-only changes.

- [ ] Custom Integration — account admin authorises the app's client id on the account
- [ ] Service Account (SSA) — a machine identity granted ACC access
- [ ] Scheduled background sync, once one of the above is in place

## Hardening — before this leaves a laptop

- [ ] Rotate the Autodesk client secret
- [ ] Move the connection string out of `appsettings.Development.json` for deployed environments
- [ ] Persist Data Protection keys to shared storage (otherwise every restart or new instance signs users out)
- [ ] Remove or lock down the development token endpoint
- [ ] Structured logging and error handling for Autodesk failures
- [ ] Tests: token caching, seeding, login flow
