# StockFlow API

StockFlow is a REST API for inventory and sales management built with ASP.NET Core, SQL Server, JWT authentication, Docker and automated tests.

## Main Features

- Products and categories
- Customers
- Sales
- Inventory movements
- Automatic stock updates
- JWT authentication
- Role-based authorization
- Swagger / OpenAPI
- Unit and integration tests
- Docker support
- Health checks

## Architecture

The solution follows a simplified Clean Architecture:

```text
StockFlow
├── src
│   ├── StockFlow.Api
│   ├── StockFlow.Application
│   ├── StockFlow.Domain
│   └── StockFlow.Infrastructure
├── tests
│   ├── StockFlow.UnitTests
│   └── StockFlow.IntegrationTests
├── Dockerfile
├── docker-compose.yml
├── README.md
└── StockFlow.sln
```

### Layers

**Domain**
- Entities
- Enums
- Core business models

**Application**
- DTOs
- Interfaces
- Services
- Validators
- Business logic

**Infrastructure**
- Entity Framework Core
- SQL Server
- Repositories
- JWT generation
- Password hashing
- Database seed

**API**
- Controllers
- Authentication
- Authorization
- Swagger
- Middleware
- Health checks

## Technologies

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core 9
- SQL Server
- JWT
- BCrypt
- FluentValidation
- xUnit
- Moq
- Docker
- Swagger

## Run with Docker

Docker is the recommended way to run StockFlow locally.

### 1. Create the environment file

Copy:

```text
.env.example
```

to:

```text
.env
```

Example:

```env
SQL_SA_PASSWORD=YourStrongPassword123*
JWT_KEY=Your-Super-Secret-JWT-Key
```

### 2. Start the application

```bash
docker compose up -d --build
```

### 3. Open Swagger

```text
http://localhost:8080/swagger
```

### 4. Health Check

```text
http://localhost:8080/health
```

### 5. Stop the application

```bash
docker compose down
```

## Development Users

The database seed creates these users for local development:

| Role | Email | Password |
|---|---|---|
| Admin | admin@stockflow.com | Admin123* |
| Manager | manager@stockflow.com | Manager123* |
| Seller | seller@stockflow.com | Seller123* |

## Authentication

Login:

```http
POST /api/auth/login
```

Example:

```json
{
  "email": "admin@stockflow.com",
  "password": "Admin123*"
}
```

Use the returned token as:

```text
Authorization: Bearer <token>
```

Swagger also includes an **Authorize** button for JWT authentication.

## Main Endpoints

```text
POST /api/auth/login

GET  /api/products
POST /api/products
PUT  /api/products/{id}

GET  /api/categories
POST /api/categories
PUT  /api/categories/{id}

GET  /api/customers
POST /api/customers
PUT  /api/customers/{id}

GET  /api/sales
GET  /api/sales/{id}
POST /api/sales

GET  /api/inventory/movements
GET  /api/inventory/movements/product/{productId}
POST /api/inventory/entry
POST /api/inventory/adjustment
```

## Sales and Inventory

When a sale is created, StockFlow:

```text
Validates customer
        ↓
Validates products
        ↓
Validates stock
        ↓
Calculates total
        ↓
Creates sale
        ↓
Updates stock
        ↓
Creates inventory movements
```

Sales and inventory operations use transactions to avoid inconsistent stock data.

## Tests

Run all tests:

```bash
dotnet test
```

Current test suite:

```text
8 Unit Tests
8 Integration Tests
16 Tests Total
```

## Security

Sensitive values are configured through environment variables.

For local Docker development:

```text
.env
```

The repository includes:

```text
.env.example
```

as a configuration template.

The real `.env` file is excluded from Git.
