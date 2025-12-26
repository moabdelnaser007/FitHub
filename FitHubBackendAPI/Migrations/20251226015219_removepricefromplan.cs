using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHubBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class removepricefromplan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 26, 1, 52, 17, 860, DateTimeKind.Utc).AddTicks(6265));

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 26, 1, 52, 17, 860, DateTimeKind.Utc).AddTicks(6273));

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 26, 1, 52, 17, 860, DateTimeKind.Utc).AddTicks(6275));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 26, 0, 28, 2, 501, DateTimeKind.Utc).AddTicks(2602));

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 26, 0, 28, 2, 501, DateTimeKind.Utc).AddTicks(2610));

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 26, 0, 28, 2, 501, DateTimeKind.Utc).AddTicks(2612));
        }
    }
}
