using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class updateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "Users",
                newName: "nickname");

            migrationBuilder.AddColumn<string>(
                name: "ipaddress",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "login",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ipaddress",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "login",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "Users",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "nickname",
                table: "Users",
                newName: "DisplayName");
        }
    }
}
