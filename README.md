# Expense Tracker

A complete, modern expense tracking web application built with ASP.NET Core MVC and PostgreSQL. Track your spending, visualize your expenses with beautiful charts, and gain insights into your financial habits.

![Expense Tracker](https://via.placeholder.com/800x400/0d6efd/ffffff?text=Expense+Tracker+Dashboard)

## Features

### 💰 Expense Management
- **Full CRUD Operations**: Create, Read, Update, and Delete expenses
- **Rich Data Model**: Track title, description, amount, category, and date for each expense
- **10 Category Types**: Food, Transportation, Housing, Utilities, Entertainment, Healthcare, Shopping, Education, Travel, and Other
- **Form Validation**: Client and server-side validation to ensure data integrity

### 📊 Dashboard & Analytics
- **Summary Cards**: View total expenses, monthly expenses, average expense, and transaction count at a glance
- **Pie Chart**: Visual breakdown of expenses by category
- **Bar Chart**: Monthly spending trends for the last 6 months
- **Recent Transactions**: Quick view of your latest 5 expenses

### 🔍 Filtering & Search
- **Date Range Filtering**: Filter expenses between specific dates
- **Category Filtering**: View expenses by category
- **Text Search**: Search expenses by title or description
- **Combined Filters**: Use multiple filters simultaneously
- **Pagination**: Navigate through large sets of expenses easily

### 🎨 Modern UI/UX
- **Bootstrap 5**: Responsive, mobile-first design
- **Font Awesome Icons**: Beautiful icons throughout the interface
- **Custom CSS**: Polished, professional color scheme and styling
- **Smooth Animations**: Hover effects, transitions, and fade-in animations
- **Toast Notifications**: Success and error messages for user actions
- **Responsive Design**: Works seamlessly on desktop, tablet, and mobile devices

## Tech Stack

- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: PostgreSQL with Entity Framework Core
- **ORM**: Entity Framework Core with Npgsql provider
- **Frontend**: Razor Views, Bootstrap 5, HTML5, CSS3
- **Charts**: Chart.js 4.4.0
- **Icons**: Font Awesome 6.4.0
- **Architecture**: Layered architecture (Controllers → Services → Repository/DbContext → Database)

## Prerequisites

Before running this application, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [PostgreSQL](https://www.postgresql.org/download/) (version 12 or later recommended)
- A code editor like [Visual Studio](https://visualstudio.microsoft.com/), [Visual Studio Code](https://code.visualstudio.com/), or [JetBrains Rider](https://www.jetbrains.com/rider/)

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/Parth5522/Expense-Tracker.git
cd Expense-Tracker/ExpenseTracker
```

### 2. Configure PostgreSQL Connection

Open `appsettings.json` and update the connection string with your PostgreSQL credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ExpenseTrackerDb;Username=postgres;Password=your_password_here"
  }
}
```

Replace:
- `localhost` with your PostgreSQL host (if different)
- `5432` with your PostgreSQL port (if different)
- `ExpenseTrackerDb` with your desired database name
- `postgres` with your PostgreSQL username
- `your_password_here` with your PostgreSQL password

### 3. Install Dependencies

```bash
dotnet restore
```

### 4. Apply Database Migrations

Create and apply the initial migration to set up your database:

```bash
# Create migration
dotnet ef migrations add InitialCreate

# Apply migration to database
dotnet ef database update
```

This will create the `ExpenseTrackerDb` database with the `Expenses` table and seed it with 20 sample expenses for demonstration.

### 5. Run the Application

```bash
dotnet run
```

Or, if using Visual Studio, press `F5` or click the "Run" button.

The application will start and be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

Open your browser and navigate to one of these URLs to start using the Expense Tracker!

### 6. Default Accounts

When running in development mode, the following seed accounts are created automatically:

| Role  | Email                       | Password   |
|-------|-----------------------------|------------|
| Admin | admin@expensetracker.com    | Admin@123  |
| Demo  | demo@expensetracker.com     | Demo@123   |

The **Admin** account has full access to the admin panel (`/Admin`), where you can manage users, roles, and currencies.

You can override these defaults by adding a `SeedCredentials` section to `appsettings.Development.json` **before** running the application for the first time:

```json
{
  "SeedCredentials": {
    "AdminEmail": "your-admin@example.com",
    "AdminPassword": "Y0ur$tr0ng@Passw0rd!",
    "DemoEmail": "your-demo@example.com",
    "DemoPassword": "Y0urD3m0@Passw0rd!"
  }
}
```

> **Note:** Seed accounts are only created once (on first startup). Changing `SeedCredentials` after the accounts already exist in the database has no effect.

## Project Structure

```
ExpenseTracker/
├── Controllers/
│   ├── HomeController.cs          # Dashboard and home page
│   └── ExpensesController.cs      # CRUD operations for expenses
├── Models/
│   ├── Expense.cs                 # Expense entity model
│   ├── ExpenseCategory.cs         # Category enumeration
│   └── ErrorViewModel.cs          # Error handling model
├── ViewModels/
│   ├── DashboardViewModel.cs      # Dashboard data aggregation
│   └── ExpenseFilterViewModel.cs  # Filtering and pagination
├── Data/
│   └── ExpenseDbContext.cs        # EF Core DbContext with seed data
├── Services/
│   ├── IExpenseService.cs         # Service interface
│   └── ExpenseService.cs          # Business logic implementation
├── Views/
│   ├── Home/
│   │   └── Index.cshtml           # Dashboard with charts
│   ├── Expenses/
│   │   ├── Index.cshtml           # List view with filters
│   │   ├── Create.cshtml          # Create expense form
│   │   ├── Edit.cshtml            # Edit expense form
│   │   ├── Details.cshtml         # Expense details view
│   │   └── Delete.cshtml          # Delete confirmation
│   └── Shared/
│       ├── _Layout.cshtml         # Main layout with navigation
│       └── _ValidationScriptsPartial.cshtml
├── wwwroot/
│   ├── css/
│   │   └── site.css               # Custom styles
│   └── js/
│       └── site.js                # Custom JavaScript
├── Program.cs                     # Application entry point
├── appsettings.json              # Configuration settings
└── README.md                      # This file
```

## Database Schema

### Expenses Table

| Column      | Type          | Description                          |
|-------------|---------------|--------------------------------------|
| Id          | INT (PK)      | Primary key                          |
| Title       | VARCHAR(200)  | Expense title (required)             |
| Description | VARCHAR(1000) | Optional description                 |
| Amount      | DECIMAL(18,2) | Expense amount (required)            |
| Category    | INT (ENUM)    | Category (0-9, required)             |
| Date        | TIMESTAMP     | Expense date (required)              |
| CreatedAt   | TIMESTAMP     | Record creation timestamp (required) |

## Usage Guide

### Adding a New Expense
1. Click "Add Expense" button in the navigation bar or dashboard
2. Fill in the expense details (title, amount, category, and date are required)
3. Click "Create Expense" to save

### Viewing Expenses
- Navigate to "Expenses" in the menu to see all expenses
- Use filters to narrow down results by date, category, or search term
- Click on any expense row for more details

### Editing an Expense
1. Go to the expense list or dashboard
2. Click the edit (pencil) icon for the expense you want to modify
3. Update the details and click "Update Expense"

### Deleting an Expense
1. Navigate to the expense you want to delete
2. Click the delete (trash) icon
3. Confirm the deletion on the confirmation page

### Dashboard Analytics
- View your spending summary at a glance
- Analyze spending patterns with the category pie chart
- Track monthly trends with the bar chart
- Review recent transactions

## Development

### Adding New Features

The application follows a layered architecture:

1. **Models**: Define your data entities in `Models/`
2. **Services**: Add business logic in `Services/`
3. **Controllers**: Handle HTTP requests in `Controllers/`
4. **Views**: Create UI in `Views/`

### Running Migrations

After making changes to models:

```bash
# Add a new migration
dotnet ef migrations add MigrationName

# Update the database
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove
```

### Entity Framework Core Commands

```bash
# List migrations
dotnet ef migrations list

# Script migration to SQL
dotnet ef migrations script

# Drop database
dotnet ef database drop
```

## Configuration

### Connection String Options

The PostgreSQL connection string supports various options:

```
Host=localhost;Port=5432;Database=ExpenseTrackerDb;Username=postgres;Password=your_password;Pooling=true;Minimum Pool Size=0;Maximum Pool Size=100;
```

Common options:
- `Host`: PostgreSQL server address
- `Port`: PostgreSQL server port (default: 5432)
- `Database`: Database name
- `Username`: PostgreSQL username
- `Password`: PostgreSQL password
- `Pooling`: Enable connection pooling (recommended: true)
- `SSL Mode`: SSL connection mode (Disable, Require, Prefer)
- `Timeout`: Connection timeout in seconds

### Environment-Specific Settings

For different environments, create:
- `appsettings.Development.json` for development
- `appsettings.Production.json` for production

## Troubleshooting

### Database Connection Issues

If you encounter "could not connect to server" errors:

1. Ensure PostgreSQL is running: `systemctl status postgresql` (Linux) or check Services (Windows)
2. Verify connection string credentials
3. Check PostgreSQL is listening on the correct port
4. Ensure firewall allows connections to PostgreSQL

### Migration Issues

If migrations fail:

1. Drop the database: `dotnet ef database drop`
2. Delete the `Migrations` folder
3. Create a new migration: `dotnet ef migrations add InitialCreate`
4. Apply migration: `dotnet ef database update`

### Port Already in Use

If the default port is in use:

```bash
# Run on a different port
dotnet run --urls="https://localhost:5501;http://localhost:5500"
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Built with [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
- UI powered by [Bootstrap 5](https://getbootstrap.com/)
- Charts by [Chart.js](https://www.chartjs.org/)
- Icons from [Font Awesome](https://fontawesome.com/)
- Database by [PostgreSQL](https://www.postgresql.org/)

## Screenshots

*Add your screenshots here after deployment*

### Dashboard
![Dashboard](https://via.placeholder.com/800x500/0d6efd/ffffff?text=Dashboard+View)

### Expense List
![Expense List](https://via.placeholder.com/800x500/198754/ffffff?text=Expense+List)

### Add Expense
![Add Expense](https://via.placeholder.com/800x500/ffc107/ffffff?text=Add+Expense+Form)

## Contact

Project Link: [https://github.com/Parth5522/Expense-Tracker](https://github.com/Parth5522/Expense-Tracker)

---

**Happy Tracking! 💰📊**