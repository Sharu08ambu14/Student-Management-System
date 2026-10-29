# Campusly Student Management

A beginner friendly ASP.NET Core MVC app for managing a school's student directory. It includes student creation, listing, search and filters, editing, deletion, validation, summary metrics and SQL Server persistence through Entity Framework Core.

## Requirements

- .NET 8 SDK
- SQL Server LocalDB (included with Visual Studio) or a SQL Server instance

## Run locally

1. Install the .NET 8 SDK and open a terminal in this folder.
2. Set `ConnectionStrings:SchoolContext` in `appsettings.json` to your SQL Server connection string. The default targets LocalDB.
3. Create the database schema and start the app:

   ```powershell
   dotnet tool install --global dotnet-ef
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   dotnet run
   ```

4. Open the localhost URL printed in the terminal.

The project uses SQL Server via EF Core. Student IDs are unique; names, email addresses, departments, marks (0–100), status and enrollment date are validated. Search covers name, email and student ID. Filters cover department and status.

## Project structure

- `Models/Student.cs` — student fields and validation rules
- `Data/SchoolContext.cs` — Entity Framework database context
- `Controllers/StudentsController.cs` — list, search, create, edit and delete actions
- `Views/Students/` — Razor pages
- `wwwroot/` — responsive interface styles and keyboard shortcut
