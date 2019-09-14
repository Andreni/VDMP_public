using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VDMP.Api.Migrations
{
    public partial class initalCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Users",
                table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(nullable: false),
                    UserPassword = table.Column<string>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Users", x => x.UserId); });

            migrationBuilder.CreateTable(
                "Libraries",
                table => new
                {
                    LibraryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    LibraryName = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    LibraryType = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libraries", x => x.LibraryId);
                    table.ForeignKey(
                        "FK_Libraries_Users_UserId",
                        x => x.UserId,
                        "Users",
                        "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                "Movie",
                table => new
                {
                    MovieId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy",
                            SqlServerValueGenerationStrategy.IdentityColumn),
                    FileName = table.Column<string>(nullable: true),
                    PathToVideo = table.Column<string>(nullable: true),
                    Rating = table.Column<int>(nullable: false),
                    LibraryId = table.Column<int>(nullable: false),
                    BackdropPath = table.Column<string>(nullable: true),
                    Homepage = table.Column<string>(nullable: true),
                    Playback = table.Column<TimeSpan>(nullable: false),
                    TMDbId = table.Column<int>(nullable: false),
                    ImdbId = table.Column<string>(nullable: true),
                    Language = table.Column<string>(nullable: true),
                    OriginalTitle = table.Column<string>(nullable: true),
                    Overview = table.Column<string>(nullable: true),
                    PosterPath = table.Column<string>(nullable: true),
                    ReleaseDate = table.Column<DateTimeOffset>(nullable: false),
                    Runtime = table.Column<string>(nullable: true),
                    TagLine = table.Column<string>(nullable: true),
                    genre = table.Column<int>(nullable: false),
                    type = table.Column<int>(nullable: false),
                    VoteAverage = table.Column<long>(nullable: false),
                    TitleOfMovie = table.Column<string>(nullable: true),
                    GridPosterImageSource = table.Column<string>(nullable: true),
                    BackdropImageSource = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movie", x => x.MovieId);
                    table.ForeignKey(
                        "FK_Movie_Libraries_LibraryId",
                        x => x.LibraryId,
                        "Libraries",
                        "LibraryId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_Libraries_UserId",
                "Libraries",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_Movie_LibraryId",
                "Movie",
                "LibraryId");

            migrationBuilder.CreateIndex(
                "IX_Users_UserName",
                "Users",
                "UserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Movie");

            migrationBuilder.DropTable(
                "Libraries");

            migrationBuilder.DropTable(
                "Users");
        }
    }
}