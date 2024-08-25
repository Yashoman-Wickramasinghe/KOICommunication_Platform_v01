using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOICommunicationPlatform.DataAccess.Migrations
{
    public partial class removeunwantedcolumnsdocupload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentsOnDocumentUploads_Courses_CourseId",
                table: "CommentsOnDocumentUploads");

            migrationBuilder.DropIndex(
                name: "IX_CommentsOnDocumentUploads_CourseId",
                table: "CommentsOnDocumentUploads");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "CommentsOnDocumentUploads");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "CommentsOnDocumentUploads");

            migrationBuilder.DropColumn(
                name: "Trimester",
                table: "CommentsOnDocumentUploads");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "CommentsOnDocumentUploads");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "CommentsOnDocumentUploads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "CommentsOnDocumentUploads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Trimester",
                table: "CommentsOnDocumentUploads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "CommentsOnDocumentUploads",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CommentsOnDocumentUploads_CourseId",
                table: "CommentsOnDocumentUploads",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentsOnDocumentUploads_Courses_CourseId",
                table: "CommentsOnDocumentUploads",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
