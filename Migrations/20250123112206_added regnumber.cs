using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FribergRentalCars.Migrations
{
    /// <inheritdoc />
    public partial class addedregnumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegNumber",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegNumber",
                table: "Cars");
        }
    }
}
