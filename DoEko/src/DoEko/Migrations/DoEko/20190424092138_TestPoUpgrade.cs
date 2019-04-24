using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class TestPoUpgrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SurveyEnergy_RSEType",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SurveyHotWater_RSEType",
                table: "Survey",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SurveyEnergy_RSEType",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "SurveyHotWater_RSEType",
                table: "Survey");
        }
    }
}
