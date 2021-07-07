using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UCLBackend.Service.Migrations
{
    public partial class AddRosterTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Account_Player_PlayerID",
                table: "Account");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Player",
                table: "Player");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Account",
                table: "Account");

            migrationBuilder.RenameTable(
                name: "Player",
                newName: "Players");

            migrationBuilder.RenameTable(
                name: "Account",
                newName: "Accounts");

            migrationBuilder.RenameIndex(
                name: "IX_Account_PlayerID",
                table: "Accounts",
                newName: "IX_Accounts_PlayerID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Players",
                table: "Players",
                column: "PlayerID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                column: "AccountID");

            migrationBuilder.CreateTable(
                name: "Roster",
                columns: table => new
                {
                    TeamID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    League = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TeamName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Conference = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CapSpace = table.Column<double>(type: "double", nullable: false),
                    PlayerAPlayerID = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PlayerBPlayerID = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PlayerCPlayerID = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReservePlayerID = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roster", x => x.TeamID);
                    table.ForeignKey(
                        name: "FK_Roster_Players_PlayerAPlayerID",
                        column: x => x.PlayerAPlayerID,
                        principalTable: "Players",
                        principalColumn: "PlayerID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Roster_Players_PlayerBPlayerID",
                        column: x => x.PlayerBPlayerID,
                        principalTable: "Players",
                        principalColumn: "PlayerID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Roster_Players_PlayerCPlayerID",
                        column: x => x.PlayerCPlayerID,
                        principalTable: "Players",
                        principalColumn: "PlayerID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Roster_Players_ReservePlayerID",
                        column: x => x.ReservePlayerID,
                        principalTable: "Players",
                        principalColumn: "PlayerID",
                        onDelete: ReferentialAction.Restrict);
                })
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
                name: "FK_Accounts_Players_PlayerID",
                table: "Accounts",
                column: "PlayerID",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Players_PlayerID",
                table: "Accounts");

            migrationBuilder.DropTable(
                name: "Roster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Players",
                table: "Players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.RenameTable(
                name: "Players",
                newName: "Player");

            migrationBuilder.RenameTable(
                name: "Accounts",
                newName: "Account");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_PlayerID",
                table: "Account",
                newName: "IX_Account_PlayerID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Player",
                table: "Player",
                column: "PlayerID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Account",
                table: "Account",
                column: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_Account_Player_PlayerID",
                table: "Account",
                column: "PlayerID",
                principalTable: "Player",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
