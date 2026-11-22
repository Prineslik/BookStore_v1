![Tests](https://img.shields.io/badge/tests-40%20passed-brightgreen)
![.NET](https://img.shields.io/badge/.NET-10.0-purple)
# BookStore — Book Catalog & Reading Management API

ASP.NET Core Web API pet project inspired by Yandex Books. The application provides a REST API for managing a book catalog, users, authentication and access control.

The project is being developed as a portfolio project to demonstrate practical backend development skills with C#, ASP.NET Core, Entity Framework Core and PostgreSQL.

> **Project status:** Core backend functionality and infrastructure are implemented. Additional product features are under development.

---

## Tech Stack

| Technology | Purpose |
|---|---|
| C# | Main programming language |
| ASP.NET Core Web API | REST API development |
| Entity Framework Core | ORM and database access |
| PostgreSQL | Relational database |
| JWT Bearer | Authentication |
| ASP.NET Core Authorization | Role- and permission-based access control |
| FluentValidation | Request validation |
| Docker / Docker Compose | Containerization and local environment |
| GitHub Actions | CI pipeline |
| Swagger / OpenAPI | API documentation and testing |
| xUnit | Automated testing |
| NSubstitute | Mocking dependencies |
| FluentAssertions | Test assertions |

---

## Architecture

The application follows a layered architecture with separation of responsibilities.

The main architectural components include:

- **API** — HTTP endpoints, controllers, middleware and application configuration.
- **Application** — business logic, services, DTOs, validation and application abstractions.
- **Domain** — domain entities and business rules.
- **Infrastructure** — Entity Framework Core, PostgreSQL, repositories and external infrastructure concerns.

The project uses dependency injection to manage dependencies between layers.

### Architectural patterns and approaches

- Repository Pattern.
- Unit of Work.
- Dependency Injection.
- DTO-based data transfer.
- Separation of read and write repository operations.
- Expression-based projections with LINQ and EF Core.
- Centralized exception handling.
- Middleware-based request processing.

---

## Implemented Features

### 1. User Management

- User registration.
- User authentication.
- User profile management.
- Password hashing.
- User and role management.

### 2. Authentication and Authorization

- JWT Bearer authentication.
- Role-based authorization.
- Permission-based authorization.
- Policy-based authorization.
- Fallback Policy (default-deny approach).
- Ownership checks for user-specific operations.
- Protection of administrative endpoints.

### 3. Book Catalog

- CRUD operations for books.
- DTO-based request and response models.
- Request validation using FluentValidation.
- Search and filtering.
- Sorting.
- Pagination.

### 4. Data Access

- PostgreSQL integration.
- Entity Framework Core.
- Repository Pattern.
- Unit of Work.
- Entity configurations using `IEntityTypeConfiguration`.
- Database migrations.
- LINQ queries and projections.
- Centralized persistence operations.

### 5. Error Handling and Validation

- Global exception handling.
- Centralized mapping of application errors to HTTP responses.
- ProblemDetails (RFC 7807).
- Request validation.
- Consistent API error responses.

### 6. Testing

The project includes automated unit and integration tests.

Testing tools:

- xUnit.
- NSubstitute.
- FluentAssertions.
- WebApplicationFactory for integration testing.

The tests cover important application scenarios, including:

- User registration.
- Authentication and authorization.
- Access restrictions.
- Business logic.
- API request processing.
- Error handling.

### 7. Development Infrastructure

- Dockerfile for the API.
- Docker Compose configuration with PostgreSQL.
- Environment-based configuration.
- User Secrets for local development.
- GitHub Actions CI workflow.
- Automated restore, build and test on push and pull requests.
- Swagger UI with JWT authentication support.

---

## Planned Features

The following functionality is planned but not yet implemented.

### Authors

- Author management endpoints.
- Book-author relationships.
- Filtering books by author.

### Favorites and Reading History

- Add and remove books from favorites.
- Mark books as read.
- View personal favorite and read lists.
- Track reading dates.

### Reviews and Ratings

- Create and delete book reviews.
- One review per user for each book.
- Book ratings.
- Average rating calculation.
- Paginated review lists.

### Additional Improvements

- CancellationToken propagation throughout asynchronous operations.
- Health Checks.
- Enhanced structured logging with Serilog.
- Additional integration tests.
- Final documentation and deployment improvements.

---

## Getting Started

### Prerequisites

Make sure the following tools are installed:

- .NET SDK (version specified by the project).
- Docker Desktop or Docker Engine.
- Git.

### 1. Clone the repository

```bash
git clone <repository-url>
cd <repository-directory>
```

### 2. Configure application settings

Configure the required connection string and JWT settings using .env for local development or environment variables.

Create a local configuration file:

```bash
cp .env.example .env
```

### 3. Run the application

The project includes a Docker Compose configuration for the API and PostgreSQL.

```bash
docker compose up --build
```

### 4. Explore the API

After starting the application, open Swagger UI using the configured application URL:

| Service    | URL                                                            |
| ---------- | -------------------------------------------------------------- |
| API        | [http://localhost:5000](http://localhost:5000)                 |
| Swagger UI | [http://localhost:5000/swagger](http://localhost:5000/swagger) |
| PostgreSQL | localhost:5432                                                 |

Swagger UI supports JWT Bearer authentication for testing protected endpoints.

Ports in Docker Compose:
- API: `5000:8080` — порт `5000` на хосте перенаправляется на `8080` внутри контейнера.
- PostgreSQL: `5432:5432`.

### 5. Close the API

To stop containers:

```bash
docker compose down
```

To also delete PostgreSQL data stored in Docker volumes:

```bash
docker compose down -v
```

>**Note:** The docker compose down -v command deletes data in the corresponding volumes. Use it only if you really want to reset the database.

---

## Running Tests

Run all tests from the solution directory:

```bash
dotnet test
```

Build the solution:

```bash
dotnet build
```

The GitHub Actions workflow automatically restores dependencies, builds the solution and executes tests on push and pull requests.

---

## Project Goals

The main goals of this project are:

- Apply clean separation of responsibilities in an ASP.NET Core application.
- Design and implement RESTful APIs.
- Work with relational databases using EF Core and PostgreSQL.
- Implement authentication and authorization.
- Write maintainable and testable business logic.
- Develop automated unit and integration tests.
- Use Docker and CI automation in the development workflow.

---

## Project Status

This is an actively developed educational project. Core backend functionality, authentication, authorization, database access and CI infrastructure are implemented. The remaining product features and final documentation are being developed incrementally.
