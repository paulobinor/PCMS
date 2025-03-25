# Pension Contribution Management System

## Overview
The Pension Contribution Management System is a backend API built with ASP.NET Core 7+, Entity Framework Core, and Hangfire. 
The system facilitates pension contributions, member management, and benefit eligibility processing, 
following Clean Architecture and Domain-Driven Design (DDD) principles.

## Features
- Member Management: Register, update, retrieve, and soft-delete members.
- Contribution Processing:
  - Monthly Contributions (one per month per member).
  - Voluntary Contributions (multiple per month per member).
  - Total contribution calculation and statement generation.
  - Business rules enforcement (minimum contribution period before benefit eligibility).
- Background Jobs:
  - Contribution validation.
  - Benefit eligibility updates and interest calculations.
  - Handling of failed transactions and notifications (using Hangfire).
- Data Validation:
  - Member details (Name, Date of Birth, Email, Phone, Age restrictions: 18-70).
  - Contribution validation (amount > 0, valid contribution date checks).
  - Employer registration validation.
- Global Exception Handling & Logging
- Unit of Work & Repository Pattern

## Prerequisites
Ensure you have the following installed:
- .NET 7 SDK or later
- SQL Server (or an alternative database configured in `appsettings.json`)
- Redis (for caching, optional)
- Hangfire Dashboard (for monitoring background jobs)
- Postman (for API testing, optional)

## Installation
## Clone the Repository
```sh
git clone https://github.com/your-repo/pension-system.git
cd pension-system
```

### Configure Database
Modify **`appsettings.json`** to set up your database connection string:
```json
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=PensionDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
}
```

### Apply Database Migrations
Run the following command in the **NuGet Package Manager Console**:
```
update-database
```

Or, using the .NET CLI:
```
dotnet ef database update
```

### 4️⃣ Run the Application
Start the application using:
```
dotnet run
```
This will start the API on **http://localhost:5000** (or **https://localhost:5001** for HTTPS).

### 5️⃣ Run Hangfire Dashboard
Hangfire dashboard is available at:
```
http://localhost:5000/hangfire
```
You can monitor and manage background jobs from here.

## API Endpoints
### 🔹 Member Management
| Endpoint                 | Method   | Description                    |
|--------------------------|----------|--------------------------------|
| `/api/members`           | `POST`   | Register a new member          |
| `/api/members/{id}`      | `GET`    | Retrieve member details        |
| `/api/members/{id}`      | `PUT`    | Update member details          |
| `/api/members/{id}`      | `DELETE` | Soft-delete a member           |

### 🔹 Contribution Management
| Endpoint                                  | Method | Description                            |
|-------------------------------------------|--------|----------------------------------------|
| `/api/contributions`                      | `POST` | Add a new contribution                 |
| `/api/contributions/{id}`                 | `GET`  | Retrieve contribution details          | 
| `/api/contributions/statement/{memberId}` | `GET`  | Generate a contribution statement      |

### 🔹 Background Jobs & Hangfire
| Endpoint                | Method | Description                  |
|------------------------ |--------|------------------------------|
| `/hangfire`             | `GET`  | Access Hangfire dashboard    |
| `/api/jobs/retryFailed` | `POST` | Retry failed background jobs |

## Unit Testing
Run the unit tests using:
```sh
dotnet test
```
This executes all unit tests to ensure the system functions correctly.

## Logging & Monitoring
- Logging is enabled using Serilog and logs are stored in `logs/log.txt`.
- Hangfire Dashboard provides real-time job monitoring.

## Error Handling
A global exception handler ensures structured API error responses. Example error response:
```
{
    "statusCode": 500,
    "message": "An unexpected error occurred. Please try again later."
}
```

## Contribution
To contribute, fork the repository, create a feature branch, and submit a pull request.

## License
This project is licensed under **MIT License**.

---
**🚀 Developed with ASP.NET Core, Entity Framework, and Hangfire.**

