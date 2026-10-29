# .NET Developer Learning and Interview Guide

This guide explains the main skills used by a junior .NET web developer, how they fit together, how to practise them, and how to discuss them in an interview. It uses the Student Management project as a working example, but it also covers the wider .NET development cycle.

## 1. The whole technology stack in plain language

Think of a web application as a restaurant:

- The **browser** is the customer. It asks for a page or sends a form.
- **HTTP** is the language used for the request and response.
- **ASP.NET Core** is the restaurant's workflow: it receives requests and routes them to the right code.
- A **controller** is the person who understands the request and coordinates the work.
- A **model** describes the data the application works with.
- A **view** prepares the page the customer sees.
- **Entity Framework Core (EF Core)** translates between C# objects and database commands.
- **SQL Server** stores the long-term data.

The technologies are related but have different jobs:

- **C#** is the programming language.
- **.NET** is the platform, runtime, standard libraries, and development tools used to build and run C# applications.
- **ASP.NET Core** is the web framework on .NET.
- **ASP.NET Core MVC** is a way to organize web applications using Models, Views, and Controllers.
- **Razor** is the template syntax used in `.cshtml` files to combine HTML with server-side C#.
- **EF Core** is an object-relational mapper (ORM) that maps C# objects to database tables.
- **SQL Server LocalDB** is the local SQL Server instance used by this development project.
- **NuGet** is the package manager for .NET libraries.

