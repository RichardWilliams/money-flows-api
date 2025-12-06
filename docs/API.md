# API Documentation

This document provides an overview of the API endpoints. For interactive documentation, visit `/swagger` when the API is running.

## Base URL

- Development: `http://localhost:5000`
- Production: TBD

## Authentication

Authentication will be added in Phase 7. Currently, all endpoints are publicly accessible.

## Health Checks

### GET /health

Overall health status of the application.

**Response:**
```json
{
  "status": "Healthy",
  "results": {
    "database": {
      "status": "Healthy",
      "description": "PostgreSQL connection successful"
    },
    "api": {
      "status": "Healthy",
      "description": "API is running"
    }
  }
}
```

### GET /health/ready

Readiness probe - indicates if the application is ready to serve traffic.

**Use case:** Kubernetes readiness probe

### GET /health/live

Liveness probe - indicates if the application is alive and should not be restarted.

**Use case:** Kubernetes liveness probe

## Features

The following feature endpoints will be implemented in subsequent phases:

### Phase 2: MoneyFlows (Core Financial Transactions)

#### POST /api/moneyflows
Create a new money flow transaction with optional file attachment.

**Request:**
```
Content-Type: multipart/form-data

propertyId: 1
moneyFlowTypeId: 1
participantId: 1
amount: 450.00
currency: GBP
transactionDate: 2025-01-15T00:00:00Z
description: Rental income from Airbnb
attachment: [file]
```

**Response:** `201 Created`
```json
{
  "id": 123,
  "propertyId": 1,
  "propertyName": "Cottage Lane",
  "moneyFlowTypeId": 1,
  "moneyFlowTypeName": "Rental Income",
  "participantId": 1,
  "participantName": "Airbnb",
  "amount": 450.00,
  "currency": "GBP",
  "transactionDate": "2025-01-15T00:00:00Z",
  "attachmentPath": "/attachments/2025/01/cottage-lane/airbnb-rental-income-15-1.pdf",
  "hasAttachment": true,
  "description": "Rental income from Airbnb",
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-01-15T10:30:00Z"
}
```

#### GET /api/moneyflows
Get paginated list of money flows with optional filtering.

**Query Parameters:**
- `propertyId` (optional): Filter by property
- `participantId` (optional): Filter by participant
- `moneyFlowTypeId` (optional): Filter by type
- `fromDate` (optional): Start date filter
- `toDate` (optional): End date filter
- `minAmount` (optional): Minimum amount filter
- `maxAmount` (optional): Maximum amount filter
- `page` (default: 1): Page number
- `pageSize` (default: 20): Items per page

**Response:** `200 OK`
```json
{
  "items": [
    {
      "id": 123,
      "propertyName": "Cottage Lane",
      "participantName": "Airbnb",
      "typeName": "Rental Income",
      "amount": 450.00,
      "currency": "GBP",
      "transactionDate": "2025-01-15T00:00:00Z",
      "hasAttachment": true
    }
  ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 150,
  "totalPages": 8,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

#### GET /api/moneyflows/{id}
Get detailed information about a specific money flow.

**Response:** `200 OK`
```json
{
  "id": 123,
  "propertyId": 1,
  "propertyName": "Cottage Lane",
  "moneyFlowTypeId": 1,
  "moneyFlowTypeName": "Rental Income",
  "participantId": 1,
  "participantName": "Airbnb",
  "amount": 450.00,
  "currency": "GBP",
  "transactionDate": "2025-01-15T00:00:00Z",
  "attachmentPath": "/attachments/2025/01/cottage-lane/airbnb-rental-income-15-1.pdf",
  "hasAttachment": true,
  "description": "Rental income from Airbnb",
  "createdAt": "2025-01-15T10:30:00Z",
  "updatedAt": "2025-01-15T10:30:00Z"
}
```

#### PUT /api/moneyflows/{id}
Update an existing money flow.

**Request:** Same as POST (multipart/form-data)

**Response:** `200 OK`

#### DELETE /api/moneyflows/{id}
Delete a money flow.

**Response:** `204 No Content`

### Phase 3: Properties & Participants

#### Properties Endpoints
- `GET /api/properties` - List all properties
- `GET /api/properties/{id}` - Get property details
- `POST /api/properties` - Create new property
- `PUT /api/properties/{id}` - Update property
- `DELETE /api/properties/{id}` - Delete property

#### Participants Endpoints
- `GET /api/participants` - List all participants
- `GET /api/participants/{id}` - Get participant details
- `POST /api/participants` - Create new participant
- `PUT /api/participants/{id}` - Update participant
- `DELETE /api/participants/{id}` - Delete participant

#### Participant Types Endpoints
- `GET /api/participant-types` - List all participant types
- `GET /api/participant-types/{id}` - Get type details
- `POST /api/participant-types` - Create new type
- `PUT /api/participant-types/{id}` - Update type
- `DELETE /api/participant-types/{id}` - Delete type

### Phase 4: Reports & Exports

#### GET /api/reports/summary
Generate financial summary report.

**Query Parameters:**
- `propertyId` (optional): Filter by property
- `fromDate` (required): Start date
- `toDate` (required): End date

**Response:** `200 OK`
```json
{
  "period": {
    "from": "2025-01-01T00:00:00Z",
    "to": "2025-03-31T00:00:00Z"
  },
  "properties": [
    {
      "propertyId": 1,
      "propertyName": "Cottage Lane",
      "totalIncome": 5400.00,
      "totalExpenses": 2100.00,
      "netProfit": 3300.00,
      "currency": "GBP"
    }
  ],
  "overall": {
    "totalIncome": 15600.00,
    "totalExpenses": 6200.00,
    "netProfit": 9400.00,
    "currency": "GBP"
  }
}
```

#### GET /api/reports/export
Export data in various formats for accountants.

**Query Parameters:**
- `format` (required): csv | excel | pdf
- `propertyId` (optional): Filter by property
- `fromDate` (required): Start date
- `toDate` (required): End date

**Response:** File download

## Error Responses

### 400 Bad Request
Validation error or invalid request.

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Validation failed",
  "status": 400,
  "errors": {
    "Amount": ["Amount must be positive or zero"],
    "PropertyId": ["Property is required"]
  }
}
```

