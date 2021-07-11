using Microsoft.EntityFrameworkCore.Migrations;

namespace UCLBackend.Service.Migrations
{
    public partial class FixTeamForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Roster_TeamID",
                table: "Players");

            migrationBuilder.AlterColumn<int>(
                name: "TeamID",
                table: "Players",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Roster_TeamID",
                table: "Players",
                column: "TeamID",
                principalTable: "Roster",
                principalColumn: "TeamID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Roster_TeamID",
                table: "Players");

            migrationBuilder.AlterColumn<int>(
                name: "TeamID",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Roster_TeamID",
                table: "Players",
                column: "TeamID",
                principalTable: "Roster",
                principalColumn: "TeamID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
