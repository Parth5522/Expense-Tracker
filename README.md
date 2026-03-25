# Expense Tracker

A full-featured, multi-user personal finance management application built with ASP.NET Core 8.0 MVC and PostgreSQL. Track expenses and income, set budgets and savings goals, manage recurring transactions, and gain deep insights into your financial habits through a rich analytics dashboard.

## Features

### 💰 Expense & Income Management
- **Full CRUD Operations**: Create, Read, Update, and Delete both expenses and income records
- **10 Expense Categories**: Food, Transportation, Housing, Utilities, Entertainment, Healthcare, Shopping, Education, Travel, and Other
- **8 Income Sources**: Salary, Freelance, Investment, Rental, Business, Gift, Bonus, and Other
- **File Attachments**: Attach receipts or documents (JPEG, PNG, GIF, PDF — up to 5 MB) to any expense
- **Tags**: Create custom color-coded tags and apply them to expenses or income for fine-grained categorization
- **Multi-Currency Support**: Record transactions in USD, EUR, GBP, INR, JPY, CAD, or AUD with configurable exchange rates
- **Form Validation**: Client-side and server-side validation to ensure data integrity

### 📊 Dashboard & Analytics
- **Summary Cards**: Total expenses, monthly expenses, average expense, and transaction count at a glance
- **Pie Chart**: Visual breakdown of expenses by category
- **Bar Chart**: Monthly spending trends for the last 6 months
- **Recent Transactions**: Quick view of your latest 5 expenses
- **Reports**: Detailed analytics view with customizable date ranges and category breakdowns

### 🔁 Recurring Transactions
- Automate creation of expenses or income on **Daily**, **Weekly**, **Monthly**, or **Yearly** schedules
- A background hosted service processes due recurring transactions automatically
- Track `NextRunAt`, `LastRunAt`, and optional `EndDate` per transaction

### 🎯 Budgets & Goals
- **Budgets**: Set monthly spending limits overall or per category; get alerts when a budget is exceeded
- **Financial Goals**: Define savings targets with a target date and track progress (current amount vs. target)

### 🔔 Notifications
- Receive in-app notifications for key events:
  - Budget exceeded
  - Goal achieved
  - Recurring transaction processed
  - Upcoming bill reminders
- Mark notifications as read with timestamps

### 🔍 Filtering, Search & Pagination
- Filter expenses or income by date range, category/source, and free-text search (title or description)
- Combine multiple filters simultaneously
- Paginate through large data sets easily

### 👤 Authentication & Authorisation
- **User registration and login** powered by ASP.NET Core Identity
- **Role-based access**: standard users see only their own data; admins manage all data
- Cookie-based authentication (7-day sliding expiry)
- JWT Bearer token authentication available for the REST API

### 🛠 Admin Panel
- Admin dashboard with site-wide statistics
- Manage all users, expenses, income entries, and supported currencies

### 🌐 REST API
- Full RESTful JSON API for Expenses, Income, Budgets, Goals, Tags, and Auth
- Secured with JWT Bearer tokens
- Swagger/OpenAPI UI available at `/swagger` in Development

### 🎨 Modern UI/UX
- **Bootstrap 5**: Responsive, mobile-first design
- **Font Awesome 6**: Icons throughout the interface
- **Chart.js 4**: Interactive charts on the dashboard
- **Toast Notifications**: Real-time success and error feedback
- **Smooth Animations**: Hover effects, transitions, and fade-in animations
- Works seamlessly on desktop, tablet, and mobile devices

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend framework | ASP.NET Core 8.0 MVC |
| Language | C# 12 (.NET 8) |
| Database | PostgreSQL 12+ |
| ORM | Entity Framework Core 8 (Npgsql provider) |
| Authentication | ASP.NET Core Identity + JWT Bearer |
| Frontend | Razor Views, Bootstrap 5, jQuery |
| Charts | Chart.js 4.4 |
| Icons | Font Awesome 6.4 |
| API docs | Swagger / Swashbuckle 6.6 |
| CSV export | CsvHelper 33 |
| Background jobs | .NET hosted background service |

