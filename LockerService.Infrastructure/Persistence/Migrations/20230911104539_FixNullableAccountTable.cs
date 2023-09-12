using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixNullableAccountTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountLocker");

            migrationBuilder.AddColumn<long>(
                name: "LockerId",
                table: "Account",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Account_LockerId",
                table: "Account",
                column: "LockerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Locker_LockerId",
                table: "Account",
                column: "LockerId",
                principalTable: "Locker",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Locker_LockerId",
                table: "Account");

            migrationBuilder.DropIndex(
                name: "IX_Account_LockerId",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "LockerId",
                table: "Account");

            migrationBuilder.CreateTable(
                name: "AccountLocker",
                columns: table => new
                {
                    LockersId = table.Column<long>(type: "bigint", nullable: false),
                    StaffsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountLocker", x => new { x.LockersId, x.StaffsId });
                    table.ForeignKey(
                        name: "FK_AccountLocker_Account_StaffsId",
                        column: x => x.StaffsId,
                        principalTable: "Account",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountLocker_Locker_LockersId",
                        column: x => x.LockersId,
                        principalTable: "Locker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountLocker_StaffsId",
                table: "AccountLocker",
                column: "StaffsId");
        }
    }
}
