using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalPriceToOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locker_Store_StoreId",
                table: "Locker");

            migrationBuilder.AddColumn<int>(
                name: "DeviceType",
                table: "Token",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Order",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<long>(
                name: "StoreId",
                table: "Locker",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Locker_Store_StoreId",
                table: "Locker",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locker_Store_StoreId",
                table: "Locker");

            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "Token");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Order",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "StoreId",
                table: "Locker",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Locker_Store_StoreId",
                table: "Locker",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id");
        }
    }
}
