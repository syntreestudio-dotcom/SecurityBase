# SecurityBase System

A comprehensive security management system built with .NET 8, Dapper, and SQL Server.

## Features
- **Clean Architecture**: Decoupled layers (Core, Infrastructure, API, Mvc).
- **Backend**: .NET 8 Web API with Dapper and Stored Procedures.
- **Frontend**: .NET 8 MVC with Bootstrap 5, AJAX, and DataTables.
- **Authentication**: JWT-based security with session handling.
- **Modules**:
    - Dashboard Overview.
    - User Management (CRUD).
    - Role Management (CRUD).
    - Menu Management (CRUD).
    - Role & Menu Assignment.

## Setup Instructions

### 1. Database Setup
1. Open SQL Server Management Studio (SSMS).
2. Create a new database named `SecurityBase`.
3. Execute the script in `Database/Tables.sql`.
4. Execute the script in `Database/StoredProcedures.sql`.

### 2. Backend Configuration
1. Navigate to `Backend/SecurityBase.Api`.
2. Update the `ConnectionStrings:DefaultConnection` in `appsettings.json` with your SQL Server instance name.
3. Run the API:
   ```bash
   dotnet run
   ```
   The API will start at `http://localhost:5000` (or as configured).

### 3. Frontend Configuration
1. Navigate to `Frontend/SecurityBase.Mvc`.
2. Ensure `ApiSettings:BaseUrl` in `appsettings.json` matches your API URL.
3. Run the MVC app:
   ```bash
   dotnet run
   ```
   The app will start at `http://localhost:5001` (or as configured).

### 4. Usage
- The default landing page is the **Login** page.
- Since there are no users initially, you may need to manually insert a user into the `Users` table or use the `sp_CreateUser` SP to create an admin account.
- **Demo Admin (Manual Insert Example)**:
  ```sql
  EXEC sp_CreateUser @Username='admin', @PasswordHash='admin123', @Email='admin@example.com', @IsActive=1;
  ```

## Technologies Used
- **Backend**: .NET 8 Web API, Dapper, SQL Server, Serilog, FluentValidation, JWT Bearer.
- **Frontend**: .NET 8 MVC, Bootstrap 5, jQuery, DataTables, SweetAlert2, FontAwesome.
