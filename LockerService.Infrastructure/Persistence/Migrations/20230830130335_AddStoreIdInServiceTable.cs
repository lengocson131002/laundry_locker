using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LockerService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStoreIdInServiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StoreId",
                table: "Service",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Service_StoreId",
                table: "Service",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Service_Store_StoreId",
                table: "Service",
                column: "StoreId",
                principalTable: "Store",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Service_Store_StoreId",
                table: "Service");

            migrationBuilder.DropIndex(
                name: "IX_Service_StoreId",
                table: "Service");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Service");
        }
    }
}
