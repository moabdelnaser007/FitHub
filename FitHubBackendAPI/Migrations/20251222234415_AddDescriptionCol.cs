using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHubBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UserCreditTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 22, 23, 44, 11, 684, DateTimeKind.Utc).AddTicks(7797));

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 22, 23, 44, 11, 684, DateTimeKind.Utc).AddTicks(7805));

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 22, 23, 44, 11, 684, DateTimeKind.Utc).AddTicks(7811));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "UserCreditTransactions");

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 22, 22, 59, 11, 690, DateTimeKind.Utc).AddTicks(680));

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 22, 22, 59, 11, 690, DateTimeKind.Utc).AddTicks(688));

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 22, 22, 59, 11, 690, DateTimeKind.Utc).AddTicks(694));
        }
    }
}
