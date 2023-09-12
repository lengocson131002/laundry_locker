using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixLaundryItemForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailId",
                table: "LandryItem");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "IntendedOvertime",
                table: "Order",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "IntendedOvertime",
                table: "Order",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DetailId",
                table: "LandryItem",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}
