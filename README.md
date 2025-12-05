# Order Management API

A .NET 8 MVC Web API for Order Management with Microsoft Dataverse integration.

## Features

- RESTful API endpoints for order management (CRUD operations)
- Microsoft Dataverse integration for data storage
- Clean architecture with Repository and Service patterns
- Swagger/OpenAPI documentation

## Project Structure

```
├── src/
│   └── OrderManagement/
│       ├── Controllers/        # API Controllers
│       ├── Data/              # Dataverse connection management
│       ├── DTOs/              # Data Transfer Objects
│       ├── Models/            # Domain models
│       ├── Repositories/      # Data access layer
│       └── Services/          # Business logic layer
└── tests/
    └── OrderManagement.Tests/ # Unit tests
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orders` | Get all orders |
| GET | `/api/orders/{id}` | Get order by ID |
| POST | `/api/orders` | Create new order |
| PUT | `/api/orders/{id}` | Update existing order |
| DELETE | `/api/orders/{id}` | Delete order |
| GET | `/api/orders/status/{status}` | Get orders by status |
| GET | `/api/orders/customer/{customerName}` | Get orders by customer name |

## Order Status Values

- 0: Pending
- 1: Confirmed
- 2: Processing
- 3: Shipped
- 4: Delivered
- 5: Cancelled

## Configuration

Configure Dataverse connection in `appsettings.json`:

```json
{
  "Dataverse": {
    "Url": "https://your-org.crm.dynamics.com",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "TenantId": "your-tenant-id"
  }
}
```

### Azure AD App Registration

1. Register an application in Azure AD
2. Grant API permissions for Dynamics CRM
3. Create a client secret
4. Use the Application (Client) ID, Secret, and Tenant ID in configuration

## Dataverse Entity Setup

Create a custom entity `cr_order` in Dataverse with the following attributes:

| Attribute | Type | Description |
|-----------|------|-------------|
| cr_orderid | Unique Identifier | Primary key |
| cr_ordernumber | Text | Order reference number |
| cr_customername | Text | Customer name |
| cr_customeremail | Text | Customer email |
| cr_totalamount | Currency | Total order amount |
| cr_status | Option Set | Order status |
| cr_shippingaddress | Text | Shipping address |
| cr_description | Text | Order description |

## Building and Running

### Prerequisites

- .NET 8.0 SDK
- Azure AD application with Dataverse permissions
- Microsoft Dataverse environment

### Build

```bash
dotnet build
```

### Run

```bash
cd src/OrderManagement
dotnet run
```

### Run Tests

```bash
dotnet test
```

## API Documentation

When running in Development mode, Swagger UI is available at:
- `/swagger`

## License

See [LICENSE](LICENSE) file for details.