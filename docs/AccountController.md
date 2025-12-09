# Account Controller API Documentation

## Overview

The AccountController provides REST API endpoints for managing Account entities in Microsoft Dataverse. This controller follows the same patterns as the OrdersController and provides full CRUD operations for accounts.

## Base URL

```
/api/Account
```

## Endpoints

### 1. Get All Accounts

**GET** `/api/Account`

Retrieves all accounts from Dataverse.

**Response:**
- `200 OK`: Returns a list of all accounts
- `500 Internal Server Error`: An error occurred while retrieving accounts

**Example Response:**
```json
[
  {
    "id": "b31a41ee-483d-f011-877a-7c1e523c1abb",
    "name": "Contoso Ltd",
    "accountNumber": "ACC-001",
    "accountRatingCode": 1,
    "emailAddress1": "contact@contoso.com",
    "telephone1": "555-1234",
    "address1_Line1": "123 Main St",
    "address1_City": "Seattle",
    "address1_StateOrProvince": "WA",
    "address1_PostalCode": "98101",
    "address1_Country": "USA",
    "websiteUrl": "https://contoso.com",
    "numberOfEmployees": 250,
    "revenue": 5000000.00,
    "description": "Leading software company",
    "statusCode": 1,
    "stateCode": 0,
    "createdOn": "2024-01-15T10:30:00Z",
    "modifiedOn": "2024-01-15T10:30:00Z"
  }
]
```

### 2. Get Account by ID

**GET** `/api/Account/{id}`

Retrieves a specific account by its ID.

**Parameters:**
- `id` (GUID, required): The unique identifier of the account

**Response:**
- `200 OK`: Returns the account
- `404 Not Found`: Account not found
- `500 Internal Server Error`: An error occurred

**Example Request:**
```
GET /api/Account/b31a41ee-483d-f011-877a-7c1e523c1abb
```

### 3. Create Account

**POST** `/api/Account`

Creates a new account in Dataverse.

**Request Body:**
```json
{
  "name": "Adventure Works",
  "accountNumber": "ACC-002",
  "accountRatingCode": 1,
  "emailAddress1": "info@adventureworks.com",
  "telephone1": "555-5678",
  "address1_Line1": "456 Oak Ave",
  "address1_City": "Portland",
  "address1_StateOrProvince": "OR",
  "address1_PostalCode": "97201",
  "address1_Country": "USA",
  "websiteUrl": "https://adventureworks.com",
  "numberOfEmployees": 150,
  "revenue": 3500000.00,
  "description": "Outdoor equipment retailer"
}
```

**Response:**
- `201 Created`: Returns the newly created account with its ID
- `400 Bad Request`: Invalid request data
- `500 Internal Server Error`: An error occurred

### 4. Update Account

**PUT** `/api/Account/{id}`

Updates an existing account. Only provided fields will be updated.

**Parameters:**
- `id` (GUID, required): The unique identifier of the account

**Request Body:**
```json
{
  "name": "Adventure Works Inc",
  "emailAddress1": "contact@adventureworks.com",
  "revenue": 4000000.00
}
```

**Response:**
- `200 OK`: Returns the updated account
- `400 Bad Request`: Invalid request data
- `404 Not Found`: Account not found
- `500 Internal Server Error`: An error occurred

### 5. Delete Account

**DELETE** `/api/Account/{id}`

Deletes an account from Dataverse.

**Parameters:**
- `id` (GUID, required): The unique identifier of the account

**Response:**
- `204 No Content`: Account deleted successfully
- `404 Not Found`: Account not found
- `500 Internal Server Error`: An error occurred

**Example Request:**
```
DELETE /api/Account/b31a41ee-483d-f011-877a-7c1e523c1abb
```

### 6. Get Accounts by Name

**GET** `/api/Account/name/{name}`

Searches for accounts by name (partial match supported).

**Parameters:**
- `name` (string, required): The account name to search for

**Response:**
- `200 OK`: Returns a list of matching accounts
- `400 Bad Request`: Name is empty
- `500 Internal Server Error`: An error occurred

**Example Request:**
```
GET /api/Account/name/Contoso
```

## Data Model

### Account Properties

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| id | GUID | Auto-generated | Unique identifier |
| name | string | Yes | Account name |
| accountNumber | string | No | Account reference number |
| accountRatingCode | int | No | Account rating (default: 1) |
| emailAddress1 | string | No | Primary email address |
| telephone1 | string | No | Primary telephone number |
| address1_Line1 | string | No | Street address line 1 |
| address1_City | string | No | City |
| address1_StateOrProvince | string | No | State or province |
| address1_PostalCode | string | No | Postal/ZIP code |
| address1_Country | string | No | Country |
| websiteUrl | string | No | Company website URL |
| numberOfEmployees | int? | No | Number of employees |
| revenue | decimal? | No | Annual revenue |
| description | string | No | Account description |
| statusCode | int | No | Status code (1=Active, 2=Inactive) |
| stateCode | int | No | State code (0=Active, 1=Inactive) |
| createdOn | DateTime | Auto-generated | Creation timestamp |
| modifiedOn | DateTime | Auto-generated | Last modification timestamp |

