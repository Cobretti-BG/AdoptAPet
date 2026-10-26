# Adopt-A-Pet

Adopt-A-Pet is an ASP.NET Core MVC web application that connects people looking to adopt pets with people with pets looking for loving home. Users can create pet listings, submit adoption requests, and manage the adoption process.

## Features

- **User accounts** — Register and log in using ASP.NET Core Identity.
- **Browse pets** — View available pets and open individual pet details.
- **Manage pet listings** — Create, view, edit, and delete your own pet listings.
- **Adoption requests** — Submit requests to adopt pets and view your request history.
- **Request management** — Pet owners can review, approve, or reject adoption requests.
- **Request cancellation** — Applicants can cancel their pending requests.
- **Validation and feedback** — Server-side validation, business-rule checks, and user notifications.
- **Interface** — Bootstrap-based layout.

## Technologies

- C#
- ASP.NET Core MVC
- Razor Views and Razor Pages
- ASP.NET Core Identity
- Entity Framework Core
- Microsoft SQL Server
- Bootstrap
- HTML and CSS

## Architecture

The solution follows a layered architecture with separate responsibilities.

| Project | Responsibility |
|---|---|
| `AdoptAPet.Web` | MVC controllers, views, view models, application configuration, and web interface |
| `AdoptAPet.Service` | Application services, service contracts, DTOs, validation, and business rules |
| `AdoptAPet.Data` | Entity models, EF Core database context, entity configurations, and migrations |
| `AdoptAPet.Common` | Shared constants and validation values |

Dependency injection is used to provide application services and database dependencies to the components that need them.

## Requirements

- Visual Studio with the ASP.NET and web development workload.
- .NET SDK 10.
- Microsoft SQL Server


## Setup

### 1. Clone the repository

Clone the repository from GitHub and open `AdoptAPet.sln` in Visual Studio.

### 2. Configure the database

The connection string settings are stored in `AdoptAPet.Web/appsettings.json`.

Configure `DefaultConnection` for your local SQL Server instance and authentication method. For example with windows authentication:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=AdoptAPetDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```


### 3. Apply database migrations

Open the Package Manager Console in Visual Studio, select `AdoptAPet.Data` as the Default project and ensure `AdoptAPet.Web` is the startup project.

Run:
```
Update-Database
```