### 404 Not Found
Resource not found.

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Resource not found",
  "status": 404,
  "detail": "Money flow with ID 123 not found"
}
```

### 500 Internal Server Error
Unexpected server error.

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.6.1",
  "title": "An error occurred",
  "status": 500,
  "detail": "Internal server error"
}
```

## Pagination

All list endpoints support pagination with the following query parameters:

- `page` (default: 1): Page number (1-based)
- `pageSize` (default: 20, max: 100): Items per page

Paginated responses include:

```json
{
  "items": [...],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 150,
  "totalPages": 8,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

## Filtering

Most list endpoints support filtering. Available filters depend on the resource:

### MoneyFlows Filters
- `propertyId`: Filter by property ID
- `participantId`: Filter by participant ID
- `moneyFlowTypeId`: Filter by transaction type ID
- `fromDate`: Transactions on or after this date
- `toDate`: Transactions on or before this date
- `minAmount`: Minimum transaction amount
- `maxAmount`: Maximum transaction amount

### Date Filters
All date filters accept ISO 8601 format:
- `2025-01-15` (date only)
- `2025-01-15T00:00:00Z` (date and time with timezone)

## Sorting

Sorting will be added in a future phase. Currently, results are sorted by:
- MoneyFlows: `transactionDate DESC` (newest first)
- Properties: `name ASC` (alphabetical)
- Participants: `name ASC` (alphabetical)

## Rate Limiting

Rate limiting will be added in a future phase.

## Versioning

This API does NOT use URL versioning (no `/v1` prefix). Instead, we maintain backward compatibility:

- Additive changes (new fields) are non-breaking
- Deprecated fields include a warning header
- Breaking changes require 90-day sunset period

Example deprecation warning header:
```
X-Deprecated-Warning: The 'oldField' field is deprecated and will be removed on 2025-12-31. Use 'newField' instead.
```

## CORS

CORS is configured to allow requests from:
- Development: `http://localhost:5173`, `http://localhost:3000`
- Production: (to be configured)

## File Uploads

File uploads use `multipart/form-data` encoding.

**Restrictions:**
- Maximum file size: 10 MB
- Allowed extensions: `.pdf`, `.jpg`, `.jpeg`, `.png`, `.doc`, `.docx`, `.xls`, `.xlsx`

**File Organization:**
Files are stored using a date-first hierarchy for easy reporting:

```
/attachments/
  {year}/                    # 2025
    {month}/                 # 01, 02, ..., 12
      {property}/            # cottage-lane, seaside-retreat
        {participant}-{type}-{day}-{count}.{ext}
```

Example: `/attachments/2025/01/cottage-lane/airbnb-rental-income-15-1.pdf`

## Webhooks

Webhooks will be added in Phase 8 for platform integrations.

## SDK / Client Libraries

Client libraries will be generated from OpenAPI specification in future phases.

## Support

For API support:
- Check Swagger UI at `/swagger`
- Review error messages in response
- Check application logs (via Serilog)
- Contact support team

## Changelog

See [CHANGELOG.md](../CHANGELOG.md) for version history and breaking changes.
