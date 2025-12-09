# Order Management API Documentation

This folder contains documentation for the Order Management API.

## Available Documentation

### API Controllers

- **[AccountController.md](AccountController.md)** - Complete API documentation for the Account management endpoints
  - All REST endpoints (GET, POST, PUT, DELETE)
  - Request/Response examples
  - Dataverse field mappings
  - Error handling
  - Testing information

### Implementation Details

- **[IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)** - Detailed summary of the AccountController implementation
  - Architecture overview
  - Design patterns used
  - Test coverage
  - Security information
  - Files added/modified

## Quick Reference

### AccountController Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/Account | Get all accounts |
| GET | /api/Account/{id} | Get account by ID |
| POST | /api/Account | Create new account |
| PUT | /api/Account/{id} | Update account |
| DELETE | /api/Account/{id} | Delete account |
| GET | /api/Account/name/{name} | Search accounts by name |

## Getting Started

1. Ensure you have configured Dataverse connection in `appsettings.json`:
   ```json
   {
     "Dataverse": {
       "Url": "https://yourorg.crm.dynamics.com",
       "ClientId": "your-client-id",
       "ClientSecret": "your-client-secret",
       "TenantId": "your-tenant-id"
     }
   }
   ```

2. Run the application:
   ```bash
   cd src/OrderManagement
   dotnet run
   ```

3. Access Swagger UI at: `https://localhost:5001/swagger`

4. Test the API endpoints using the examples in AccountController.md

## Testing

Run all tests:
```bash
dotnet test
```

Run only Account tests:
```bash
dotnet test --filter "FullyQualifiedName~Account"
```

## Support

For questions or issues, please refer to the detailed documentation in the individual markdown files.
