using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class _3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_UsersId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_UsersId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsersId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_UsersId",
                table: "Messages",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_UsersId",
                table: "Messages",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
