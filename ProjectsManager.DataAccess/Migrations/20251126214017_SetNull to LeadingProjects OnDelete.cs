using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectsManager.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SetNulltoLeadingProjectsOnDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Employee_LeaderId",
                table: "Projects");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Employee_LeaderId",
                table: "Projects",
                column: "LeaderId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Employee_LeaderId",
                table: "Projects");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Employee_LeaderId",
                table: "Projects",
                column: "LeaderId",
                principalTable: "Employee",
                principalColumn: "Id");
        }
    }
}