Your project targets `net8.0` and references EF Core SQL Server version `8.0.11`. .NET 8 support ends in November 2026; .NET 10 is the current LTS release and is supported through November 2028. For the interview, describe the version this project actually uses. For a new project after the interview, use a currently supported release. [Microsoft .NET support policy](https://learn.microsoft.com/en-us/dotnet/core/releases-and-support)

## 2. C# foundations

### Variables and types

A variable is a named place for a value. A type says what kind of value it holds.

```csharp
string studentName = "Asha";
int studentCount = 25;
decimal marks = 88.5m;
bool isActive = true;
```

Use `decimal` for values that need precise decimal arithmetic, such as marks, prices, or money. Use `int` for whole-number identifiers and counts.

### Conditions, loops, and methods

Conditions choose which code runs. Loops repeat code. Methods give a named job to a block of code.

```csharp
if (marks >= 40)
{
    Console.WriteLine("Pass");
}

static decimal Average(decimal first, decimal second)
{
    return (first + second) / 2;
}
```

In an interview, explain a method by saying what it receives, what it does, and what it returns.

### Classes and objects

A class is a definition. An object is one actual value created from that definition.

```csharp
public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Marks { get; set; }
}

var student = new Student { Name = "Asha", Marks = 88.5m };
```

The `Student` class describes the shape of a student. The `student` variable refers to one student object.

### Object-oriented programming

- **Encapsulation:** keep related data and behavior together, and control how data is changed.
- **Abstraction:** expose a simple operation while hiding implementation details. For example, a controller asks a context to save data without constructing SQL itself.
- **Inheritance:** derive a class from another class to reuse or specialize behavior. Use it when the relationship is real, not just to avoid writing a few lines.
- **Polymorphism:** call behavior through a common base type or interface while allowing different implementations.

Interfaces describe capabilities. For example, code can depend on `IStudentService` instead of a specific service class. This makes replacing or testing that dependency easier.

### Collections, generics, and LINQ

A collection holds multiple values. `List<Student>` means a list containing `Student` objects. Generics let a type or method work safely with different data types.

LINQ filters and transforms data in C#:

```csharp
var activeNames = students
    .Where(s => s.Status == "Active")
    .OrderBy(s => s.Name)
    .Select(s => s.Name)
    .ToList();
```

In the project, EF Core can translate many LINQ operations into SQL, so filtering can happen in the database rather than after every record is loaded.

### Exceptions and validation

An exception represents an error during execution. Use `try`/`catch` when you can handle a specific failure or provide a helpful response. Do not catch every error and silently ignore it.

Validation checks whether input is acceptable. In the project, data annotations such as `[Required]`, `[EmailAddress]`, and `[Range(0, 100)]` describe rules for a `Student`.

### Async and await

Database and network calls can take time. An asynchronous method can wait for that work without holding a web request thread idle. `await` pauses that method until the operation completes, then continues.

```csharp
var students = await context.Students.ToListAsync();
```

`async` does not make the database itself faster. It helps the server use its threads efficiently while waiting for I/O.

## 3. The web request and response cycle

The browser and server communicate using HTTP:

1. The browser requests a URL, such as `GET /Students`.
2. ASP.NET Core receives the request.
3. Middleware runs in order.
4. Routing identifies the matching controller and action.
5. Model binding copies route, query-string, and form values into C# parameters or objects.
6. The controller validates input and coordinates work.
7. EF Core may read or write the database.
8. The action returns a result: a view, redirect, JSON response, or error status.
9. ASP.NET Core sends the HTTP response to the browser.
10. The browser renders the HTML or handles the JSON.

Common HTTP methods:

- `GET` reads a page or data. It should not change stored data.
- `POST` submits form data or creates a resource.
- `PUT` usually replaces a resource in an API.
- `PATCH` changes part of a resource in an API.
- `DELETE` deletes a resource in an API.

Common response codes:

- `200 OK`: request succeeded.
- `201 Created`: a new API resource was created.
- `302 Found` / redirect: browser should go to another URL.
- `400 Bad Request`: request data was invalid.
- `401 Unauthorized`: user is not authenticated.
- `403 Forbidden`: user is authenticated but not allowed.
- `404 Not Found`: requested resource does not exist.
- `500 Internal Server Error`: unexpected server failure.

## 4. ASP.NET Core and middleware

`Program.cs` configures and starts the web application.

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<SchoolContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SchoolContext")));

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Students}/{action=Index}/{id?}");
app.Run();
```

Important parts:

- `WebApplication.CreateBuilder` creates the app builder and loads configuration.
- `builder.Services` is the dependency injection container where services are registered.
- `AddControllersWithViews` enables MVC controllers and Razor views.
- `AddDbContext` registers the database context so controllers can receive it.
- `UseHttpsRedirection` redirects HTTP requests to HTTPS when configured.
- `UseStaticFiles` serves files such as CSS and JavaScript from `wwwroot`.
- `UseRouting` finds the endpoint that matches the URL.
- `MapControllerRoute` sets the conventional route. Here the default page is `Students/Index`.
- `Run` starts the web server and waits for requests.

Middleware is like a sequence of request-processing steps. Order matters because each component can inspect, change, pass on, or finish a request. ASP.NET Core also uses middleware for authentication, authorization, exception handling, and logging.

## 5. MVC in detail

MVC stands for **Model–View–Controller**. The separation helps each part focus on a job. [Microsoft's MVC overview](https://learn.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-10.0)

- **Model:** the data and rules the app works with. In this project, `Student` is an entity model.
- **View:** the user interface. In MVC, a `.cshtml` Razor view generates HTML.
- **Controller:** receives a request, coordinates work, and chooses what result to return.

Typical route format:

```text
/Controller/Action/OptionalId
```

Examples:

- `/Students` maps to `StudentsController.Index`.
- `/Students/Create` maps to `StudentsController.Create`.
- `/Students/Edit/5` maps to `StudentsController.Edit` with ID `5`.

`IActionResult` is useful because an action can return several result types, such as `View`, `RedirectToAction`, or `NotFound`.

## 6. MVC versus Web API

An MVC page action commonly returns HTML:

```csharp
return View(students);
```

A Web API action commonly returns data such as JSON and an HTTP status code. API controllers often derive from `ControllerBase` and use attributes such as `[ApiController]` and `[Route("api/[controller]")]`. Microsoft supports both controller-based APIs and Minimal APIs. [Microsoft Web API guide](https://learn.microsoft.com/en-us/aspnet/core/web-api/)

This Student Management project is an MVC website. It does not currently provide REST API endpoints. If another app needed student data as JSON, you could add API controllers and define DTOs (data transfer objects) for the request and response shapes.

## 7. SQL Server and relational database basics

A relational database stores related information in tables:

- A **table** is like a structured sheet, for example `Students`.
- A **row** is one student record.
- A **column** is one field, such as `Email`.
- A **primary key** uniquely identifies a row. Here, `Id` is the primary key.
- A **foreign key** connects a row to another table, for example an `Enrollments.StudentId` column could point to `Students.Id`.
- A **unique constraint/index** prevents duplicate values. This project has a unique index on `StudentNumber`.
- An **index** helps the database find rows efficiently, at the cost of storage and some write work.
- A **join** combines related rows from different tables.
- A **transaction** groups database operations so they succeed or fail together.

Know the basic SQL commands:

```sql
SELECT Name, Email FROM Students WHERE Status = 'Active';
INSERT INTO Students (Name, Email) VALUES ('Asha', 'asha@school.edu');
UPDATE Students SET Department = 'Science' WHERE Id = 1;
DELETE FROM Students WHERE Id = 1;
```

The project uses SQL Server LocalDB, a local SQL Server instance for development. It is suitable for a learning project on one computer; production apps generally connect to a managed or server-based database.

## 8. Entity Framework Core and the database connection

### Connection path

1. `appsettings.json` contains a connection string named `SchoolContext`.
2. The connection string tells EF Core which SQL Server instance and database to use.
3. `Program.cs` passes that string into `UseSqlServer`.
4. `AddDbContext<SchoolContext>` registers the context with dependency injection.
5. `StudentsController` receives `SchoolContext` through its constructor.
6. The context exposes `DbSet<Student> Students`, which EF maps to the `Students` table.
7. EF Core turns LINQ queries into SQL and sends them through the SQL Server provider.
8. `SaveChangesAsync` sends pending inserts, updates, or deletes to the database.

### What `SchoolContext` does

```csharp
public class SchoolContext(DbContextOptions<SchoolContext> options)
    : DbContext(options)
{
    public DbSet<Student> Students => Set<Student>();
}
```

`SchoolContext` is EF Core's session with the database. It tracks entity changes and coordinates queries and writes. The context should generally be short-lived, commonly one per web request through dependency injection.

### Migrations

A migration is a versioned description of a database schema change. For example, adding a property to `Student` may require adding a column to the `Students` table.

- `dotnet ef migrations add AddSomething` generates migration code from model changes.
- `dotnet ef database update` applies migrations to the configured database.
- `Up` contains the forward schema changes.
- `Down` describes how to reverse them when possible.
- `__EFMigrationsHistory` records migrations already applied to a database.

Your `InitialCreate` migration created `StudentManagementDb`, the `Students` table, and its unique student-number index. The database has already been created and updated. EF Core uses migrations to keep a model and database schema in sync. [EF Core migrations documentation](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)

### Tracking and read queries

EF Core tracks objects returned by a normal query so it can detect changes. For read-only pages, `AsNoTracking()` avoids that tracking overhead. Your `Index` action uses it because listing students does not edit those loaded objects.

## 9. Walk through the Student app's CRUD cycle

CRUD means **Create, Read, Update, Delete**.

### Read/list

`GET /Students` calls `Index`. The action builds an EF Core query, applies search and filters, counts results, orders by name, and uses `Skip` and `Take` for pagination. It sends the records to `Views/Students/Index.cshtml`, which renders the table as HTML.

### Create

1. `GET /Students/Create` returns a new empty form.
2. The browser posts the form to `POST /Students/Create`.
3. Model binding copies submitted fields into a `Student` object.
4. Data annotations and `ModelState.IsValid` validate the values.
5. The controller checks whether the student number is already used.
6. `context.Add(student)` marks the object for insertion.
7. `SaveChangesAsync()` writes it to SQL Server.
8. `TempData` stores a one-time success message and `RedirectToAction` sends the browser back to the list.

This redirect after a successful POST is called the **Post/Redirect/Get** pattern. It helps prevent the browser from submitting the same form again when the user refreshes.

### Update

The edit GET loads a student by ID and displays the existing values. The edit POST validates the submitted values, checks that the route ID matches the submitted student ID, checks for a duplicate student number, updates the record, saves, and redirects.

### Delete

The delete GET displays a confirmation page. The delete POST finds and removes the record. The state-changing delete is a POST so simply opening a link cannot delete the record.

## 10. Validation, security, and reliability

- **Server-side validation:** always validate on the server; browser validation can be bypassed. This app uses data annotations and `ModelState.IsValid`.
- **Anti-forgery / CSRF protection:** `[ValidateAntiForgeryToken]` checks a token on form submissions, helping prevent another site from causing a user's browser to submit an unwanted request.
- **Authentication:** proves who the user is, usually through a sign-in process.
- **Authorization:** decides what an authenticated user is allowed to do.
- **Password safety:** store password hashes using a supported identity system; never store plain-text passwords.
- **SQL injection:** parameterized ORM queries help prevent attacker input from becoming executable SQL. Avoid building SQL by concatenating user-provided strings.
- **HTTPS:** encrypts traffic between browser and server.
- **Secrets:** production passwords and keys should not be committed to source control. Use environment variables or a secret store.
- **Error handling and logs:** show users safe error messages while recording useful diagnostic details for developers.

The project has anti-forgery checks on its POST actions and a database unique index for student numbers. It does not currently implement real login or authorization. The administrator label in the interface is display text.

For a production version, improve form binding by using dedicated input/view-model classes, add authentication and authorization, handle the database unique-index race gracefully, and add automated tests.

## 11. Testing, Git, and deployment

### Testing

- **Unit test:** tests one class or behavior in isolation, often using a fake or mock dependency.
- **Integration test:** tests several pieces working together, such as an HTTP request through a controller to a test database.
- **Manual test:** use the browser to try valid and invalid form values and verify that search, edit, and delete behave correctly.

Useful test cases for this app include: valid student creation, invalid email, marks outside 0–100, duplicate student number, editing a missing ID, and deleting a missing ID.

### Git

Git tracks source-code history. A normal workflow is: make a focused change, inspect it, commit it with a clear message, and push the branch to GitHub. Never commit database passwords, API keys, or private student information.

### Build and deployment

- **Build:** compile the code and report compiler errors.
- **Run locally:** run the app on your computer, such as `http://localhost:5000`.
- **Publish:** compile the app for deployment using `dotnet publish`.
- **Configure production:** set production connection strings and secrets safely, use HTTPS, set up database hosting, and apply migrations with a controlled deployment process.
- **Monitor:** review logs and health so failures can be diagnosed after deployment.

