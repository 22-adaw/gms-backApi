using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gms.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVipInfoAndCardForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VipCards_VipInfos_VipInfoId",
                table: "VipCards");

            migrationBuilder.DropIndex(
                name: "IX_VipCards_VipInfoId",
                table: "VipCards");

            migrationBuilder.DropColumn(
                name: "VipInfoId",
                table: "VipCards");

            migrationBuilder.AddColumn<int>(
                name: "VipCardId",
                table: "VipInfos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VipInfos_VipCardId",
                table: "VipInfos",
                column: "VipCardId",
                unique: true,
                filter: "[VipCardId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_VipInfos_VipCards_VipCardId",
                table: "VipInfos",
                column: "VipCardId",
                principalTable: "VipCards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VipInfos_VipCards_VipCardId",
                table: "VipInfos");

            migrationBuilder.DropIndex(
                name: "IX_VipInfos_VipCardId",
                table: "VipInfos");

            migrationBuilder.DropColumn(
                name: "VipCardId",
                table: "VipInfos");

            migrationBuilder.AddColumn<int>(
                name: "VipInfoId",
                table: "VipCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VipCards_VipInfoId",
                table: "VipCards",
                column: "VipInfoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VipCards_VipInfos_VipInfoId",
                table: "VipCards",
                column: "VipInfoId",
                principalTable: "VipInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
