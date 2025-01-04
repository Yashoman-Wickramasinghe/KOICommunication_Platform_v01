using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOICommunicationPlatform.DataAccess.Migrations
{
    public partial class newcolumnTaskBoard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskBoards_Courses_CourseId",
                table: "TaskBoards");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "TaskBoards",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskBoards_Courses_CourseId",
                table: "TaskBoards",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskBoards_Courses_CourseId",
                table: "TaskBoards");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "TaskBoards",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskBoards_Courses_CourseId",
                table: "TaskBoards",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
