using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GradeService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GradeServiceMigrationUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateGraded",
                table: "Grades",
                newName: "GradedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GradedAt",
                table: "Grades",
                newName: "DateGraded");
        }
    }
}
