using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Wallet",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "Wallet",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Wallet",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Wallet",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeletedBy",
                table: "Wallet",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "Wallet",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "Wallet",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<long>(
                name: "UpdatedBy",
                table: "Wallet",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "Wallet",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "Wallet");
        }
    }
}
