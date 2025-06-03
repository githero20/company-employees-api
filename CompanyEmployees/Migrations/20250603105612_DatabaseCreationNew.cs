using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CompanyEmployees.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseCreationNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "006d884e-7193-4c89-ab27-c3ce0b36febe");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d73b2d7c-5670-442c-a50d-4dc435c9aa75");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1a98dbcf-bdcb-46dc-8e11-2c317ee9a620", null, "Manager", "MANAGER" },
                    { "8bde5730-6634-4ead-bd07-0c116f576e58", null, "Administrator", "ADMINISTRATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1a98dbcf-bdcb-46dc-8e11-2c317ee9a620");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8bde5730-6634-4ead-bd07-0c116f576e58");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "006d884e-7193-4c89-ab27-c3ce0b36febe", null, "Administrator", "ADMINISTRATOR" },
                    { "d73b2d7c-5670-442c-a50d-4dc435c9aa75", null, "Manager", "MANAGER" }
                });
        }
    }
}
