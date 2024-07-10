using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineChess.Migrations
{
    /// <inheritdoc />
    public partial class MatchSummaryAdjust : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchSummaries_AspNetUsers_WinnerId",
                table: "MatchSummaries");

            migrationBuilder.DropIndex(
                name: "IX_MatchSummaries_WinnerId",
                table: "MatchSummaries");

            migrationBuilder.DropColumn(
                name: "WinnerId",
                table: "MatchSummaries");

            migrationBuilder.AddColumn<int>(
                name: "Result",
                table: "MatchSummaries",
                type: "int",
                maxLength: 1,
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "MatchSummaries");

            migrationBuilder.AddColumn<string>(
                name: "WinnerId",
                table: "MatchSummaries",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MatchSummaries_WinnerId",
                table: "MatchSummaries",
                column: "WinnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchSummaries_AspNetUsers_WinnerId",
                table: "MatchSummaries",
                column: "WinnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
