using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsManager.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class emailisunique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Employee_Email",
                table: "Employee",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employee_Email",
                table: "Employee");
        }
    }
}
