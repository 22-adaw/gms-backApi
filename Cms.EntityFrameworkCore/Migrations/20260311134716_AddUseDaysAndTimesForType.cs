using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gms.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddUseDaysAndTimesForType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UseDays",
                table: "VipCardTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UseTimes",
                table: "VipCardTypes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UseDays",
                table: "VipCardTypes");

            migrationBuilder.DropColumn(
                name: "UseTimes",
                table: "VipCardTypes");
        }
    }
}
