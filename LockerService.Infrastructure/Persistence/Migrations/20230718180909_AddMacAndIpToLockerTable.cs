using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMacAndIpToLockerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "Locker",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MacAddress",
                table: "Locker",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderCount",
                table: "Account",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountLocker");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "Locker");

            migrationBuilder.DropColumn(
                name: "MacAddress",
                table: "Locker");

            migrationBuilder.DropColumn(
                name: "OrderCount",
                table: "Account");
        }
    }
}
