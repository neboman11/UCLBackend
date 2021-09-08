using Microsoft.EntityFrameworkCore.Migrations;

namespace UCLBackend.Service.Migrations
{
    public partial class AddCubsTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Key", "Value" },
                values: new object[] { "Franchise.Cubs.RoleId", "885229552211750942" }
            );
            migrationBuilder.InsertData(
                table: "Roster",
                columns: new[] { "League", "TeamName", "Conference" },
                values: new object[] { "Origins", "Cubs", "Origins" }
            );
            migrationBuilder.InsertData(
                table: "Roster",
                columns: new[] { "League", "TeamName", "Conference" },
                values: new object[] { "Ultra", "Cubs", "East" }
            );
            migrationBuilder.InsertData(
                table: "Roster",
                columns: new[] { "League", "TeamName", "Conference" },
                values: new object[] { "Elite", "Cubs", "East" }
            );
            migrationBuilder.InsertData(
                table: "Roster",
                columns: new[] { "League", "TeamName", "Conference" },
                values: new object[] { "Superior", "Cubs", "East" }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Key",
                keyValue: "Roster.FreeAgent.ChannelId"
            );
            migrationBuilder.DeleteData(
                table: "Roster",
                keyColumns: new[] { "League", "TeamName" },
                keyValues: new[] { "Origins", "Cubs" }
            );
            migrationBuilder.DeleteData(
                table: "Roster",
                keyColumns: new[] { "League", "TeamName" },
                keyValues: new[] { "Ultra", "Cubs" }
            );
            migrationBuilder.DeleteData(
                table: "Roster",
                keyColumns: new[] { "League", "TeamName" },
                keyValues: new[] { "Elite", "Cubs" }
            );
            migrationBuilder.DeleteData(
                table: "Roster",
                keyColumns: new[] { "League", "TeamName" },
                keyValues: new[] { "Superior", "Cubs" }
            );
        }
    }
}
