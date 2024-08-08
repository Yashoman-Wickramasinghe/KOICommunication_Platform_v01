using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KOICommunicationPlatform.Repositories.Migrations
{
    public partial class second_modify_client_Table_userRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_UserRoles_UserRoleId",
                table: "Clients");

            migrationBuilder.AlterColumn<int>(
                name: "UserRoleId",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_UserRoles_UserRoleId",
                table: "Clients",
                column: "UserRoleId",
                principalTable: "UserRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_UserRoles_UserRoleId",
                table: "Clients");

            migrationBuilder.AlterColumn<int>(
                name: "UserRoleId",
                table: "Clients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_UserRoles_UserRoleId",
                table: "Clients",
                column: "UserRoleId",
                principalTable: "UserRoles",
                principalColumn: "Id");
        }
    }
}
