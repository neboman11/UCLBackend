using Microsoft.EntityFrameworkCore.Migrations;

namespace UCLBackend.Service.Migrations
{
    public partial class AddTransactionChannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Key", "Value" },
                values: new object[] { "Transaction.ChannelId", "866885724350447626" }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Settings",
                keyColumn: "Key",
                keyValue: "Transaction.ChannelId"
            );
        }
    }
}
