using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineChess.Migrations
{
    /// <inheritdoc />
    public partial class UniqueIngame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_InGame",
                table: "AspNetUsers",
                column: "InGame",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_InGame",
                table: "AspNetUsers");
        }
    }
}
