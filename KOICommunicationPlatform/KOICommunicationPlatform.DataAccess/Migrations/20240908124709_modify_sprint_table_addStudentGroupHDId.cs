using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOICommunicationPlatform.DataAccess.Migrations
{
    public partial class modify_sprint_table_addStudentGroupHDId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sprints_StudentGroupHDs_StudentGroupHDId",
                table: "Sprints");

            migrationBuilder.AlterColumn<int>(
                name: "StudentGroupHDId",
                table: "Sprints",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Sprints_StudentGroupHDs_StudentGroupHDId",
                table: "Sprints",
                column: "StudentGroupHDId",
                principalTable: "StudentGroupHDs",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sprints_StudentGroupHDs_StudentGroupHDId",
                table: "Sprints");

            migrationBuilder.AlterColumn<int>(
                name: "StudentGroupHDId",
                table: "Sprints",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sprints_StudentGroupHDs_StudentGroupHDId",
                table: "Sprints",
                column: "StudentGroupHDId",
                principalTable: "StudentGroupHDs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
