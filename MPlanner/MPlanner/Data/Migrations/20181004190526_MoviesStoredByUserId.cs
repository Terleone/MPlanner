using Microsoft.EntityFrameworkCore.Migrations;

namespace MPlanner.Data.Migrations
{
    public partial class MoviesStoredByUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Movie",
                newName: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Movie",
                newName: "UserName");
        }
    }
}
