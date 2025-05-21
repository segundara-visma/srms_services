using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradeService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class gradeServiceMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    GradeValue = table.Column<decimal>(type: "numeric", nullable: false),
                    DateGraded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Grades",
                columns: new[] { "Id", "Comments", "CourseId", "DateGraded", "GradeValue", "StudentId" },
                values: new object[] { new Guid("c53ef888-3f6e-4f72-8cf4-da1e5f03ca16"), "Excellent performance", new Guid("0365eed4-e67d-460a-abd7-6742b3698d86"), new DateTime(2025, 5, 20, 17, 59, 0, 0, DateTimeKind.Utc), 90.0m, new Guid("b1c2d3e4-f5f6-7890-abcd-ef1234567890") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Grades");
        }
    }
}
