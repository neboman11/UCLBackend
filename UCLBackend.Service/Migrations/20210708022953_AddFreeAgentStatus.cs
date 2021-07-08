using Microsoft.EntityFrameworkCore.Migrations;

namespace UCLBackend.Service.Migrations
{
    public partial class AddFreeAgentStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roster_Players_PlayerAPlayerID",
                table: "Roster");

            migrationBuilder.DropForeignKey(
                name: "FK_Roster_Players_PlayerBPlayerID",
                table: "Roster");

            migrationBuilder.DropForeignKey(
                name: "FK_Roster_Players_PlayerCPlayerID",
                table: "Roster");

            migrationBuilder.DropForeignKey(
                name: "FK_Roster_Players_ReservePlayerID",
                table: "Roster");

            migrationBuilder.DropIndex(
                name: "IX_Roster_PlayerAPlayerID",
                table: "Roster");

            migrationBuilder.DropIndex(
                name: "IX_Roster_PlayerBPlayerID",
                table: "Roster");

            migrationBuilder.DropIndex(
                name: "IX_Roster_PlayerCPlayerID",
                table: "Roster");

            migrationBuilder.DropIndex(
                name: "IX_Roster_ReservePlayerID",
                table: "Roster");

            migrationBuilder.DropColumn(
                name: "PlayerAPlayerID",
                table: "Roster");

            migrationBuilder.DropColumn(
                name: "PlayerBPlayerID",
                table: "Roster");

            migrationBuilder.DropColumn(
                name: "PlayerCPlayerID",
                table: "Roster");

            migrationBuilder.DropColumn(
                name: "ReservePlayerID",
                table: "Roster");

            migrationBuilder.DropColumn(
                name: "League",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "Team",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "TimeZone",
                table: "Players");

            migrationBuilder.AddColumn<bool>(
                name: "IsFreeAgent",
                table: "Players",
                type: "tinyint(1)",
                nullable: true,
                defaultValue: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamID",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamID",
                table: "Players",
                column: "TeamID");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Roster_TeamID",
                table: "Players",
                column: "TeamID",
                principalTable: "Roster",
                principalColumn: "TeamID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Roster_TeamID",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_TeamID",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "IsFreeAgent",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "TeamID",
                table: "Players");

            migrationBuilder.AddColumn<string>(
                name: "PlayerAPlayerID",
                table: "Roster",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PlayerBPlayerID",
                table: "Roster",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PlayerCPlayerID",
                table: "Roster",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReservePlayerID",
                table: "Roster",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "League",
                table: "Players",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Team",
                table: "Players",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TimeZone",
                table: "Players",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Roster_PlayerAPlayerID",
                table: "Roster",
                column: "PlayerAPlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_Roster_PlayerBPlayerID",
                table: "Roster",
                column: "PlayerBPlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_Roster_PlayerCPlayerID",
                table: "Roster",
                column: "PlayerCPlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_Roster_ReservePlayerID",
                table: "Roster",
                column: "ReservePlayerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Roster_Players_PlayerAPlayerID",
                table: "Roster",
                column: "PlayerAPlayerID",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Roster_Players_PlayerBPlayerID",
                table: "Roster",
                column: "PlayerBPlayerID",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Roster_Players_PlayerCPlayerID",
                table: "Roster",
                column: "PlayerCPlayerID",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Roster_Players_ReservePlayerID",
                table: "Roster",
                column: "ReservePlayerID",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
