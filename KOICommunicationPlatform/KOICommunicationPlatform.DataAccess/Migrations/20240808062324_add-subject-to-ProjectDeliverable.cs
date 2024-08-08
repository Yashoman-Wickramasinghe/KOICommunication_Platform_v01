using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOICommunicationPlatform.DataAccess.Migrations
{
    public partial class addsubjecttoProjectDeliverable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProjectDeliverables_SubjectId",
                table: "ProjectDeliverables",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectDeliverables_Subjects_SubjectId",
                table: "ProjectDeliverables",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectDeliverables_Subjects_SubjectId",
                table: "ProjectDeliverables");

            migrationBuilder.DropIndex(
                name: "IX_ProjectDeliverables_SubjectId",
                table: "ProjectDeliverables");
        }
    }
}
