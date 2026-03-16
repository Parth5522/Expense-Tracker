using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data;

public static class SeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure database is created and migrations applied
        await context.Database.MigrateAsync();

        // Seed roles
        string[] roles = ["Admin", "User"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Seed admin user
        const string adminEmail = "admin@expensetracker.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                DisplayName = "Admin User",
                BaseCurrency = "USD",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
                await userManager.AddToRoleAsync(admin, "User");
            }
        }

        // Seed demo user
        const string demoEmail = "demo@expensetracker.com";
        if (await userManager.FindByEmailAsync(demoEmail) == null)
        {
            var demo = new ApplicationUser
            {
                UserName = demoEmail,
                Email = demoEmail,
                DisplayName = "Demo User",
                BaseCurrency = "USD",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(demo, "Demo@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(demo, "User");

                // Seed demo expenses
                var demoUser = await userManager.FindByEmailAsync(demoEmail);
                if (demoUser != null && !await context.Expenses.AnyAsync(e => e.UserId == demoUser.Id))
                {
                    var expenses = new[]
                    {
                        new Expense { Title = "Grocery Shopping", Amount = 125.50m, Category = ExpenseCategory.Food, Date = DateTime.UtcNow.AddDays(-5), UserId = demoUser.Id, Currency = "USD", ExchangeRate = 1, AmountInBaseCurrency = 125.50m, CreatedAt = DateTime.UtcNow },
                        new Expense { Title = "Netflix Subscription", Amount = 15.99m, Category = ExpenseCategory.Entertainment, Date = DateTime.UtcNow.AddDays(-3), UserId = demoUser.Id, Currency = "USD", ExchangeRate = 1, AmountInBaseCurrency = 15.99m, CreatedAt = DateTime.UtcNow },
                        new Expense { Title = "Monthly Rent", Amount = 1200m, Category = ExpenseCategory.Housing, Date = DateTime.UtcNow.AddDays(-1), UserId = demoUser.Id, Currency = "USD", ExchangeRate = 1, AmountInBaseCurrency = 1200m, CreatedAt = DateTime.UtcNow },
                        new Expense { Title = "Gas Station", Amount = 55m, Category = ExpenseCategory.Transportation, Date = DateTime.UtcNow.AddDays(-7), UserId = demoUser.Id, Currency = "USD", ExchangeRate = 1, AmountInBaseCurrency = 55m, CreatedAt = DateTime.UtcNow },
                    };
                    context.Expenses.AddRange(expenses);

                    var incomes = new[]
                    {
                        new Income { Title = "Monthly Salary", Amount = 5000m, Source = IncomeSource.Salary, Date = DateTime.UtcNow.AddDays(-1), UserId = demoUser.Id, Currency = "USD", ExchangeRate = 1, AmountInBaseCurrency = 5000m, CreatedAt = DateTime.UtcNow },
                        new Income { Title = "Freelance Project", Amount = 500m, Source = IncomeSource.Freelance, Date = DateTime.UtcNow.AddDays(-10), UserId = demoUser.Id, Currency = "USD", ExchangeRate = 1, AmountInBaseCurrency = 500m, CreatedAt = DateTime.UtcNow },
                    };
                    context.Incomes.AddRange(incomes);

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