Do not apply a development database migration to an important production database without reviewing the generated migration and planning a backup/deployment. EF Core's documentation describes deployment options and cautions to inspect migrations. [Applying EF Core migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/applying)

## 12. The complete software-development cycle

This is the cycle a developer follows for a feature, from idea to maintenance:

1. **Understand the requirement:** who needs the feature, what should happen, and what can go wrong?
2. **Write acceptance criteria:** concrete checks that say when the feature is complete.
3. **Design the data and interface:** decide what information is needed, what screens or endpoints are needed, and how data relates.
4. **Plan the change:** identify model, database, controller/API, view, security, and test changes.
5. **Implement:** write a small amount of code at a time, following the project structure.
6. **Update the database schema:** create and review a migration if the data model changes.
7. **Build:** compile and resolve errors.
8. **Test:** run automated tests and manually try normal and invalid cases.
9. **Review:** inspect the diff, check validation and security, and get a teammate's review if available.
10. **Commit and deploy:** save the change in Git and publish using the team's release process.
11. **Monitor and maintain:** examine errors, fix issues, and improve the feature based on feedback.

For example, “add a student's phone number” requires more than editing the form: add a validated model property, make a migration, update create/edit views, confirm save/read behavior, and test valid and invalid input.

## 13. How to describe your project in an interview

