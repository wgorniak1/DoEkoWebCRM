using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using DoEko.Models.DoEko.Survey;
using DoEko.Models.DoEko;

namespace DoEko.Migrations.DoEko
{
    public partial class SurveyAddPlanInstall : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstallationLocalization",
                table: "SurveyDetBuilding");

            migrationBuilder.DropColumn(
                name: "OnWallPlacementAvailable",
                table: "SurveyDetBuilding");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "SurveyDetBuilding");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SurveyDetBuilding");

            migrationBuilder.DropColumn(
                name: "Configuration",
                table: "Survey");

            migrationBuilder.CreateTable(
                name: "SurveyDetPlannedInstall",
                columns: table => new
                {
                    SurveyId = table.Column<Guid>(nullable: false),
                    Configuration = table.Column<int>(nullable: false),
                    Localization = table.Column<int>(nullable: false),
                    OnWallPlacementAvailable = table.Column<bool>(nullable: false),
                    Purpose = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyDetPlannedInstall", x => x.SurveyId);
                    table.ForeignKey(
                        name: "FK_SurveyDetPlannedInstall_Survey_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Survey",
                        principalColumn: "SurveyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<int>(
                name: "Stage",
                table: "SurveyDetBuilding",
                nullable: false,
                defaultValue: BuildingStage.Completed);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetPlannedInstall_SurveyId",
                table: "SurveyDetPlannedInstall",
                column: "SurveyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stage",
                table: "SurveyDetBuilding");

            migrationBuilder.DropTable(
                name: "SurveyDetPlannedInstall");

            migrationBuilder.AddColumn<int>(
                name: "InstallationLocalization",
                table: "SurveyDetBuilding",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "OnWallPlacementAvailable",
                table: "SurveyDetBuilding",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Purpose",
                table: "SurveyDetBuilding",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "SurveyDetBuilding",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Configuration",
                table: "Survey",
                nullable: true);
        }
    }
}
