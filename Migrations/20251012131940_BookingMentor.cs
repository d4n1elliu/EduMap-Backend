using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduMap_Backend.Migrations
{
    /// <inheritdoc />
    public partial class BookingMentor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Bookings_MentorId",
                table: "Bookings",
                column: "MentorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_MentorId",
                table: "Bookings",
                column: "MentorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_MentorId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_MentorId",
                table: "Bookings");
        }
    }
}
