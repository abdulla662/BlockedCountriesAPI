# BlockedCountriesAPI

A .NET Core Web API that detects blocked countries based on IP address using ipgeolocation.io.  
Implements in-memory storage, temporary block with auto-expiry, and logs all access attempts.

## Endpoints
- `POST /api/countries/block`
- `DELETE /api/countries/block/{code}`
- `GET /api/countries/blocked`
- `POST /api/countries/temporal-block`
- `GET /api/ip/lookup`
- `GET /api/ip/check-block`
- `GET /api/logs/blocked-attempts`

## Technologies Used
- .NET 9 Web API
- Swagger
- HttpClient
- BackgroundService
