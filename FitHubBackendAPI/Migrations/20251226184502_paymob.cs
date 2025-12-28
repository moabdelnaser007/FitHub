using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHubBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class paymob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "UserCreditTransactions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "UserCreditTransactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentKey",
                table: "UserCreditTransactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 26, 18, 45, 1, 546, DateTimeKind.Utc).AddTicks(4556));

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 26, 18, 45, 1, 546, DateTimeKind.Utc).AddTicks(4563));

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 26, 18, 45, 1, 546, DateTimeKind.Utc).AddTicks(4565));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "UserCreditTransactions");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "UserCreditTransactions");

            migrationBuilder.DropColumn(
                name: "PaymentKey",
                table: "UserCreditTransactions");

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 26, 5, 25, 43, 914, DateTimeKind.Utc).AddTicks(7883));

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 26, 5, 25, 43, 914, DateTimeKind.Utc).AddTicks(7892));

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 26, 5, 25, 43, 914, DateTimeKind.Utc).AddTicks(7894));
        }
    }
}
