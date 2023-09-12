using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixOrderPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LandryItem_Order_OrderId",
                table: "LandryItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTimeline_Account_AccountId",
                table: "OrderTimeline");

            migrationBuilder.DropIndex(
                name: "IX_OrderTimeline_AccountId",
                table: "OrderTimeline");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "OrderTimeline");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "LandryItem",
                newName: "OrderDetailId");

            migrationBuilder.RenameIndex(
                name: "IX_LandryItem_OrderId",
                table: "LandryItem",
                newName: "IX_LandryItem_OrderDetailId");

            migrationBuilder.AddColumn<long>(
                name: "StaffId",
                table: "OrderTimeline",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "OrderDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DetailId",
                table: "LandryItem",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "Prepaid",
                table: "Bill",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Account",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTimeline_StaffId",
                table: "OrderTimeline",
                column: "StaffId");

            migrationBuilder.AddForeignKey(
                name: "FK_LandryItem_OrderDetail_OrderDetailId",
                table: "LandryItem",
                column: "OrderDetailId",
                principalTable: "OrderDetail",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTimeline_Account_StaffId",
                table: "OrderTimeline",
                column: "StaffId",
                principalTable: "Account",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LandryItem_OrderDetail_OrderDetailId",
                table: "LandryItem");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderTimeline_Account_StaffId",
                table: "OrderTimeline");

            migrationBuilder.DropIndex(
                name: "IX_OrderTimeline_StaffId",
                table: "OrderTimeline");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "OrderTimeline");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "DetailId",
                table: "LandryItem");

            migrationBuilder.DropColumn(
                name: "Prepaid",
                table: "Bill");

            migrationBuilder.RenameColumn(
                name: "OrderDetailId",
                table: "LandryItem",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_LandryItem_OrderDetailId",
                table: "LandryItem",
                newName: "IX_LandryItem_OrderId");

            migrationBuilder.AddColumn<long>(
                name: "AccountId",
                table: "OrderTimeline",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "Account",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderTimeline_AccountId",
                table: "OrderTimeline",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_LandryItem_Order_OrderId",
                table: "LandryItem",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderTimeline_Account_AccountId",
                table: "OrderTimeline",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
