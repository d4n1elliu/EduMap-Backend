using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduMap_Backend.Migrations
{
    /// <inheritdoc />
    public partial class RefactorMentorProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "Reviews",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "Studies",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "University",
                table: "MentorProfiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "MentorProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "MentorProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Reviews",
                table: "MentorProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "MentorProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Studies",
                table: "MentorProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "University",
                table: "MentorProfiles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
