using Microsoft.EntityFrameworkCore.Migrations;

namespace VDMP.Api.Migrations
{
    public partial class cleanMovieModel1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "VoteAverage",
                "Movie");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                "VoteAverage",
                "Movie",
                nullable: false,
                defaultValue: 0L);
        }
    }
}