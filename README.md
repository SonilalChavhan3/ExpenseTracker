# ExpenseTracker

Expense Tracker Web API
A RESTful Web API built with ASP.NET Core (.NET 9) and SQL Server for tracking personal expenses. This API provides full CRUD operations for managing expenses, categories, and generating expense reports.

🚀 Features
Expense Management - Create, read, update, and delete expenses

Category Management - Organize expenses by categories

User Authentication - JWT-based authentication (optional)

Expense Reports - Generate reports by date range and category

Data Validation - Input validation and error handling

Swagger Documentation - Interactive API documentation

Entity Framework Core - Database access with Code-First approach

Repository Pattern - Clean separation of concerns

📋 Prerequisites
.NET 9 SDK

SQL Server (2019 or later) or SQL Server Express

Visual Studio 2022 / VS Code / Rider (optional)

🛠️ Tech Stack
Framework: ASP.NET Core Web API (.NET 9)

Database: SQL Server

ORM: Entity Framework Core 9

Authentication: JWT Bearer (optional)

Documentation: Swagger/OpenAPI

Logging: Serilog (optional)

📦 Project Structure
text
ExpenseTrackerAPI/
├── Controllers/
│   ├── ExpensesController.cs
│   └── CategoriesController.cs
├── Models/
│   ├── Expense.cs
│   ├── Category.cs
│   └── User.cs
├── Data/
│   └── ApplicationDbContext.cs
├── DTOs/
│   ├── ExpenseDto.cs
│   └── CategoryDto.cs
├── Repositories/
│   ├── IExpenseRepository.cs
│   ├── ExpenseRepository.cs
│   └── ICategoryRepository.cs
├── Services/
│   └── IExpenseService.cs
├── Migrations/
├── Program.cs
└── appsettings.json
🔧 Installation
1. Clone the repository
bash
git clone https://github.com/yourusername/ExpenseTrackerAPI.git
cd ExpenseTrackerAPI
2. Configure Database Connection
Update appsettings.json with your SQL Server connection string:

json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ExpenseTrackerDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
3. Install NuGet Packages
bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore
4. Apply Database Migrations
bash
# Using .NET CLI
dotnet ef migrations add InitialCreate
dotnet ef database update

# Using Package Manager Console (Visual Studio)
Add-Migration InitialCreate
Update-Database
5. Run the Application
bash
dotnet run
The API will be available at:

HTTP: http://localhost:5000

HTTPS: https://localhost:7000

Swagger UI: https://localhost:7000/swagger

📚 API Endpoints
Expenses
Method	Endpoint	Description
GET	/api/expenses	Get all expenses
GET	/api/expenses/{id}	Get expense by ID
GET	/api/expenses/date-range	Get expenses by date range
GET	/api/expenses/category/{categoryId}	Get expenses by category
POST	/api/expenses	Create new expense
PUT	/api/expenses/{id}	Update expense
DELETE	/api/expenses/{id}	Delete expense
GET	/api/expenses/summary/total	Get total expenses
Categories
Method	Endpoint	Description
GET	/api/categories	Get all categories
GET	/api/categories/{id}	Get category by ID
POST	/api/categories	Create new category
PUT	/api/categories/{id}	Update category
DELETE	/api/categories/{id}	Delete category
