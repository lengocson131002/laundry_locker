using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Account",
                newName: "Avatar");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Account",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Account",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Account");

            migrationBuilder.RenameColumn(
                name: "Avatar",
                table: "Account",
                newName: "Email");
        }
    }
}
