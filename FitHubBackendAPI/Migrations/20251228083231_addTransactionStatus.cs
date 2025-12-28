using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHubBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class addTransactionStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UserCreditTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 28, 8, 32, 30, 751, DateTimeKind.Utc).AddTicks(1328));

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 28, 8, 32, 30, 751, DateTimeKind.Utc).AddTicks(1335));

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 28, 8, 32, 30, 751, DateTimeKind.Utc).AddTicks(1336));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserCreditTransactions");

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
    }
}
