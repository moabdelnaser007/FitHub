using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHubBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class addAmenityEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AmenitiesAvailable",
                table: "GymBranches",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmenitiesAvailable",
                table: "GymBranches");
        }
    }
}
