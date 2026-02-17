using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data
{
    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Expense entity
            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Amount).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.Category).IsRequired();
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
            });

            // Seed sample data
            var sampleExpenses = new List<Expense>
            {
                new Expense { Id = 1, Title = "Grocery Shopping", Description = "Weekly groceries from supermarket", Amount = 125.50m, Category = ExpenseCategory.Food, Date = DateTime.UtcNow.AddDays(-5), CreatedAt = DateTime.UtcNow.AddDays(-5) },
                new Expense { Id = 2, Title = "Gas Station", Description = "Fuel for car", Amount = 45.00m, Category = ExpenseCategory.Transportation, Date = DateTime.UtcNow.AddDays(-4), CreatedAt = DateTime.UtcNow.AddDays(-4) },
                new Expense { Id = 3, Title = "Monthly Rent", Description = "Apartment rent for December", Amount = 1200.00m, Category = ExpenseCategory.Housing, Date = DateTime.UtcNow.AddDays(-3), CreatedAt = DateTime.UtcNow.AddDays(-3) },
                new Expense { Id = 4, Title = "Electricity Bill", Description = "Monthly electricity payment", Amount = 85.30m, Category = ExpenseCategory.Utilities, Date = DateTime.UtcNow.AddDays(-2), CreatedAt = DateTime.UtcNow.AddDays(-2) },
                new Expense { Id = 5, Title = "Movie Tickets", Description = "Cinema tickets for 2", Amount = 32.00m, Category = ExpenseCategory.Entertainment, Date = DateTime.UtcNow.AddDays(-1), CreatedAt = DateTime.UtcNow.AddDays(-1) },
                new Expense { Id = 6, Title = "Doctor Visit", Description = "General checkup", Amount = 150.00m, Category = ExpenseCategory.Healthcare, Date = DateTime.UtcNow.AddDays(-7), CreatedAt = DateTime.UtcNow.AddDays(-7) },
                new Expense { Id = 7, Title = "New Shoes", Description = "Running shoes from sports store", Amount = 89.99m, Category = ExpenseCategory.Shopping, Date = DateTime.UtcNow.AddDays(-6), CreatedAt = DateTime.UtcNow.AddDays(-6) },
                new Expense { Id = 8, Title = "Online Course", Description = "Programming course subscription", Amount = 49.99m, Category = ExpenseCategory.Education, Date = DateTime.UtcNow.AddDays(-10), CreatedAt = DateTime.UtcNow.AddDays(-10) },
                new Expense { Id = 9, Title = "Hotel Stay", Description = "Weekend getaway", Amount = 320.00m, Category = ExpenseCategory.Travel, Date = DateTime.UtcNow.AddDays(-15), CreatedAt = DateTime.UtcNow.AddDays(-15) },
                new Expense { Id = 10, Title = "Coffee Shop", Description = "Morning coffee", Amount = 5.50m, Category = ExpenseCategory.Food, Date = DateTime.UtcNow, CreatedAt = DateTime.UtcNow },
                new Expense { Id = 11, Title = "Uber Ride", Description = "Ride to office", Amount = 12.50m, Category = ExpenseCategory.Transportation, Date = DateTime.UtcNow.AddDays(-8), CreatedAt = DateTime.UtcNow.AddDays(-8) },
                new Expense { Id = 12, Title = "Internet Bill", Description = "Monthly internet service", Amount = 59.99m, Category = ExpenseCategory.Utilities, Date = DateTime.UtcNow.AddDays(-9), CreatedAt = DateTime.UtcNow.AddDays(-9) },
                new Expense { Id = 13, Title = "Concert Tickets", Description = "Live music event", Amount = 75.00m, Category = ExpenseCategory.Entertainment, Date = DateTime.UtcNow.AddDays(-12), CreatedAt = DateTime.UtcNow.AddDays(-12) },
                new Expense { Id = 14, Title = "Pharmacy", Description = "Prescription medicines", Amount = 28.50m, Category = ExpenseCategory.Healthcare, Date = DateTime.UtcNow.AddDays(-11), CreatedAt = DateTime.UtcNow.AddDays(-11) },
                new Expense { Id = 15, Title = "Clothing Store", Description = "New winter jacket", Amount = 129.99m, Category = ExpenseCategory.Shopping, Date = DateTime.UtcNow.AddDays(-13), CreatedAt = DateTime.UtcNow.AddDays(-13) },
                new Expense { Id = 16, Title = "Book Purchase", Description = "Educational textbooks", Amount = 65.00m, Category = ExpenseCategory.Education, Date = DateTime.UtcNow.AddDays(-14), CreatedAt = DateTime.UtcNow.AddDays(-14) },
                new Expense { Id = 17, Title = "Flight Ticket", Description = "Business trip", Amount = 450.00m, Category = ExpenseCategory.Travel, Date = DateTime.UtcNow.AddDays(-20), CreatedAt = DateTime.UtcNow.AddDays(-20) },
                new Expense { Id = 18, Title = "Restaurant Dinner", Description = "Family dinner", Amount = 95.00m, Category = ExpenseCategory.Food, Date = DateTime.UtcNow.AddDays(-18), CreatedAt = DateTime.UtcNow.AddDays(-18) },
                new Expense { Id = 19, Title = "Parking Fee", Description = "Mall parking", Amount = 8.00m, Category = ExpenseCategory.Transportation, Date = DateTime.UtcNow.AddDays(-16), CreatedAt = DateTime.UtcNow.AddDays(-16) },
                new Expense { Id = 20, Title = "Water Bill", Description = "Monthly water service", Amount = 35.00m, Category = ExpenseCategory.Utilities, Date = DateTime.UtcNow.AddDays(-17), CreatedAt = DateTime.UtcNow.AddDays(-17) }
            };

            modelBuilder.Entity<Expense>().HasData(sampleExpenses);
        }
    }
}
