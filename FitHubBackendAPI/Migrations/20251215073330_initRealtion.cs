using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHubBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class initRealtion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "GymStaffs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GymStaffs_UserId",
                table: "GymStaffs",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GymStaffs_Users_UserId",
                table: "GymStaffs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GymStaffs_Users_UserId",
                table: "GymStaffs");

            migrationBuilder.DropIndex(
                name: "IX_GymStaffs_UserId",
                table: "GymStaffs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "GymStaffs");
        }
    }
}
