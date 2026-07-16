# Car Management System

A terminal-based (TUI) Car Management application built with **C# / .NET 8**, **Terminal.Gui**, and **PostgreSQL**. It supports full CRUD operations (Create, Read, Update, Delete) on a car inventory and is designed to be operated **entirely from the keyboard**, with color-coded screens for quick visual orientation.

## Features

- **Create** — add a new car with year, make, model, odometer, and price.
- **Read** — browse cars split into two lists: *Available* and *Sold*.
- **Update** — edit a car's details, or mark an available car as sold.
- **Delete** — permanently remove a car (with a confirmation prompt).
- **Keyboard-first navigation** — every action (`Edit`, `Sell`, `Delete`, `Save`, `Back`) has a single-key shortcut; no mouse required.
- **Color-coded screens** — Available, Sold, Add, and Edit each use a distinct color scheme so you always know which screen you're on.
- **Input validation** — numeric fields and required fields are validated before hitting the database.

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 8 |
| UI | [Terminal.Gui](https://github.com/gui-cs/Terminal.Gui) 1.19 (console-based UI) |
| Database | PostgreSQL |
| Data access | [Npgsql](https://www.npgsql.org/) (raw SQL, parameterized queries) |
| Dependency Injection | Microsoft.Extensions.DependencyInjection |

## Project Structure

```
Car/
├── Car.sln
└── Car/
    ├── CarApp.csproj
    ├── Program.cs                  # composition root / DI setup / app entry point
    ├── Database/
    │   └── CreateDatabase.sql      # schema for the Cars table
    ├── Data/
    │   ├── IDatabase.cs            # connection abstraction
    │   └── Postgresdb.cs           # Npgsql connection factory
    ├── Models/
    │   ├── Car.cs                  # domain entity
    │   └── CarStatus.cs            # Available / Sold enum
    ├── Repository/
    │   ├── ICarRepository.cs       # data access contract
    │   └── CarRepository.cs        # SQL implementation (CRUD)
    ├── Service/
    │   └── CarService.cs           # business logic layer
    └── UI/
        └── MainWindow.cs           # Terminal.Gui screens & keyboard handling
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/download/) (v13 or later recommended), running locally or accessible over the network
- A terminal that supports standard ANSI colors (Windows Terminal, macOS Terminal/iTerm2, or any modern Linux terminal)

## Database Setup

The application does **not** create its database or table automatically — you need to set these up once before running it.

### 1. Create the database

Connect to PostgreSQL with `psql` (or any client such as pgAdmin/DBeaver) and create a dedicated database:

```sql
CREATE DATABASE "CarManage";
```

From the command line, this can also be done with:

```bash
createdb -U postgres CarManage
```

### 2. Create the `Cars` table

Run the provided schema script against the new database. From the `Car/Car` directory:

```bash
psql -U postgres -d CarManage -f Database/CreateDatabase.sql
```

This creates the `Cars` table:

```sql
CREATE TABLE IF NOT EXISTS Cars (
    Id          BIGINT GENERATED ALWAYS AS IDENTITY,
    "Year"      INTEGER          NOT NULL,
    Make        VARCHAR(50)      NOT NULL,
    Model       VARCHAR(50)      NOT NULL,
    Odometer    BIGINT           NOT NULL,
    Price       NUMERIC(12,2)    NOT NULL,
    Status      VARCHAR(50)      NOT NULL,
    CONSTRAINT  PK_Cars          PRIMARY KEY (Id),
    CONSTRAINT  CK_Cars_Year     CHECK ("Year" BETWEEN 1901 AND 2026),
    CONSTRAINT  CK_Cars_Price    CHECK (Price >= 0),
    CONSTRAINT  CK_Cars_Status   CHECK (Status IN ('Available', 'Sold'))
);
```

### 3. Configure the connection string

The app reads its PostgreSQL connection string from an environment variable named **`CARAPP_CONNECTION`** — nothing is hard-coded in the source, so your credentials never end up in source control.

Set it before running the app:

**Windows (PowerShell)**
```powershell
$env:CARAPP_CONNECTION = "Host=localhost;Port=5432;Username=postgres;Password=your_password;Database=CarManage"
```

**Windows (permanent, so it persists across sessions)**
```powershell
setx CARAPP_CONNECTION "Host=localhost;Port=5432;Username=postgres;Password=your_password;Database=CarManage"
```

**macOS / Linux (bash/zsh)**
```bash
export CARAPP_CONNECTION="Host=localhost;Port=5432;Username=postgres;Password=your_password;Database=CarManage"
```

To make it permanent on macOS/Linux, add the `export` line to your `~/.bashrc`, `~/.zshrc`, or `~/.profile`.

Adjust `Host`, `Port`, `Username`, `Password`, and `Database` to match your actual PostgreSQL setup. If the variable is not set, the application will throw a clear error on startup telling you to define it.

## Running the Application

From the `Car/` directory (where `Car.sln` lives):

```bash
dotnet restore
dotnet build
dotnet run --project Car
```

On first run, `dotnet restore` will download the required NuGet packages (`Npgsql`, `Terminal.Gui`, `Microsoft.Extensions.DependencyInjection`).

## Usage / Keyboard Shortcuts

Everything in the app is reachable without a mouse:

| Screen | Shortcut | Action |
|---|---|---|
| Main Menu | `1` / `2` / `3` | Open Available Cars / Sold Cars / Add Car |
| Main Menu | `E` | Exit the application |
| Main Menu | `↑` / `↓` + `Enter` | Navigate and select via the menu list |
| Available Cars | `E` | Edit the selected car |
| Available Cars | `S` | Mark the selected car as sold (confirmation required) |
| Available Cars | `D` | Delete the selected car (confirmation required) |
| Available Cars | `Esc` | Back to main menu |
| Sold Cars | `D` | Delete the selected car (confirmation required) |
| Sold Cars | `Esc` | Back to main menu |
| Add Car / Edit Car | `Tab` / `Shift+Tab` | Move between input fields |
| Add Car / Edit Car | `Enter` | Save |
| Add Car / Edit Car | `Esc` | Cancel and go back |

## Architecture Notes

- **Layered design**: `Data` → `Repository` → `Service` → `UI`. Each layer only depends on the interface of the layer beneath it (`IDatabase`, `ICarRepository`), which keeps the SQL and the UI fully decoupled.
- **Dependency Injection**: all components are registered in `Program.cs` via `Microsoft.Extensions.DependencyInjection` and resolved through constructor injection — no class calls `new` on its own dependencies.
- **Parameterized SQL**: all queries use `NpgsqlCommand` parameters (e.g. `@year`, `@make`) to prevent SQL injection.
- **Resilient data access**: repository methods wrap database calls in `try/catch/finally`, so a database outage or query failure is caught and reported instead of crashing the app.

## Troubleshooting

- **"Please create the 'CARAPP_CONNECTION' environment variable..."** — the environment variable isn't set in the terminal session you're running from. Set it as shown above, and make sure you're running `dotnet run` from a terminal where that variable is visible (restart the terminal after using `setx` on Windows).
- **Connection refused / timeout** — verify PostgreSQL is running and listening on the host/port in your connection string, and that the `pg_hba.conf` on the server allows connections from your machine.
- **relation "cars" does not exist** — the schema script (`Database/CreateDatabase.sql`) hasn't been run against the target database yet.
