# Company Organization REST API

REST API for managing a company's organizational structure and keeping track of employees.

## Hierarchy

* Company → Division → Project → Department

The API allows creating, updating, and deleting all hierarchy nodes, as well as managing employees including assigning leaders.

## Technologies Used

* C# / .NET 10.0
* ASP.NET Core Web API
* Entity Framework Core
* Microsoft SQL Server Express
* Scalar (API documentation)
* TeaPie (HTTP tests)

## Running the Project (Local)

### Requirements

The following must be installed on your machine:

* .NET SDK 10.0
* Microsoft SQL Server Express
* sqlcmd (SQL Server command-line tools)

### 1️⃣ Database Configuration

Edit the connection string in `appsettings.json` according to your local SQL Server Express instance:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=firmyDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 2️⃣ Creating the Database

From the project root, run the SQL script:

```powershell
sqlcmd -S localhost\SQLEXPRESS -E -C -i db/create-database.sql
```

Replace `localhost\SQLEXPRESS` with your local SQL Server instance name if different.

The script will:

* create the database `firmyDB`
* create all tables
* populate the database with sample data (2 companies, full hierarchy, employees)

### 3️⃣ Running the Application

```powershell
dotnet run
```

The API will be available at:

```
http://localhost:5051
```

### 4️⃣ API Documentation (Scalar)

Scalar is integrated directly into the application and available at:

```
http://localhost:5051/scalar
```

It allows you to:

* browse all endpoints
* test requests directly in the browser

### 5️⃣ Endpoint Testing (TeaPie)

To run HTTP tests, install the TeaPie CLI tool:

```powershell
dotnet tool install -g TeaPie.Tool
```

Run the tests:

```powershell
teapie test ./test-req.http
```

The `test-req.http` file contains tests for:

* Companies
* Divisions
* Projects
* Departments
* Employees

## Basic Validation Rules

* All required fields must be filled (e.g., name, code)
* Node leader must be an employee belonging to the respective company
* Hierarchical relationships are validated (e.g., project must belong to a division, division to a company)

## Known Limitations

* **A company cannot be deleted if it has assigned employees**

  * The API returns an error to prevent breaking referential integrity.
* **Deleting hierarchical nodes**

  * Divisions, projects, and departments are deleted cascade-wise according to SQL constraints.
* **Employee may not be assigned to a department**

  * `DepartmentId` is optional and is set to `NULL` when a department is deleted.
* **Authentication and authorization not implemented**

  * The API is designed as a technical demo without a security layer.

## Project Structure (Overview)

* `Controllers/` – REST API endpoints
* `Models/` – database entities
* `DTOs/` – request/response objects
* `Validation/` – hierarchy validation logic
* `db/` – SQL scripts (database creation and seed)

---


