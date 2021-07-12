using Microsoft.EntityFrameworkCore.Migrations;

namespace UCLBackend.Service.Migrations
{
    public partial class AddFranchiseRoleIDs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Key", "Value" },
                values: new object[] { "Franchise.Astros.RoleId", "860638221892976656" }
            );
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Key", "Value" },
                values: new object[] { "Franchise.Atlantics.RoleId", "860638221892976655" }
            );
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Key", "Value" },
                values: new object[] { "Franchise.Bison.RoleId", "820875797635006525" }
            );
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Key", "Value" },
                values: new object[] { "Franchise.Cobras.RoleId", "820875901180182539" }
            );
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Key", "Value" },
                values: new object[] { "Franchise.Gators.RoleId", "820876543563399179" }
            );
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Key", "Value" },
                values: new object[] { "Franchise.Knights.RoleId", "820876631810899978" }
            );
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Key", "Value" },
                values: new object[] { "Franchise.Lightning.RoleId", "858715056149364756" }
            );
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Key", "Value" },
                values: new object[] { "Franchise.Raptors.RoleId", "820876670658674709" }
            );
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Key", "Value" },
                values: new object[] { "Franchise.Samurai.RoleId", "820876924246818827" }
            );
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Key", "Value" },
                values: new object[] { "Franchise.Spartans.RoleId", "820876212489682945" }
            );
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Key", "Value" },
                values: new object[] { "Franchise.XII_Boost.RoleId", "820876089592512512" }
            );
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Key", "Value" },
                values: new object[] { "Franchise.Vikings.RoleId", "820877018887094312" }
            );
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Key", "Value" },
                values: new object[] { "Franchise.Free_Agent.RoleId", "860638221872136193" }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Key",
                keyValue: "Franchise.Astros.RoleId"
            );
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Key",
                keyValue: "Franchise.Atlantics.RoleId"
            );
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Key",
                keyValue: "Franchise.Bison.RoleId"
            );
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Key",
                keyValue: "Franchise.Cobras.RoleId"
            );
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Key",
                keyValue: "Franchise.Gators.RoleId"
            );
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Key",
                keyValue: "Franchise.Knights.RoleId"
            );
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Key",
                keyValue: "Franchise.Lightning.RoleId"
            );
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Key",
                keyValue: "Franchise.Raptors.RoleId"
            );
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Key",
                keyValue: "Franchise.Samurai.RoleId"
            );
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Key",
                keyValue: "Franchise.Spartans.RoleId"
            );
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Key",
                keyValue: "Franchise.XII_Boost.RoleId"
            );
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Key",
                keyValue: "Franchise.Vikings.RoleId"
            );
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Key",
                keyValue: "Franchise.Free_Agent.RoleId"
            );
        }
    }
}
