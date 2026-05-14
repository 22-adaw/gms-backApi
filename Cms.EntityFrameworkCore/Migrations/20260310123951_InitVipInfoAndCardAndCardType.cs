using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gms.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class InitVipInfoAndCardAndCardType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VipCardTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VipCardTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VipCardTypeCode = table.Column<int>(type: "int", nullable: true),
                    DiscountRate = table.Column<double>(type: "float", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VipCardTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VipInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VipName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VipPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VipEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VipInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VipCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RemainTimes = table.Column<int>(type: "int", nullable: true),
                    FreezeStatus = table.Column<int>(type: "int", nullable: true),
                    VipInfoId = table.Column<int>(type: "int", nullable: false),
                    VipCardTypeId = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VipCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VipCards_VipCardTypes_VipCardTypeId",
                        column: x => x.VipCardTypeId,
                        principalTable: "VipCardTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VipCards_VipInfos_VipInfoId",
                        column: x => x.VipInfoId,
                        principalTable: "VipInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VipCards_VipCardTypeId",
                table: "VipCards",
                column: "VipCardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VipCards_VipInfoId",
                table: "VipCards",
                column: "VipInfoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VipCards");

            migrationBuilder.DropTable(
                name: "VipCardTypes");

            migrationBuilder.DropTable(
                name: "VipInfos");
        }
    }
}
