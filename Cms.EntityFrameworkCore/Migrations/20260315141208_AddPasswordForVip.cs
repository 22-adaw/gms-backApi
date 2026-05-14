using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gms.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordForVip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Users_CoachId",
                table: "Lessons");

            migrationBuilder.AddColumn<string>(
                name: "VipPassword",
                table: "VipInfos",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "123456");

            migrationBuilder.AlterColumn<int>(
                name: "CoachId",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Users_CoachId",
                table: "Lessons",
                column: "CoachId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Users_CoachId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "VipPassword",
                table: "VipInfos");

            migrationBuilder.AlterColumn<int>(
                name: "CoachId",
                table: "Lessons",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Users_CoachId",
                table: "Lessons",
                column: "CoachId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
