using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingAppllicaiton.Migrations
{
    /// <inheritdoc />
    public partial class addtableandremovecolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Credit",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Classes",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPersons",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "NumberOfPersons",
                table: "Classes");

            migrationBuilder.AddColumn<string>(
                name: "Credit",
                table: "User",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
