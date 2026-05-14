using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gms.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class InitPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PermissionInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermissionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermissionType = table.Column<int>(type: "int", nullable: false),
                    PermissionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermissionDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionApis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApiMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermissionInfoId = table.Column<int>(type: "int", nullable: true),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionApis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionApis_PermissionInfos_PermissionInfoId",
                        column: x => x.PermissionInfoId,
                        principalTable: "PermissionInfos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PermissionInfoRoleInfo",
                columns: table => new
                {
                    PermissionInfosId = table.Column<int>(type: "int", nullable: false),
                    RoleInfosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionInfoRoleInfo", x => new { x.PermissionInfosId, x.RoleInfosId });
                    table.ForeignKey(
                        name: "FK_PermissionInfoRoleInfo_PermissionInfos_PermissionInfosId",
                        column: x => x.PermissionInfosId,
                        principalTable: "PermissionInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionInfoRoleInfo_Roles_RoleInfosId",
                        column: x => x.RoleInfosId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PermissionMenus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MenunOrder = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermissionInfoId = table.Column<int>(type: "int", nullable: true),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionMenus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionMenus_PermissionInfos_PermissionInfoId",
                        column: x => x.PermissionInfoId,
                        principalTable: "PermissionInfos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PermissionPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PointClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PointIcon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PointStatus = table.Column<int>(type: "int", nullable: false),
                    PermissionInfoId = table.Column<int>(type: "int", nullable: true),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermissionPoints_PermissionInfos_PermissionInfoId",
                        column: x => x.PermissionInfoId,
                        principalTable: "PermissionInfos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PermissionApis_PermissionInfoId",
                table: "PermissionApis",
                column: "PermissionInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionInfoRoleInfo_RoleInfosId",
                table: "PermissionInfoRoleInfo",
                column: "RoleInfosId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionMenus_PermissionInfoId",
                table: "PermissionMenus",
                column: "PermissionInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionPoints_PermissionInfoId",
                table: "PermissionPoints",
                column: "PermissionInfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PermissionApis");

            migrationBuilder.DropTable(
                name: "PermissionInfoRoleInfo");

            migrationBuilder.DropTable(
                name: "PermissionMenus");

            migrationBuilder.DropTable(
                name: "PermissionPoints");

            migrationBuilder.DropTable(
                name: "PermissionInfos");
        }
    }
}
