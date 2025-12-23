using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FitHubBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class ADDFITHUBPLANSMODELS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FithubPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditsValue = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsAcTive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FithubPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FithubUserPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsAcTive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FithubUserPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FithubUserPlans_FithubPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "FithubPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FithubUserPlans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FithubPlans",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "CreditsValue", "Description", "IsAcTive", "IsDeleted", "Name", "Price", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 22, 22, 59, 11, 690, DateTimeKind.Utc).AddTicks(680), null, 250, "Perfect for starters", true, false, "Basic", 250m, null, null },
                    { 2, new DateTime(2025, 12, 22, 22, 59, 11, 690, DateTimeKind.Utc).AddTicks(688), null, 500, "Most Popular Choice", true, false, "Premium", 500m, null, null },
                    { 3, new DateTime(2025, 12, 22, 22, 59, 11, 690, DateTimeKind.Utc).AddTicks(694), null, 800, "Best Value for Pros", true, false, "Gold", 800m, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FithubUserPlans_PlanId",
                table: "FithubUserPlans",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_FithubUserPlans_UserId",
                table: "FithubUserPlans",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FithubUserPlans");

            migrationBuilder.DropTable(
                name: "FithubPlans");
        }
    }
}
