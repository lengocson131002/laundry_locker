using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditLogTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Token",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "Token",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "Token",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Store",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "Store",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "Store",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "StaffLocker",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "StaffLocker",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "StaffLocker",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Setting",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "Setting",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "Setting",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Service",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "Service",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "Service",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "OrderTimeline",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "OrderTimeline",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "OrderTimeline",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "OrderDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "OrderDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "OrderDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Order",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "Order",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingFee",
                table: "Order",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "Order",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "Notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "Notification",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "LockerTimeline",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "LockerTimeline",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "LockerTimeline",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Locker",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "Locker",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "Locker",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Location",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "Location",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "Location",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "LandryItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "LandryItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "LandryItem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Hardware",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "Hardware",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "Hardware",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Box",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "Box",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "Box",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Bill",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "Bill",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "Bill",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Account",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "Account",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "Account",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Audit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<long>(type: "bigint", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    TableName = table.Column<string>(type: "text", nullable: false),
                    Time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OldValues = table.Column<string>(type: "text", nullable: true),
                    NewValues = table.Column<string>(type: "text", nullable: true),
                    AffectedColumns = table.Column<string>(type: "text", nullable: true),
                    PrimaryKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Audit");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Token");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "Token");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "Token");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "Store");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "StaffLocker");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "StaffLocker");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "StaffLocker");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "Setting");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "OrderTimeline");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "OrderTimeline");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "OrderTimeline");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ShippingFee",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "LockerTimeline");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "LockerTimeline");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "LockerTimeline");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Locker");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "Locker");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "Locker");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "LandryItem");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "LandryItem");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "LandryItem");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Hardware");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "Hardware");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "Hardware");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Box");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "Box");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "Box");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "Bill");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "Account");
        }
    }
}
