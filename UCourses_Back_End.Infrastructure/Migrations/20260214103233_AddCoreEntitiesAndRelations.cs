using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UCourses_Back_End.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCoreEntitiesAndRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "Instructors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Courses_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrollments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VideoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PdfUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartAt = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndAt = table.Column<TimeOnly>(type: "time", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sections_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "CreatedAt", "Description", "ImageUrl", "Name", "PublicId" },
                values: new object[,]
                {
                    { new Guid("a1a1a1a1-b2b2-4c3c-d4d4-e5e5e5e5e5e5"), new DateOnly(2024, 1, 1), "Learn modern web development technologies", "/Images/Departments/web-dev.jpg", "Web Development", "DEPT-a1a1a1a1" },
                    { new Guid("b2b2b2b2-c3c3-4d4d-e5e5-f6f6f6f6f6f6"), new DateOnly(2024, 1, 1), "Master mobile app development", "/Images/Departments/mobile-dev.jpg", "Mobile Development", "DEPT-b2b2b2b2" }
                });

            migrationBuilder.UpdateData(
                table: "Instructors",
                keyColumn: "Id",
                keyValue: new Guid("e5f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"),
                column: "DepartmentId",
                value: null);

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "CreatedAt", "DepartmentId", "Description", "ImageUrl", "InstructorId", "Name", "Price", "PublicId" },
                values: new object[,]
                {
                    { new Guid("c1c1c1c1-d2d2-4e3e-f4f4-a5a5a5a5a5a5"), new DateOnly(2024, 1, 1), new Guid("a1a1a1a1-b2b2-4c3c-d4d4-e5e5e5e5e5e5"), "Complete guide to building web applications with ASP.NET Core", "/Images/Courses/aspnet-core.jpg", new Guid("e5f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"), "ASP.NET Core Complete Guide", 499.99m, "CRS-c1c1c1c1" },
                    { new Guid("d2d2d2d2-e3e3-4f4f-a5a5-b6b6b6b6b6b6"), new DateOnly(2024, 1, 1), new Guid("b2b2b2b2-c3c3-4d4d-e5e5-f6f6f6f6f6f6"), "Build cross-platform mobile apps with React Native", "/Images/Courses/react-native.jpg", new Guid("e5f6a7b8-c9d0-4e1f-2a3b-4c5d6e7f8a9b"), "React Native Masterclass", 599.99m, "CRS-d2d2d2d2" }
                });

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "Id", "CourseId", "CreatedAt", "PublicId", "StudentId" },
                values: new object[] { new Guid("a1a1a1a1-b2b2-4c3c-d4d4-111111111111"), new Guid("c1c1c1c1-d2d2-4e3e-f4f4-a5a5a5a5a5a5"), new DateOnly(2024, 1, 1), "ENRL-a1a1a1a1", new Guid("f6a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c") });

            migrationBuilder.InsertData(
                table: "Sections",
                columns: new[] { "Id", "CourseId", "CreatedAt", "DayOfWeek", "Description", "EndAt", "Name", "PdfUrl", "PublicId", "StartAt", "VideoUrl" },
                values: new object[,]
                {
                    { new Guid("e1e1e1e1-f2f2-4a3a-b4b4-c5c5c5c5c5c5"), new Guid("c1c1c1c1-d2d2-4e3e-f4f4-a5a5a5a5a5a5"), new DateOnly(2024, 1, 1), 1, "Getting started with ASP.NET Core framework", new TimeOnly(12, 0, 0), "Introduction to ASP.NET Core", "/PDFs/Sections/aspnet-intro.pdf", "SEC-e1e1e1e1", new TimeOnly(10, 0, 0), "/Videos/Sections/aspnet-intro.mp4" },
                    { new Guid("f2f2f2f2-a3a3-4b4b-c5c5-d6d6d6d6d6d6"), new Guid("d2d2d2d2-e3e3-4f4f-a5a5-b6b6b6b6b6b6"), new DateOnly(2024, 1, 1), 2, "Learn the fundamentals of React Native", new TimeOnly(16, 0, 0), "React Native Basics", null, "SEC-f2f2f2f2", new TimeOnly(14, 0, 0), "/Videos/Sections/react-native-basics.mp4" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_DepartmentId",
                table: "Instructors",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_DepartmentId",
                table: "Courses",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstructorId",
                table: "Courses",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId_StudentId",
                table: "Enrollments",
                columns: new[] { "CourseId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId",
                table: "Enrollments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CourseId",
                table: "Sections",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Instructors_Departments_DepartmentId",
                table: "Instructors",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Instructors_Departments_DepartmentId",
                table: "Instructors");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Instructors_DepartmentId",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Instructors");
        }
    }
}
