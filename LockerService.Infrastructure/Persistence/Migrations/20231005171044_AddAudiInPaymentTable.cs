using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAudiInPaymentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Payment",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "Payment",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Payment",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeletedBy",
                table: "Payment",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "Payment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Payment",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "Payment",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "Payment",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "Payment");
        }
    }
}
