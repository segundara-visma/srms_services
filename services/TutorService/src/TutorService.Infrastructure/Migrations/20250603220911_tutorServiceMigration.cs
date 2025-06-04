using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TutorService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class tutorServiceMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tutors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tutors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TutorCourses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TutorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutorCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TutorCourses_Tutors_TutorId",
                        column: x => x.TutorId,
                        principalTable: "Tutors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Tutors",
                columns: new[] { "Id", "UserId" },
                values: new object[] { new Guid("cdf8756b-1b69-49b3-b89a-a7ac0652b080"), new Guid("b2c3d4e5-f6a7-890b-cdef-1234567890ab") });

            migrationBuilder.InsertData(
                table: "TutorCourses",
                columns: new[] { "Id", "AssignmentDate", "CourseId", "TutorId" },
                values: new object[,]
                {
                    { new Guid("70466362-d316-4385-a0b5-b5df1eb8b779"), new DateTime(2025, 6, 3, 22, 9, 11, 333, DateTimeKind.Utc).AddTicks(4845), new Guid("0365eed4-e67d-460a-abd7-6742b3698d86"), new Guid("cdf8756b-1b69-49b3-b89a-a7ac0652b080") },
                    { new Guid("7c7753bb-90ad-48f6-8260-4fa8ef82322f"), new DateTime(2025, 6, 3, 22, 9, 11, 333, DateTimeKind.Utc).AddTicks(4724), new Guid("6daa15cc-f355-42e7-99aa-9a52086350a7"), new Guid("cdf8756b-1b69-49b3-b89a-a7ac0652b080") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TutorCourses_TutorId",
                table: "TutorCourses",
                column: "TutorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TutorCourses");

            migrationBuilder.DropTable(
                name: "Tutors");
        }
    }
}
