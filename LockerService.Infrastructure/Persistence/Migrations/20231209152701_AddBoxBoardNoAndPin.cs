using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBoxBoardNoAndPin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BoardNo",
                table: "Box",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pin",
                table: "Box",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoardNo",
                table: "Box");

            migrationBuilder.DropColumn(
                name: "Pin",
                table: "Box");
        }
    }
}
