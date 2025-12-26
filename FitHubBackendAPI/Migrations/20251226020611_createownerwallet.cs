using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHubBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class createownerwallet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "OwnerWallets",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 26, 2, 6, 9, 846, DateTimeKind.Utc).AddTicks(9750));

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 26, 2, 6, 9, 846, DateTimeKind.Utc).AddTicks(9758));

            migrationBuilder.UpdateData(
                table: "FithubPlans",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 12, 26, 2, 6, 9, 846, DateTimeKind.Utc).AddTicks(9764));

            migrationBuilder.CreateIndex(
                name: "IX_OwnerWallets_UserId",
                table: "OwnerWallets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OwnerWallets_Users_UserId",
                table: "OwnerWallets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OwnerWallets_Users_UserId",
                table: "OwnerWallets");

            migrationBuilder.DropIndex(
                name: "IX_OwnerWallets_UserId",
                table: "OwnerWallets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OwnerWallets");

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
    }
}