## Dataverse Integration

### Entity Name
The controller interacts with the standard `account` entity in Microsoft Dataverse.

### Field Mapping

The following Dataverse fields are mapped:

- `accountid` → Account.Id
- `name` → Account.Name
- `accountnumber` → Account.AccountNumber
- `accountratingcode` → Account.AccountRatingCode
- `emailaddress1` → Account.EmailAddress1
- `telephone1` → Account.Telephone1
- `address1_line1` → Account.Address1_Line1
- `address1_city` → Account.Address1_City
- `address1_stateorprovince` → Account.Address1_StateOrProvince
- `address1_postalcode` → Account.Address1_PostalCode
- `address1_country` → Account.Address1_Country
- `websiteurl` → Account.WebsiteUrl
- `numberofemployees` → Account.NumberOfEmployees
- `revenue` → Account.Revenue (Money type in Dataverse)
- `description` → Account.Description
- `statuscode` → Account.StatusCode
- `statecode` → Account.StateCode

## Error Handling

All endpoints follow consistent error handling patterns:

1. **Validation Errors (400)**: Returned when request data is invalid
2. **Not Found Errors (404)**: Returned when the specified account doesn't exist
3. **Server Errors (500)**: Returned when an unexpected error occurs during processing

Error responses include descriptive messages to help diagnose issues.

## Testing

The Account API includes comprehensive unit tests:

### AccountServiceTests
- `GetAllAccountsAsync_ReturnsAllAccounts`
- `GetAccountByIdAsync_ExistingAccount_ReturnsAccount`
- `GetAccountByIdAsync_NonExistingAccount_ReturnsNull`
- `CreateAccountAsync_ValidAccount_ReturnsCreatedAccount`
- `UpdateAccountAsync_ExistingAccount_ReturnsUpdatedAccount`
- `UpdateAccountAsync_NonExistingAccount_ReturnsNull`
- `DeleteAccountAsync_ExistingAccount_ReturnsTrue`
- `DeleteAccountAsync_NonExistingAccount_ReturnsFalse`
- `GetAccountsByNameAsync_ReturnsMatchingAccounts`

### AccountRepositoryTests
- `GetAllAsync_ReturnsAllAccounts`
- `GetByIdAsync_ExistingAccount_ReturnsAccount`
- `GetByIdAsync_NonExistingAccount_ReturnsNull`
- `CreateAsync_ValidAccount_ReturnsCreatedAccount`
- `UpdateAsync_ValidAccount_ReturnsUpdatedAccount`
- `DeleteAsync_ExistingAccount_ReturnsTrue`
- `DeleteAsync_NonExistingAccount_ReturnsFalse`
- `GetByNameAsync_ReturnsMatchingAccounts`

Run tests with:
```bash
dotnet test
```

## Architecture

The Account API follows a layered architecture:

1. **Controller Layer** (`AccountController`): Handles HTTP requests/responses
2. **Service Layer** (`AccountService`): Implements business logic
3. **Repository Layer** (`AccountRepository`): Manages Dataverse data access
4. **Model Layer** (`Account`): Defines the domain model
5. **DTO Layer** (`AccountDto`, `CreateAccountDto`, `UpdateAccountDto`): Data transfer objects

## Dependencies

The Account functionality requires:

- Microsoft.Xrm.Sdk (for Dataverse integration)
- Microsoft.AspNetCore.Mvc (for REST API)
- Microsoft.Extensions.Logging (for logging)

## Security Considerations

1. **Input Sanitization**: All LIKE query inputs are sanitized to prevent injection attacks
2. **Null Checking**: All nullable parameters are validated before use
3. **Exception Handling**: Comprehensive error handling prevents information leakage
4. **Logging**: All operations are logged for audit purposes

## Examples

### Creating an Account via cURL

```bash
curl -X POST "https://localhost:5001/api/Account" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Fabrikam Inc",
    "accountNumber": "FAB-001",
    "emailAddress1": "info@fabrikam.com",
    "telephone1": "555-9999",
    "address1_Line1": "789 Pine St",
    "address1_City": "San Francisco",
    "address1_StateOrProvince": "CA",
    "address1_PostalCode": "94102",
    "address1_Country": "USA",
    "accountRatingCode": 1
  }'
```

### Updating an Account via cURL

```bash
curl -X PUT "https://localhost:5001/api/Account/b31a41ee-483d-f011-877a-7c1e523c1abb" \
  -H "Content-Type: application/json" \
  -d '{
    "revenue": 6000000.00,
    "numberOfEmployees": 300
  }'
```

### Searching Accounts by Name via cURL

```bash
curl -X GET "https://localhost:5001/api/Account/name/Contoso"
```
