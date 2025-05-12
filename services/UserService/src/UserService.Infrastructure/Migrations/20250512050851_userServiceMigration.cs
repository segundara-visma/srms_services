using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class userServiceMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Firstname = table.Column<string>(type: "text", nullable: false),
                    Lastname = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    ProfilePictureUrl = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    ZipCode = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Nationality = table.Column<string>(type: "text", nullable: true),
                    Bio = table.Column<string>(type: "text", nullable: true),
                    FacebookUrl = table.Column<string>(type: "text", nullable: true),
                    TwitterUrl = table.Column<string>(type: "text", nullable: true),
                    LinkedInUrl = table.Column<string>(type: "text", nullable: true),
                    InstagramUrl = table.Column<string>(type: "text", nullable: true),
                    WebsiteUrl = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Tutor" },
                    { 3, "Student" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Firstname", "Lastname", "PasswordHash", "RoleId" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), "admin.user@example.com", "Admin", "User", "$2a$12$FtTE2pZ1h7nRML.MFelVL.d6eyre70x295KX/T6kiaqW55v0Fdo2a", 1 },
                    { new Guid("b1c2d3e4-f5f6-7890-abcd-ef1234567890"), "john.doe@example.com", "John", "Doe", "$2a$12$FtTE2pZ1h7nRML.MFelVL.d6eyre70x295KX/T6kiaqW55v0Fdo2a", 3 },
                    { new Guid("b2c3d4e5-f6a7-890b-cdef-1234567890ab"), "tutor.teacher@example.com", "Tutor", "Teacher", "$2a$12$FtTE2pZ1h7nRML.MFelVL.d6eyre70x295KX/T6kiaqW55v0Fdo2a", 2 },
                    { new Guid("c2d3e4f5-f678-90ab-cdef-1234567890ab"), "jane.smith@example.com", "Jane", "Smith", "$2a$12$FtTE2pZ1h7nRML.MFelVL.d6eyre70x295KX/T6kiaqW55v0Fdo2a", 3 }
                });

            migrationBuilder.InsertData(
                table: "Profiles",
                columns: new[] { "Id", "Address", "Bio", "City", "Country", "FacebookUrl", "InstagramUrl", "LinkedInUrl", "Nationality", "Phone", "ProfilePictureUrl", "State", "TwitterUrl", "UserId", "WebsiteUrl", "ZipCode" },
                values: new object[,]
                {
                    { new Guid("9eb76b25-ce6d-7890-abcd-ef1234567890"), "101 Tutor Ln", "A dedicated tutor.", "Seattle", "USA", null, null, null, "American", "555-1011", null, "WA", "https://twitter.com/tutorteacher", new Guid("b2c3d4e5-f6a7-890b-cdef-1234567890ab"), null, "98101" },
                    { new Guid("d1e2f3a4-b5c6-7890-abcd-ef1234567890"), "123 Main St", "A passionate student.", "New York", "USA", "https://facebook.com/johndoe", null, null, "American", "555-0123", null, "NY", "https://twitter.com/johndoe", new Guid("b1c2d3e4-f5f6-7890-abcd-ef1234567890"), null, "10001" },
                    { new Guid("e1f2a3b4-c5d6-7890-abcd-ef1234567890"), "456 Oak St", "An enthusiastic learner.", "Los Angeles", "USA", null, null, "https://linkedin.com/in/janesmith", "American", "555-0456", null, "CA", null, new Guid("c2d3e4f5-f678-90ab-cdef-1234567890ab"), null, "90001" },
                    { new Guid("febfaa10-358c-7890-abcd-ef1234567890"), "789 Admin Rd", "An experienced administrator.", "Chicago", "USA", null, null, "https://linkedin.com/in/adminuser", "American", "555-0789", null, "IL", null, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), null, "60601" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_UserId",
                table: "Profiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
