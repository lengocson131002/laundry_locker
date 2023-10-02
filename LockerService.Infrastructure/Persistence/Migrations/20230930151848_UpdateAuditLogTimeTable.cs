using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditLogTimeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Audit",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "Audit",
                newName: "UpdatedBy");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Audit",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<long>(
                name: "CreatedBy",
                table: "Audit",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUsername",
                table: "Audit",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Audit",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeletedBy",
                table: "Audit",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUsername",
                table: "Audit",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUsername",
                table: "Audit",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Audit");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Audit");

            migrationBuilder.DropColumn(
                name: "CreatedByUsername",
                table: "Audit");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Audit");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Audit");

            migrationBuilder.DropColumn(
                name: "DeletedByUsername",
                table: "Audit");

            migrationBuilder.DropColumn(
                name: "UpdatedByUsername",
                table: "Audit");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Audit",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Audit",
                newName: "Time");
        }
    }
}
