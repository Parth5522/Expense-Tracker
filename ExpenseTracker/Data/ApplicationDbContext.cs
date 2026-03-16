using ExpenseTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Income> Incomes { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ExpenseTag> ExpenseTags { get; set; }
    public DbSet<IncomeTag> IncomeTags { get; set; }
    public DbSet<RecurringTransaction> RecurringTransactions { get; set; }
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Currency> Currencies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Expense
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.AmountInBaseCurrency).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ExchangeRate).HasColumnType("decimal(18,6)");
            entity.Property(e => e.Currency).HasMaxLength(3);
            entity.HasOne(e => e.User).WithMany(u => u.Expenses).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Income
        modelBuilder.Entity<Income>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Title).IsRequired().HasMaxLength(200);
            entity.Property(i => i.Description).HasMaxLength(1000);
            entity.Property(i => i.Amount).HasColumnType("decimal(18,2)");
            entity.Property(i => i.AmountInBaseCurrency).HasColumnType("decimal(18,2)");
            entity.Property(i => i.ExchangeRate).HasColumnType("decimal(18,6)");
            entity.Property(i => i.Currency).HasMaxLength(3);
            entity.HasOne(i => i.User).WithMany(u => u.Incomes).HasForeignKey(i => i.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Budget
        modelBuilder.Entity<Budget>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Amount).HasColumnType("decimal(18,2)");
            entity.HasOne(b => b.User).WithMany(u => u.Budgets).HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Tag
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(50);
        });

        // ExpenseTag (junction)
        modelBuilder.Entity<ExpenseTag>(entity =>
        {
            entity.HasKey(et => new { et.ExpenseId, et.TagId });
            entity.HasOne(et => et.Expense).WithMany(e => e.ExpenseTags).HasForeignKey(et => et.ExpenseId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(et => et.Tag).WithMany(t => t.ExpenseTags).HasForeignKey(et => et.TagId).OnDelete(DeleteBehavior.Cascade);
        });

        // IncomeTag (junction)
        modelBuilder.Entity<IncomeTag>(entity =>
        {
            entity.HasKey(it => new { it.IncomeId, it.TagId });
            entity.HasOne(it => it.Income).WithMany(i => i.IncomeTags).HasForeignKey(it => it.IncomeId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(it => it.Tag).WithMany(t => t.IncomeTags).HasForeignKey(it => it.TagId).OnDelete(DeleteBehavior.Cascade);
        });

        // RecurringTransaction
        modelBuilder.Entity<RecurringTransaction>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Amount).HasColumnType("decimal(18,2)");
            entity.HasOne(r => r.User).WithMany(u => u.RecurringTransactions).HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Attachment
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.HasOne(a => a.Expense).WithMany(e => e.Attachments).HasForeignKey(a => a.ExpenseId).OnDelete(DeleteBehavior.SetNull);
        });

        // Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.Id);
            entity.HasOne(n => n.User).WithMany(u => u.Notifications).HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        // Goal
        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.TargetAmount).HasColumnType("decimal(18,2)");
            entity.Property(g => g.CurrentAmount).HasColumnType("decimal(18,2)");
            entity.HasOne(g => g.User).WithMany(u => u.Goals).HasForeignKey(g => g.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.Ignore(g => g.ProgressPercentage);
            entity.Ignore(g => g.RemainingAmount);
        });

        // Currency
        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(c => c.Code);
            entity.Property(c => c.RateToUsd).HasColumnType("decimal(18,6)");
        });

        // Seed currencies
        modelBuilder.Entity<Currency>().HasData(
            new Currency { Code = "USD", Name = "US Dollar", Symbol = "$", RateToUsd = 1m, UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Currency { Code = "EUR", Name = "Euro", Symbol = "€", RateToUsd = 0.92m, UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Currency { Code = "GBP", Name = "British Pound", Symbol = "£", RateToUsd = 0.79m, UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Currency { Code = "INR", Name = "Indian Rupee", Symbol = "₹", RateToUsd = 83.12m, UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Currency { Code = "JPY", Name = "Japanese Yen", Symbol = "¥", RateToUsd = 149.50m, UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Currency { Code = "CAD", Name = "Canadian Dollar", Symbol = "CA$", RateToUsd = 1.36m, UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Currency { Code = "AUD", Name = "Australian Dollar", Symbol = "A$", RateToUsd = 1.53m, UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
