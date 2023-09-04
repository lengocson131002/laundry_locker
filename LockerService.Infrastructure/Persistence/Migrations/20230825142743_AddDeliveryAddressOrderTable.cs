using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryAddressOrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DeliveryAddressId",
                table: "Order",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_DeliveryAddressId",
                table: "Order",
                column: "DeliveryAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Location_DeliveryAddressId",
                table: "Order",
                column: "DeliveryAddressId",
                principalTable: "Location",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Location_DeliveryAddressId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_DeliveryAddressId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DeliveryAddressId",
                table: "Order");
        }
    }
}
