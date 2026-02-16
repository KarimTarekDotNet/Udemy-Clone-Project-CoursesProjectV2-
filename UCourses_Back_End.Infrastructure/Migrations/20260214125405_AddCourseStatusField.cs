using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCourses_Back_End.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseStatusField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("c1c1c1c1-d2d2-4e3e-f4f4-a5a5a5a5a5a5"),
                column: "Status",
                value: "Published");

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: new Guid("d2d2d2d2-e3e3-4f4f-a5a5-b6b6b6b6b6b6"),
                column: "Status",
                value: "Draft");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Courses");
        }
    }
}
