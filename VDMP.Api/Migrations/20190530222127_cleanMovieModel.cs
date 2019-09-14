using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VDMP.Api.Migrations
{
    public partial class cleanMovieModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "Homepage",
                "Movie");

            migrationBuilder.DropColumn(
                "ImdbId",
                "Movie");

            migrationBuilder.DropColumn(
                "Playback",
                "Movie");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "Homepage",
                "Movie",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                "ImdbId",
                "Movie",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                "Playback",
                "Movie",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}