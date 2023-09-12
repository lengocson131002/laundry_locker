using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixAccountTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Token_DeletedAt",
                table: "Token",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Store_DeletedAt",
                table: "Store",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StaffLocker_DeletedAt",
                table: "StaffLocker",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Setting_DeletedAt",
                table: "Setting",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Service_DeletedAt",
                table: "Service",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTimeline_DeletedAt",
                table: "OrderTimeline",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_DeletedAt",
                table: "OrderDetail",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Order_DeletedAt",
                table: "Order",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_DeletedAt",
                table: "Notification",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LockerTimeline_DeletedAt",
                table: "LockerTimeline",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Locker_DeletedAt",
                table: "Locker",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Location_DeletedAt",
                table: "Location",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LandryItem_DeletedAt",
                table: "LandryItem",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Hardware_DeletedAt",
                table: "Hardware",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Box_DeletedAt",
                table: "Box",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_DeletedAt",
                table: "Bill",
                column: "DeletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Account_DeletedAt",
                table: "Account",
                column: "DeletedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Token_DeletedAt",
                table: "Token");

            migrationBuilder.DropIndex(
                name: "IX_Store_DeletedAt",
                table: "Store");

            migrationBuilder.DropIndex(
                name: "IX_StaffLocker_DeletedAt",
                table: "StaffLocker");

            migrationBuilder.DropIndex(
                name: "IX_Setting_DeletedAt",
                table: "Setting");

            migrationBuilder.DropIndex(
                name: "IX_Service_DeletedAt",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_OrderTimeline_DeletedAt",
                table: "OrderTimeline");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetail_DeletedAt",
                table: "OrderDetail");

            migrationBuilder.DropIndex(
                name: "IX_Order_DeletedAt",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Notification_DeletedAt",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_LockerTimeline_DeletedAt",
                table: "LockerTimeline");

            migrationBuilder.DropIndex(
                name: "IX_Locker_DeletedAt",
                table: "Locker");

            migrationBuilder.DropIndex(
                name: "IX_Location_DeletedAt",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_LandryItem_DeletedAt",
                table: "LandryItem");

            migrationBuilder.DropIndex(
                name: "IX_Hardware_DeletedAt",
                table: "Hardware");

            migrationBuilder.DropIndex(
                name: "IX_Box_DeletedAt",
                table: "Box");

            migrationBuilder.DropIndex(
                name: "IX_Bill_DeletedAt",
                table: "Bill");

            migrationBuilder.DropIndex(
                name: "IX_Account_DeletedAt",
                table: "Account");
        }
    }
}