---

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [PostgreSQL](https://www.postgresql.org/download/) 12 or later
- A code editor such as [Visual Studio](https://visualstudio.microsoft.com/), [Visual Studio Code](https://code.visualstudio.com/), or [JetBrains Rider](https://www.jetbrains.com/rider/)

---

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/Parth5522/Expense-Tracker.git
cd Expense-Tracker/ExpenseTracker
```

### 2. Configure the Database Connection

Open `appsettings.json` and update the connection string with your PostgreSQL credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ExpenseTrackerDb;Username=postgres;Password=your_password_here"
  }
}
```

| Placeholder | Description |
|---|---|
| `localhost` | PostgreSQL host |
| `5432` | PostgreSQL port |
| `ExpenseTrackerDb` | Database name to create |
| `postgres` | PostgreSQL username |
| `your_password_here` | PostgreSQL password |

### 3. Configure JWT (for API access)

In `appsettings.json`, set a strong secret key (minimum 32 characters) for JWT token signing:

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyAtLeast32CharsLong!",
    "Issuer": "ExpenseTracker",
    "Audience": "ExpenseTrackerUsers"
  }
}
```

> ⚠️ **Never commit a real production secret to source control.** Use environment variables or a secrets manager in production.

### 4. Restore Dependencies

```bash
dotnet restore
```

### 5. Apply Database Migrations

The application runs migrations automatically on startup. Alternatively, apply them manually:

```bash
dotnet ef database update
```

This creates the database and all tables. In Development mode, seed data is loaded automatically.

### 6. Run the Application

```bash
dotnet run
```

Or press **F5** in Visual Studio / Rider.

The application is available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

Register a new account to get started. The first admin account can be set up directly in the database or via seeded data.

---

## Project Structure

```
ExpenseTracker/
├── Controllers/
│   ├── AccountController.cs          # Registration, login, profile, password
│   ├── AdminController.cs            # Admin dashboard & user management
│   ├── BudgetsController.cs          # Budget CRUD
│   ├── ExpensesController.cs         # Expense CRUD with filters
│   ├── GoalsController.cs            # Financial goals CRUD
│   ├── HomeController.cs             # Dashboard & analytics
│   ├── IncomeController.cs           # Income CRUD
│   ├── NotificationsController.cs    # Notification list & mark-read
│   ├── RecurringController.cs        # Recurring transaction CRUD
│   ├── ReportsController.cs          # Reports & analytics
│   ├── TagsController.cs             # Tag management
│   └── Api/                          # RESTful JSON API controllers
│       ├── AuthApiController.cs
│       ├── BudgetsApiController.cs
│       ├── ExpensesApiController.cs
│       ├── GoalsApiController.cs
│       ├── IncomeApiController.cs
│       └── TagsApiController.cs
│
├── Models/
│   ├── ApplicationUser.cs            # Extended Identity user
│   ├── Attachment.cs                 # File attachment entity
│   ├── Budget.cs                     # Budget entity
│   ├── Currency.cs                   # Supported currencies & rates
│   ├── Expense.cs                    # Expense entity
│   ├── ExpenseCategory.cs            # Category enum (10 values)
│   ├── Goal.cs                       # Savings goal entity
│   ├── Income.cs                     # Income entity
│   ├── Notification.cs               # Notification entity
│   ├── RecurringTransaction.cs       # Recurring transaction entity
│   └── Tag.cs                        # Tag entity + junction tables
│
├── ViewModels/
│   ├── DashboardViewModel.cs
│   ├── ExpenseFilterViewModel.cs
│   ├── ReportViewModel.cs
│   ├── LoginViewModel.cs
│   ├── RegisterViewModel.cs
│   ├── ChangePasswordViewModel.cs
│   ├── ForgotPasswordViewModel.cs
│   └── ResetPasswordViewModel.cs
│
├── Services/
│   ├── IExpenseService.cs / ExpenseService.cs
│   ├── IIncomeService.cs / IncomeService.cs
│   ├── IBudgetService.cs / BudgetService.cs
│   ├── IGoalService.cs / GoalService.cs
│   ├── ITagService.cs / TagService.cs
│   ├── INotificationService.cs / NotificationService.cs
│   ├── IAttachmentService.cs / AttachmentService.cs
│   ├── IRecurringTransactionService.cs / RecurringTransactionService.cs
│   ├── IReportService.cs / ReportService.cs
│   ├── ICurrencyService.cs / CurrencyService.cs
│   ├── IJwtService.cs / JwtService.cs
│   ├── RecurringTransactionHostedService.cs  # Background scheduler
│   └── CustomUserClaimsPrincipalFactory.cs
│
├── Data/
│   ├── ApplicationDbContext.cs       # EF Core DbContext
│   └── SeedData.cs                   # Demo data seeder
│
├── Migrations/                       # EF Core migration files
│
├── Views/
│   ├── Home/                         # Dashboard
│   ├── Expenses/                     # Expense CRUD views
│   ├── Income/                       # Income CRUD views
│   ├── Budgets/                      # Budget CRUD views
│   ├── Goals/                        # Goals CRUD views
│   ├── Recurring/                    # Recurring transaction views
│   ├── Tags/                         # Tag management views
│   ├── Reports/                      # Analytics & reports
│   ├── Notifications/                # Notification list
│   ├── Account/                      # Auth views (login, register, profile…)
│   ├── Admin/                        # Admin panel views
│   └── Shared/                       # Layout & partials
│
├── wwwroot/
│   ├── css/site.css                  # Custom styles
│   └── js/site.js                    # Custom JavaScript
│
├── Program.cs                        # Entry point & DI configuration
├── appsettings.json                  # Production configuration
└── appsettings.Development.json      # Development overrides
```

---

## Database Schema

### Core Tables

| Table | Key Columns |
|---|---|
| **AspNetUsers** | Id, UserName, Email, DisplayName, BaseCurrency, CreatedAt |
| **Expenses** | Id, UserId (FK), Title, Description, Amount, Category, Date, CurrencyId, ExchangeRate, CreatedAt |
| **Incomes** | Id, UserId (FK), Title, Description, Amount, Source, Date, CurrencyId, ExchangeRate, CreatedAt |
| **Budgets** | Id, UserId (FK), Month, Year, Category (nullable), Amount |
| **Goals** | Id, UserId (FK), Title, TargetAmount, CurrentAmount, TargetDate, CreatedAt |
| **RecurringTransactions** | Id, UserId (FK), Title, Amount, Type, Frequency, NextRunAt, LastRunAt, EndDate |
| **Tags** | Id, UserId (FK), Name, Color |
| **ExpenseTags** | ExpenseId (FK), TagId (FK) — junction table |
| **IncomeTags** | IncomeId (FK), TagId (FK) — junction table |
| **Attachments** | Id, ExpenseId (FK), FileName, FilePath, ContentType, FileSize, UploadedAt |
| **Notifications** | Id, UserId (FK), Type, Message, IsRead, CreatedAt, ReadAt |
| **Currencies** | Id, Code, Name, Symbol, ExchangeRateToUsd |

---

## Usage Guide

### Expenses & Income
1. Click **Add Expense** or **Add Income** in the navigation bar
2. Fill in the required fields (title, amount, category/source, date)
3. Optionally add a description, tags, currency, and file attachment
4. Click **Save** to create the record

### Budgets
1. Go to **Budgets** → **Add Budget**
2. Select the month/year and optionally a specific category
3. Enter the spending limit and save
4. The dashboard and budget list show how much of each budget has been used

### Financial Goals
1. Go to **Goals** → **Add Goal**
2. Set a title, target amount, and target date
3. Update **Current Amount** as you progress toward your goal
4. A progress bar shows how close you are

### Recurring Transactions
1. Go to **Recurring** → **Add Recurring**
2. Choose type (Expense or Income), frequency, start date, and optional end date
3. The background service creates the corresponding records automatically when they fall due

### Tags
1. Go to **Tags** → **Add Tag** to create custom tags with a color
2. Assign tags to expenses or income from the create/edit forms
3. Use tag filters on the Expenses or Income list pages

### Reports
- Navigate to **Reports** for a detailed breakdown by date range and category
- Export data as CSV if needed

### Admin Panel
- Accessible to users with the **Admin** role at `/Admin`
- View all users, all expenses, all income records, and manage supported currencies

### REST API
- API endpoints are available under `/api/`
- Obtain a JWT token via `POST /api/auth/login`
- Include the token in the `Authorization: Bearer <token>` header for subsequent requests
- Interactive API documentation is available at `/swagger` when running in Development mode

---

## Development

### Architecture

The application follows a layered architecture:

```
HTTP Request → Controller → Service → DbContext → PostgreSQL
```

1. **Controllers** — handle HTTP requests, validate input, call services
2. **Services** — contain business logic, interact with `ApplicationDbContext`
3. **Models** — EF Core entities mapped to database tables
4. **ViewModels** — data shapes for Razor views
5. **Background Service** — `RecurringTransactionHostedService` polls for due recurring transactions

### Entity Framework Migrations

```bash
# Add a new migration after changing models
dotnet ef migrations add MigrationName

