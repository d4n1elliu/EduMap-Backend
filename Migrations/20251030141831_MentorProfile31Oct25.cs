using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduMap_Backend.Migrations
{
    /// <inheritdoc />
    public partial class MentorProfile31Oct25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Latitude",
                table: "MentorProfiles",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Longitude",
                table: "MentorProfiles",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "MentorProfiles");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "MentorProfiles");
        }
    }
}
