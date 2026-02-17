using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Expenses",
                columns: new[] { "Id", "Amount", "Category", "CreatedAt", "Date", "Description", "Title" },
                values: new object[,]
                {
                    { 1, 125.50m, 0, new DateTime(2026, 2, 12, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5273), new DateTime(2026, 2, 12, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5268), "Weekly groceries from supermarket", "Grocery Shopping" },
                    { 2, 45.00m, 1, new DateTime(2026, 2, 13, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5278), new DateTime(2026, 2, 13, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5278), "Fuel for car", "Gas Station" },
                    { 3, 1200.00m, 2, new DateTime(2026, 2, 14, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5282), new DateTime(2026, 2, 14, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5281), "Apartment rent for December", "Monthly Rent" },
                    { 4, 85.30m, 3, new DateTime(2026, 2, 15, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5284), new DateTime(2026, 2, 15, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5284), "Monthly electricity payment", "Electricity Bill" },
                    { 5, 32.00m, 4, new DateTime(2026, 2, 16, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5287), new DateTime(2026, 2, 16, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5287), "Cinema tickets for 2", "Movie Tickets" },
                    { 6, 150.00m, 5, new DateTime(2026, 2, 10, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5291), new DateTime(2026, 2, 10, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5291), "General checkup", "Doctor Visit" },
                    { 7, 89.99m, 6, new DateTime(2026, 2, 11, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5294), new DateTime(2026, 2, 11, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5294), "Running shoes from sports store", "New Shoes" },
                    { 8, 49.99m, 7, new DateTime(2026, 2, 7, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5297), new DateTime(2026, 2, 7, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5296), "Programming course subscription", "Online Course" },
                    { 9, 320.00m, 8, new DateTime(2026, 2, 2, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5300), new DateTime(2026, 2, 2, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5299), "Weekend getaway", "Hotel Stay" },
                    { 10, 5.50m, 0, new DateTime(2026, 2, 17, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5303), new DateTime(2026, 2, 17, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5303), "Morning coffee", "Coffee Shop" },
                    { 11, 12.50m, 1, new DateTime(2026, 2, 9, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5306), new DateTime(2026, 2, 9, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5305), "Ride to office", "Uber Ride" },
                    { 12, 59.99m, 3, new DateTime(2026, 2, 8, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5309), new DateTime(2026, 2, 8, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5308), "Monthly internet service", "Internet Bill" },
                    { 13, 75.00m, 4, new DateTime(2026, 2, 5, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5311), new DateTime(2026, 2, 5, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5311), "Live music event", "Concert Tickets" },
                    { 14, 28.50m, 5, new DateTime(2026, 2, 6, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5314), new DateTime(2026, 2, 6, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5313), "Prescription medicines", "Pharmacy" },
                    { 15, 129.99m, 6, new DateTime(2026, 2, 4, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5317), new DateTime(2026, 2, 4, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5316), "New winter jacket", "Clothing Store" },
                    { 16, 65.00m, 7, new DateTime(2026, 2, 3, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5320), new DateTime(2026, 2, 3, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5319), "Educational textbooks", "Book Purchase" },
                    { 17, 450.00m, 8, new DateTime(2026, 1, 28, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5323), new DateTime(2026, 1, 28, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5322), "Business trip", "Flight Ticket" },
                    { 18, 95.00m, 0, new DateTime(2026, 1, 30, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5326), new DateTime(2026, 1, 30, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5326), "Family dinner", "Restaurant Dinner" },
                    { 19, 8.00m, 1, new DateTime(2026, 2, 1, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5329), new DateTime(2026, 2, 1, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5329), "Mall parking", "Parking Fee" },
                    { 20, 35.00m, 3, new DateTime(2026, 1, 31, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5332), new DateTime(2026, 1, 31, 10, 56, 40, 950, DateTimeKind.Utc).AddTicks(5331), "Monthly water service", "Water Bill" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");
        }
    }
}
