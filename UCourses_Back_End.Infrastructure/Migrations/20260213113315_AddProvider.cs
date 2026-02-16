using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCourses_Back_End.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProvider : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProviderId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5d",
                columns: new[] { "Provider", "ProviderId" },
                values: new object[] { "Local", "a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b2c3d4e5-f6a7-4b8c-9d0e-1f2a3b4c5d6e",
                columns: new[] { "Provider", "ProviderId" },
                values: new object[] { "Local", "a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5q" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c3d4e5f6-a7b8-4c9d-0e1f-2a3b4c5d6e7f",
                columns: new[] { "Provider", "ProviderId" },
                values: new object[] { "Local", "a1b2c3d4-e5f6-4a7b-8c9d-0e1f2a3b4c5p" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Provider",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "AspNetUsers");
        }
    }
}
