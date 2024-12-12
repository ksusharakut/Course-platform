using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Course_platform.Migrations
{
    /// <inheritdoc />
    public partial class lessonMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "lessons",
                newName: "FilePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "lessons",
                newName: "Content");
        }
    }
}
