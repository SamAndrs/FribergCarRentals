using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FribergRentalCars.Migrations
{
    /// <inheritdoc />
    public partial class bookingclassupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Adresses_AdressId",
                table: "Accounts");

            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "AdressId",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Adresses_AdressId",
                table: "Accounts",
                column: "AdressId",
                principalTable: "Adresses",
                principalColumn: "AdressId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Adresses_AdressId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "Bookings");

            migrationBuilder.AlterColumn<int>(
                name: "AdressId",
                table: "Accounts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Adresses_AdressId",
                table: "Accounts",
                column: "AdressId",
                principalTable: "Adresses",
                principalColumn: "AdressId");
        }
    }
}
