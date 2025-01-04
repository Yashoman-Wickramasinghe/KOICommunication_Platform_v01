using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOICommunicationPlatform.DataAccess.Migrations
{
    public partial class migration_sprintTask_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TaskName",
                table: "SprintTasks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaskName",
                table: "SprintTasks");
        }
    }
}
