using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FribergRentalCars.Migrations
{
    /// <inheritdoc />
    public partial class adminupd2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Admin",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Admin_AccountId",
                table: "Admin",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Admin_Accounts_AccountId",
                table: "Admin",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admin_Accounts_AccountId",
                table: "Admin");

            migrationBuilder.DropIndex(
                name: "IX_Admin_AccountId",
                table: "Admin");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Admin");
        }
    }
}
