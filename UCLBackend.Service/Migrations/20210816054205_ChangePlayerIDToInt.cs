using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UCLBackend.Service.Migrations
{
    public partial class ChangePlayerIDToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Players_PlayerID",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Standings_Players_PlayerID",
                table: "Standings");

            migrationBuilder.AlterColumn<int>(
                name: "PlayerID",
                table: "Standings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<double>(
                name: "Salary",
                table: "Players",
                type: "double",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PeakMMR",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CurrentMMR",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlayerID",
                table: "Players",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "PlayerID",
                table: "Accounts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Players_PlayerID",
                table: "Accounts",
                column: "PlayerID",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Standings_Players_PlayerID",
                table: "Standings",
                column: "PlayerID",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Players_PlayerID",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Standings_Players_PlayerID",
                table: "Standings");

            migrationBuilder.AlterColumn<string>(
                name: "PlayerID",
                table: "Standings",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<double>(
                name: "Salary",
                table: "Players",
                type: "double",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<int>(
                name: "PeakMMR",
                table: "Players",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentMMR",
                table: "Players",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PlayerID",
                table: "Players",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "PlayerID",
                table: "Accounts",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Players_PlayerID",
                table: "Accounts",
                column: "PlayerID",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Standings_Players_PlayerID",
                table: "Standings",
                column: "PlayerID",
                principalTable: "Players",
                principalColumn: "PlayerID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
