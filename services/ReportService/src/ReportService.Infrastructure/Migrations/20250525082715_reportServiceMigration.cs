using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class reportServiceMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GPA = table.Column<decimal>(type: "numeric", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReportId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Grade = table.Column<decimal>(type: "numeric", nullable: true),
                    CourseTitle = table.Column<string>(type: "text", nullable: false),
                    Credits = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportDetails_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Reports",
                columns: new[] { "Id", "GPA", "GeneratedAt", "Status", "StudentId" },
                values: new object[] { new Guid("c6afdaf1-93f9-4a43-a743-4c762667c15f"), 3.7m, new DateTime(2025, 5, 24, 18, 58, 0, 0, DateTimeKind.Utc), "Completed", new Guid("b1c2d3e4-f5f6-7890-abcd-ef1234567890") });

            migrationBuilder.InsertData(
                table: "ReportDetails",
                columns: new[] { "Id", "CourseId", "CourseTitle", "Credits", "Grade", "ReportId", "Status" },
                values: new object[] { new Guid("0f0ae5cf-ebe6-4440-ac49-0872244a0b33"), new Guid("0365eed4-e67d-460a-abd7-6742b3698d86"), "Introduction to Physics", 3, 90.0m, new Guid("c6afdaf1-93f9-4a43-a743-4c762667c15f"), "Completed" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportDetails_ReportId",
                table: "ReportDetails",
                column: "ReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportDetails");

            migrationBuilder.DropTable(
                name: "Reports");
        }
    }
}
