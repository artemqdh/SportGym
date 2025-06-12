using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsGym.Migrations
{
    /// <inheritdoc />
    public partial class AddDateColumnToTrainingSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "Trainings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "Trainings");
        }
    }
}
