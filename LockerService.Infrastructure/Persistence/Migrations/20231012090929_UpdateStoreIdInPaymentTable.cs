using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStoreIdInPaymentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StoreId",
                table: "Payment",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_StoreId",
                table: "Payment",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Store_StoreId",
                table: "Payment",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Store_StoreId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_StoreId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Payment");
        }
    }
}
