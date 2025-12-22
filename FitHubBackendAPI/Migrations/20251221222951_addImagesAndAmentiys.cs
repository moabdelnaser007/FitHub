using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHubBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class addImagesAndAmentiys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "GymAmenities");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "GymBranches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkingDays",
                table: "GymBranches",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Amenity",
                table: "GymAmenities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    imagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GymId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsAcTive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Image_GymBranches_GymId",
                        column: x => x.GymId,
                        principalTable: "GymBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Image_GymId",
                table: "Image",
                column: "GymId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "GymBranches");

            migrationBuilder.DropColumn(
                name: "WorkingDays",
                table: "GymBranches");

            migrationBuilder.DropColumn(
                name: "Amenity",
                table: "GymAmenities");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "GymAmenities",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
