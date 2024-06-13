using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations
{
    /// <inheritdoc />
    public partial class mb_last : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SenderUsername",
                table: "Messages",
                newName: "Sendernick");

            migrationBuilder.RenameColumn(
                name: "ReceiverUsername",
                table: "Messages",
                newName: "Receivernick");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sendernick",
                table: "Messages",
                newName: "SenderUsername");

            migrationBuilder.RenameColumn(
                name: "Receivernick",
                table: "Messages",
                newName: "ReceiverUsername");
        }
    }
}