### Short answer (about 30 seconds)

> “I built a Student Management web app in C# using ASP.NET Core MVC. It supports listing, searching, filtering, creating, editing, and deleting students. EF Core connects the app to SQL Server LocalDB. I used data annotations for validation, async database operations, anti-forgery validation for form posts, and a migration to create the database schema.”

### Longer answer (about 90 seconds)

> “The request enters ASP.NET Core and routing maps it to an action in `StudentsController`. For example, the list action builds a LINQ query using `SchoolContext`. EF Core translates the query into SQL and reads from the `Students` table in SQL Server LocalDB. The controller passes the records to a strongly typed Razor view, which renders HTML for the browser. For a create request, the form posts values, model binding fills a `Student`, validation checks it, the controller checks the unique student number, and `SaveChangesAsync` writes it. I used EF Core migrations to create and update the database schema.”

### If asked what you would improve

> “I would add real authentication and role-based authorization, use dedicated view models instead of binding database entities directly, add unit and integration tests, and create a separate Web API if a mobile app needed JSON. I would also add production-grade logging and configure secrets outside source control.”

Be accurate: this project is a server-rendered MVC app, not a REST API. It uses custom CSS rather than Bootstrap. The displayed administrator profile is not a real login system.

## 14. Common interview questions and beginner answers

