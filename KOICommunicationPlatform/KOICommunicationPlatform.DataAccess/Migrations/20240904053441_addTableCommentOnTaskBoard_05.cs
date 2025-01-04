using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOICommunicationPlatform.DataAccess.Migrations
{
    public partial class addTableCommentOnTaskBoard_05 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommentsOnTaskBoards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifieDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SprintTaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentsOnTaskBoards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentsOnTaskBoards_SprintTasks_SprintTaskId",
                        column: x => x.SprintTaskId,
                        principalTable: "SprintTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentsOnTaskBoards_SprintTaskId",
                table: "CommentsOnTaskBoards",
                column: "SprintTaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentsOnTaskBoards");
        }
    }
}
