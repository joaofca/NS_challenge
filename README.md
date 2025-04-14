# CollisionEvents API

A Web API built with ASP.NET Core (.NET 8) for managing space object collision events. Designed with a modular and testable architecture, the project emphasizes maintainability, performance, and clean documentation.

---

## Technologies Used

- **.NET 8** – Core platform
- **ASP.NET Core Web API** – RESTful backend
- **Entity Framework Core** – Data access (support for InMemory and relational databases)
- **Swagger / Swashbuckle** – Automatic API documentation
- **xUnit** – Unit testing framework
- **Unit & Integration Tests** – Validating business logic and full application flows

---

## Solution Structure

The solution is organized into clearly defined layers, following separation of concerns:

CollisionEvents.sln
│
├── CollisionEvents.Api
│   └── Entry point of the application. Contains controllers, Swagger setup, dependency injection, and middleware configuration.
│
├── CollisionEvents.Application
│   └── Application logic and use cases. Contains services and business rules
│
├── CollisionEvents.Contracts
│   └── Data contracts used for communication between layers (e.g., DTOs, request/response models, service interfaces).
│
├── CollisionEvents.Domain
│   └── Core dtos and entities. Contains pure domain logic, validations, and value objects.
│
├── CollisionEvents.Infrastructure
│   └── Implementation of data access logic. Contains the EF Core DbContext, repositories, and configuration for persistence.
│
├── CollisionEvents.UnitTests
│   └── Unit tests for services, domain logic, and application rules using xUnit.
│
├── CollisionEvents.IntegrationTests
│   └── Integration tests that verify the behavior of the system as a whole, including the database and API endpoints.
