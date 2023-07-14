using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountLockerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoreId",
                table: "Locker",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountLocker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AccountId = table.Column<int>(type: "integer", nullable: false),
                    LockerId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountLocker", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountLocker_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountLocker_Locker_LockerId",
                        column: x => x.LockerId,
                        principalTable: "Locker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Locker_StoreId",
                table: "Locker",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLocker_AccountId",
                table: "AccountLocker",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLocker_LockerId",
                table: "AccountLocker",
                column: "LockerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Locker_Store_StoreId",
                table: "Locker",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locker_Store_StoreId",
                table: "Locker");

            migrationBuilder.DropTable(
                name: "AccountLocker");

            migrationBuilder.DropIndex(
                name: "IX_Locker_StoreId",
                table: "Locker");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Locker");
        }
    }
}