**What is .NET?**  
“.NET is a development platform with a runtime, libraries, and tools. I use it with C# to build and run applications.”

**What is ASP.NET Core?**  
“It is the web framework on .NET for building web applications and APIs.”

**What is MVC?**  
“Model–View–Controller. The model describes the data, the view displays the page, and the controller handles the request and coordinates work.”

**What is dependency injection?**  
“It is a pattern where a class receives the services it needs instead of constructing them itself. ASP.NET Core provides `SchoolContext` to my controller.”

**What is EF Core?**  
“It is an ORM that maps C# entities to database tables and translates LINQ queries into database commands.”

**What is a migration?**  
“It is a versioned description of a database schema change. EF Core creates migrations from model changes, and `database update` applies them.”

**What is a primary key?**  
“A column or set of columns that uniquely identifies each row.”

**What is an API?**  
“A defined way for software systems to communicate. A web API commonly receives HTTP requests and returns JSON data with status codes.”

**What is the difference between authentication and authorization?**  
“Authentication checks who the user is. Authorization checks what that user is permitted to do.”

**What is `AsNoTracking`?**  
“It tells EF Core a query is read-only, so EF does not track changes to those returned objects.”

**Why use async database calls?**  
“Database operations wait on I/O. Async lets the server avoid blocking a thread during that wait.”

If you do not know an answer, explain what you do know and say how you would investigate it. Do not pretend to have used a technology you have not used.

## 15. What to study before October 30

You have a few days, so prioritize understanding and explaining these ideas:

- **October 27:** C# types, classes, methods, collections, LINQ, and the four OOP concepts.
- **October 28:** .NET, ASP.NET Core, request routing, MVC, middleware, and dependency injection.
- **October 29:** SQL basics, EF Core, migrations, validation, HTTP methods, APIs, and your project walkthrough.
- **October 30:** run the app, demo one create and one list flow, give the 30-second introduction, and review questions you struggled with.

For longer-term learning, use a repeatable cycle: **learn one concept → write a tiny example → use it in a feature → explain it aloud → test it**. Microsoft Learn has free starting points for [C#](https://learn.microsoft.com/en-us/training/paths/get-started-c-sharp-part-1/), [ASP.NET Core](https://learn.microsoft.com/en-us/training/paths/aspnet-core-web-app/), [ASP.NET Core fundamentals](https://learn.microsoft.com/en-us/training/paths/aspnet-core-fundamentals/), and [EF Core](https://learn.microsoft.com/en-us/ef/core/).

## 16. Self-checklist for becoming job-ready

You are building a solid junior foundation when you can do these without copying every step:

- Explain basic C# types, methods, classes, interfaces, collections, LINQ, exceptions, and async/await.
- Build a basic MVC page and explain its model, view, controller, route, and request flow.
- Build a CRUD Web API that returns JSON and uses correct HTTP methods and status codes.
- Design simple SQL tables with keys, constraints, and relationships; write basic queries and joins.
- Connect EF Core to a database, create a migration, apply it, and explain what changed.
- Validate user input, handle errors, and explain authentication versus authorization.
- Use Git to commit work and describe the changes in a clear README.
- Build and run the app, diagnose a simple error, and explain how you would publish it.

You do not need mastery of every item before applying for a junior job. Keep practising, make your own small changes, and be honest about what you have and have not used.
