using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitHubBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class StoreFilesAsBytes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DropColumn(
                name: "DocumentUrl",
                table: "GymOwners");

            migrationBuilder.RenameColumn(
                name: "LicenseFileUrl",
                table: "GymOwners",
                newName: "LicenseFileType");

            migrationBuilder.AddColumn<byte[]>(
                name: "Document",
                table: "GymOwners",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "LicenseFile",
                table: "GymOwners",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Document",
                table: "GymOwners");

            migrationBuilder.DropColumn(
                name: "LicenseFile",
                table: "GymOwners");

            migrationBuilder.RenameColumn(
                name: "LicenseFileType",
                table: "GymOwners",
                newName: "LicenseFileUrl");

            migrationBuilder.AddColumn<string>(
                name: "DocumentUrl",
                table: "GymOwners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "City", "CreatedAt", "CreatedBy", "Email", "FullName", "IsAcTive", "IsDeleted", "PasswordHash", "Phone", "Role", "Status", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 100, "Cairo", new DateTime(2025, 12, 9, 20, 9, 50, 597, DateTimeKind.Utc).AddTicks(2964), null, "admin@fithub.com", "abdelnaser", null, null, "$2a$11$hgM0fnxUWL2W.ddCLVOuSe8kuW5Q4k.kulx8IH.2Cbtk.ux623K.O", "01122334455", 4, "Active", null, null });
        }
    }
}
