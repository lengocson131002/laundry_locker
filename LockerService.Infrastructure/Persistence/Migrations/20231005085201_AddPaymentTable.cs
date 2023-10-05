using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Bill_BillId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Box_ReceiveBoxId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_BillId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "BillId",
                table: "Order");

            migrationBuilder.AlterColumn<long>(
                name: "ReceiveBoxId",
                table: "Order",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    ReferenceTransactionId = table.Column<string>(type: "text", nullable: true),
                    Qr = table.Column<string>(type: "text", nullable: true),
                    Url = table.Column<string>(type: "text", nullable: true),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Account_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payment_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_CustomerId",
                table: "Payment",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_OrderId",
                table: "Payment",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Box_ReceiveBoxId",
                table: "Order",
                column: "ReceiveBoxId",
                principalTable: "Box",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Box_ReceiveBoxId",
                table: "Order");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.AlterColumn<long>(
                name: "ReceiveBoxId",
                table: "Order",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BillId",
                table: "Order",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_BillId",
                table: "Order",
                column: "BillId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Bill_BillId",
                table: "Order",
                column: "BillId",
                principalTable: "Bill",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Box_ReceiveBoxId",
                table: "Order",
                column: "ReceiveBoxId",
                principalTable: "Box",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