# Apply pending migrations
dotnet ef database update

# Revert last unapplied migration
dotnet ef migrations remove

# List all migrations
dotnet ef migrations list

# Generate SQL script
dotnet ef migrations script

# Drop the database (use with caution)
dotnet ef database drop
```

### Environment-Specific Configuration

| File | Purpose |
|---|---|
| `appsettings.json` | Shared / production defaults |
| `appsettings.Development.json` | Development overrides (enables Swagger, seed data) |
| `appsettings.Production.json` | Create this file to override secrets in production |

---

## Configuration Reference

### Connection String

```
Host=localhost;Port=5432;Database=ExpenseTrackerDb;Username=postgres;Password=your_password;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;
```

### JWT Settings

```json
{
  "Jwt": {
    "Key": "<32+ character secret key>",
    "Issuer": "ExpenseTracker",
    "Audience": "ExpenseTrackerUsers"
  }
}
```

### File Upload Settings

```json
{
  "FileUpload": {
    "MaxSizeBytes": 5242880,
    "AllowedTypes": "image/jpeg,image/png,image/gif,application/pdf"
  }
}
```

---

## Troubleshooting

### Database Connection Issues

```bash
# Check PostgreSQL is running (Linux)
systemctl status postgresql

# Check PostgreSQL is running (Windows)
# Open Services and look for "postgresql-xx"
```

1. Verify the connection string credentials in `appsettings.json`
2. Confirm PostgreSQL is listening on the configured port
3. Check firewall rules allow the connection

### Migration Issues

```bash
# Reset and re-apply migrations
dotnet ef database drop
dotnet ef database update
```

### Port Already in Use

```bash
dotnet run --urls="https://localhost:5501;http://localhost:5500"
```

---

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/AmazingFeature`
3. Commit your changes: `git commit -m 'Add AmazingFeature'`
4. Push the branch: `git push origin feature/AmazingFeature`
5. Open a Pull Request

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## Acknowledgments

- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet) — web framework
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) — ORM
- [PostgreSQL](https://www.postgresql.org/) — database
- [Bootstrap 5](https://getbootstrap.com/) — UI framework
- [Chart.js](https://www.chartjs.org/) — charts
- [Font Awesome](https://fontawesome.com/) — icons
- [Swashbuckle / Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) — API documentation
- [CsvHelper](https://joshclose.github.io/CsvHelper/) — CSV export

---

## Contact

Project Link: [https://github.com/Parth5522/Expense-Tracker](https://github.com/Parth5522/Expense-Tracker)

---

**Happy Tracking! 💰📊**