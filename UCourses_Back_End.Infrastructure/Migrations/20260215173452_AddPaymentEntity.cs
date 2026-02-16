using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UCourses_Back_End.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Enrollments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnrollmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: new Guid("a1a1a1a1-b2b2-4c3c-d4d4-111111111111"),
                column: "Status",
                value: "Captured");

            migrationBuilder.InsertData(
                table: "Enrollments",
                columns: new[] { "Id", "CourseId", "CreatedAt", "PublicId", "Status", "StudentId" },
                values: new object[] { new Guid("b2b2b2b2-c3c3-4d4d-e5e5-222222222222"), new Guid("d2d2d2d2-e3e3-4f4f-a5a5-b6b6b6b6b6b6"), new DateOnly(2024, 1, 1), "ENRL-b2b2b2b2", "Captured", new Guid("f6a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c") });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "EnrollmentId", "PaymentDate", "PaymentMethod", "Status", "StudentId", "TransactionId" },
                values: new object[,]
                {
                    { 1, 499.99m, new Guid("a1a1a1a1-b2b2-4c3c-d4d4-111111111111"), new DateTime(2024, 1, 15, 10, 30, 0, 0, DateTimeKind.Utc), "CreditCard", "Captured", new Guid("f6a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c"), "TXN-2024-001-ABC123" },
                    { 2, 599.99m, new Guid("b2b2b2b2-c3c3-4d4d-e5e5-222222222222"), new DateTime(2024, 1, 20, 14, 45, 0, 0, DateTimeKind.Utc), "PayPal", "Captured", new Guid("f6a7b8c9-d0e1-4f2a-3b4c-5d6e7f8a9b0c"), "TXN-2024-002-XYZ789" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_EnrollmentId",
                table: "Payments",
                column: "EnrollmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_StudentId",
                table: "Payments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TransactionId",
                table: "Payments",
                column: "TransactionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DeleteData(
                table: "Enrollments",
                keyColumn: "Id",
                keyValue: new Guid("b2b2b2b2-c3c3-4d4d-e5e5-222222222222"));

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Enrollments");
        }
    }
}
