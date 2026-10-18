# RA_MuniAnnouncements

A .NET 10 ASP.NET Core Web API and Angular resident-announcements client. The API is structured as a small, independently deployable service; CMS and site-settings access are represented by replaceable mock implementations.

## Architecture

```text
Angular resident widget
	| GET /api/announcements/{authorityCode}
AnnouncementsController
	-> AnnouncementService
	   -> ICmsClient / MockCmsClient
	   -> ISiteSettingsRepository / MockSiteSettingsRepository
	   -> IAnnouncementNormalizer / AnnouncementNormalizer
	-> AnnouncementsResponse
```

- `Controllers` handles HTTP validation and ProblemDetails responses.
- `Services` coordinates parallel data retrieval, enforces the tenant boundary, and normalizes announcements.
- `Clients` and `Repositories` isolate external data sources behind interfaces; the current implementations use hard-coded data.
- `Models` contains CMS, settings, and response contracts. `Options` is reserved for externalized configuration.
- The Angular standalone component is reusable through its required `authorityCode` input; the demo root supplies `6500`.

## Requirements and assumptions

- Unknown authorities are represented by missing site settings and return HTTP 404.
- CMS records are filtered again by `authorityCode` before normalization, even if the CMS client should already scope its response.
- A notice is active when it is published, `publishDate <= now`, and its optional `expiryDate > now`.
- Category matching is case-insensitive. Missing display names fall back to the category key.
- Dates are displayed in Asia/Jerusalem time. HTML is reduced to plain text on the API; the client renders summaries with interpolation.
- `maxItems <= 0` uses the normalizer default of 10. Production data access, secrets, authentication, caching, and observability are intentionally outside this mock assignment.

## Prerequisites

- .NET 10 SDK
- Node.js 24 and npm 11 (or compatible versions supported by the installed Angular CLI)

## Run the API

From the repository root:

```powershell
dotnet restore RA_MuniAnnouncements.sln
dotnet run --project src/RA_MuniAnnouncements.Api/RA_MuniAnnouncements.Api.csproj --launch-profile http
```

The API listens at `http://localhost:5152`. Swagger UI is available at `/swagger`; health is available at `/health`.

## Run the Angular client

In a second terminal, from the repository root:

```powershell
npm.cmd --prefix client ci
npm.cmd --prefix client start
```

Open `http://localhost:4200`. The Angular dev-server proxy forwards `/api` to `http://localhost:5152`. CORS allows the Angular development origin only.

## Build and test

```powershell
dotnet build RA_MuniAnnouncements.sln
dotnet test RA_MuniAnnouncements.sln
npm.cmd --prefix client run build
npm.cmd --prefix client run test -- --watch=false --isolate=false
```

The xUnit suite covers normalization and the service tenant guard. The Angular suite currently contains one root-component test; `--isolate=false` avoids a Vitest worker-startup timeout in some local environments.
