using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountTableStoreId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Account",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_StoreId",
                table: "Account",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Store_StoreId",
                table: "Account",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Store_StoreId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_StoreId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Account");
        }
    }
}
