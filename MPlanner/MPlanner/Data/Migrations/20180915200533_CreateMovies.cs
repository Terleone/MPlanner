using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MPlanner.Data.Migrations
{
    public partial class CreateMovies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MovieId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Movie",
                columns: table => new
                {
                    MovieId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    Genre = table.Column<string>(nullable: true),
                    Time = table.Column<int>(nullable: true),
                    Director = table.Column<string>(nullable: true),
                    Year = table.Column<string>(nullable: true),
                    Actors = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movie", x => x.MovieId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_MovieId",
                table: "AspNetUsers",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Movie_MovieId",
                table: "AspNetUsers",
                column: "MovieId",
                principalTable: "Movie",
                principalColumn: "MovieId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Movie_MovieId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Movie");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_MovieId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "MovieId",
                table: "AspNetUsers");
        }
    }
}
