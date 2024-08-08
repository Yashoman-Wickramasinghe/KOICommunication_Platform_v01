using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOICommunicationPlatform.Repositories.Migrations
{
    public partial class migration_13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sprints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SprintName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentGroupHDId = table.Column<int>(type: "int", nullable: true),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: true),
                    UserRoleId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Trimester = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifieDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sprints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sprints_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sprints_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sprints_UserRoles_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SprintTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentGroupHDId = table.Column<int>(type: "int", nullable: false),
                    StudentGroupDetailId = table.Column<int>(type: "int", nullable: false),
                    UserRoleId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SprintId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifieDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SprintTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SprintTasks_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SprintTasks_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SprintTasks_StudentGroupDetails_StudentGroupDetailId",
                        column: x => x.StudentGroupDetailId,
                        principalTable: "StudentGroupDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SprintTasks_StudentGroupHDs_StudentGroupHDId",
                        column: x => x.StudentGroupHDId,
                        principalTable: "StudentGroupHDs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SprintTasks_UserRoles_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentsOnTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentGroupHDId = table.Column<int>(type: "int", nullable: false),
                    UserRoleId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateTaskId = table.Column<int>(type: "int", nullable: false),
                    SprintId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifieDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentsOnTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentsOnTasks_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentsOnTasks_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentsOnTasks_SprintTasks_CreateTaskId",
                        column: x => x.CreateTaskId,
                        principalTable: "SprintTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentsOnTasks_StudentGroupHDs_StudentGroupHDId",
                        column: x => x.StudentGroupHDId,
                        principalTable: "StudentGroupHDs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentsOnTasks_UserRoles_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskAllocationMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentGroupHDId = table.Column<int>(type: "int", nullable: false),
                    StudentGroupDetailId = table.Column<int>(type: "int", nullable: false),
                    UserRoleId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SprintTaskId = table.Column<int>(type: "int", nullable: false),
                    SprintId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifieDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAllocationMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskAllocationMembers_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskAllocationMembers_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskAllocationMembers_SprintTasks_SprintTaskId",
                        column: x => x.SprintTaskId,
                        principalTable: "SprintTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskAllocationMembers_StudentGroupDetails_StudentGroupDetailId",
                        column: x => x.StudentGroupDetailId,
                        principalTable: "StudentGroupDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskAllocationMembers_StudentGroupHDs_StudentGroupHDId",
                        column: x => x.StudentGroupHDId,
                        principalTable: "StudentGroupHDs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskAllocationMembers_UserRoles_UserRoleId",
                        column: x => x.UserRoleId,
                        principalTable: "UserRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskBoards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SprintId = table.Column<int>(type: "int", nullable: false),
                    SprintTaskId = table.Column<int>(type: "int", nullable: false),
                    StudentGroupHDId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifieDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskBoards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskBoards_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskBoards_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskBoards_Sprints_SprintId",
                        column: x => x.SprintId,
                        principalTable: "Sprints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskBoards_SprintTasks_SprintTaskId",
                        column: x => x.SprintTaskId,
                        principalTable: "SprintTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskBoards_StudentGroupHDs_StudentGroupHDId",
                        column: x => x.StudentGroupHDId,
                        principalTable: "StudentGroupHDs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskBoards_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentsOnTasks_ApplicationUserId",
                table: "CommentsOnTasks",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentsOnTasks_CreateTaskId",
                table: "CommentsOnTasks",
                column: "CreateTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentsOnTasks_SprintId",
                table: "CommentsOnTasks",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentsOnTasks_StudentGroupHDId",
                table: "CommentsOnTasks",
                column: "StudentGroupHDId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentsOnTasks_UserRoleId",
                table: "CommentsOnTasks",
                column: "UserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_ApplicationUserId",
                table: "Sprints",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_CourseId",
                table: "Sprints",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Sprints_UserRoleId",
                table: "Sprints",
                column: "UserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintTasks_ApplicationUserId",
                table: "SprintTasks",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintTasks_SprintId",
                table: "SprintTasks",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintTasks_StudentGroupDetailId",
                table: "SprintTasks",
                column: "StudentGroupDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintTasks_StudentGroupHDId",
                table: "SprintTasks",
                column: "StudentGroupHDId");

            migrationBuilder.CreateIndex(
                name: "IX_SprintTasks_UserRoleId",
                table: "SprintTasks",
                column: "UserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAllocationMembers_ApplicationUserId",
                table: "TaskAllocationMembers",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAllocationMembers_SprintId",
                table: "TaskAllocationMembers",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAllocationMembers_SprintTaskId",
                table: "TaskAllocationMembers",
                column: "SprintTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAllocationMembers_StudentGroupDetailId",
                table: "TaskAllocationMembers",
                column: "StudentGroupDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAllocationMembers_StudentGroupHDId",
                table: "TaskAllocationMembers",
                column: "StudentGroupHDId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAllocationMembers_UserRoleId",
                table: "TaskAllocationMembers",
                column: "UserRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBoards_ApplicationUserId",
                table: "TaskBoards",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBoards_CourseId",
                table: "TaskBoards",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBoards_SprintId",
                table: "TaskBoards",
                column: "SprintId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBoards_SprintTaskId",
                table: "TaskBoards",
                column: "SprintTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBoards_StudentGroupHDId",
                table: "TaskBoards",
                column: "StudentGroupHDId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskBoards_SubjectId",
                table: "TaskBoards",
                column: "SubjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentsOnTasks");

            migrationBuilder.DropTable(
                name: "TaskAllocationMembers");

            migrationBuilder.DropTable(
                name: "TaskBoards");

            migrationBuilder.DropTable(
                name: "SprintTasks");

            migrationBuilder.DropTable(
                name: "Sprints");
        }
    }
}
