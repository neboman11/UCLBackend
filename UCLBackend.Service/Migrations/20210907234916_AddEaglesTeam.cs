using Microsoft.EntityFrameworkCore.Migrations;

namespace UCLBackend.Service.Migrations
{
    public partial class AddEaglesTeam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Key", "Value" },
                values: new object[] { "Franchise.Eagles.RoleId", "820876299732123691" }
            );
            migrationBuilder.InsertData(
                table: "Roster",
                columns: new[] { "League", "TeamName", "Conference" },
                values: new object[] { "Origins", "Eagles", "Origins" }
            );
            migrationBuilder.InsertData(
                table: "Roster",
                columns: new[] { "League", "TeamName", "Conference" },
                values: new object[] { "Ultra", "Eagles", "East" }
            );
            migrationBuilder.InsertData(
                table: "Roster",
                columns: new[] { "League", "TeamName", "Conference" },
                values: new object[] { "Elite", "Eagles", "East" }
            );
            migrationBuilder.InsertData(
                table: "Roster",
                columns: new[] { "League", "TeamName", "Conference" },
                values: new object[] { "Superior", "Eagles", "East" }
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
                keyValues: new[] { "Origins", "Eagles" }
            );
            migrationBuilder.DeleteData(
                table: "Roster",
                keyColumns: new[] { "League", "TeamName" },
                keyValues: new[] { "Ultra", "Eagles" }
            );
            migrationBuilder.DeleteData(
                table: "Roster",
                keyColumns: new[] { "League", "TeamName" },
                keyValues: new[] { "Elite", "Eagles" }
            );
            migrationBuilder.DeleteData(
                table: "Roster",
                keyColumns: new[] { "League", "TeamName" },
                keyValues: new[] { "Superior", "Eagles" }
            );
        }
    }
}
