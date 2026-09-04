using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeTrail.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonYouTubeVideoId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YouTubeVideoId",
                table: "Lessons",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YouTubeVideoId",
                table: "Lessons");
        }
    }
}
