using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gms.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddCardNumForCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CardNum",
                table: "VipCards",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardNum",
                table: "VipCards");
        }
    }
}
