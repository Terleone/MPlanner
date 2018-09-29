using Microsoft.EntityFrameworkCore.Migrations;

namespace MPlanner.Data.Migrations
{
    public partial class PositionAttributeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Movie",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "Movie");
        }
    }
}
