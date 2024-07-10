using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineChess.Migrations
{
    /// <inheritdoc />
    public partial class Ingame_and_MatchSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InGame",
                table: "AspNetUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MatchSummaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlackId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WhiteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WinnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchSummaries_AspNetUsers_BlackId",
                        column: x => x.BlackId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchSummaries_AspNetUsers_WhiteId",
                        column: x => x.WhiteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchSummaries_AspNetUsers_WinnerId",
                        column: x => x.WinnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchSummaries_BlackId",
                table: "MatchSummaries",
                column: "BlackId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchSummaries_WhiteId",
                table: "MatchSummaries",
                column: "WhiteId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchSummaries_WinnerId",
                table: "MatchSummaries",
                column: "WinnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchSummaries");

            migrationBuilder.DropColumn(
                name: "InGame",
                table: "AspNetUsers");
        }
    }
}
