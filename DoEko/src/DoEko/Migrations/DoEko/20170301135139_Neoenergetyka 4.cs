using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class Neoenergetyka4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SurveyResultCalculations",
                columns: table => new
                {
                    SurveyId = table.Column<Guid>(nullable: false),
                    BenzoPirenPercent = table.Column<double>(nullable: false),
                    BenzoPirenValue = table.Column<double>(nullable: false),
                    CHMaxRequiredEn = table.Column<double>(nullable: false),
                    CHRSEWorkingTime = table.Column<double>(nullable: false),
                    CHRSEYearlyProduction = table.Column<double>(nullable: false),
                    CHRequiredEn = table.Column<double>(nullable: false),
                    CHRequiredEnFactor = table.Column<double>(nullable: false),
                    CO2DustEquivPercent = table.Column<double>(nullable: false),
                    CO2DustEquivValue = table.Column<double>(nullable: false),
                    CO2EquivValue = table.Column<double>(nullable: false),
                    CO2Percent = table.Column<double>(nullable: false),
                    CO2Value = table.Column<double>(nullable: false),
                    FinalRSEPower = table.Column<double>(nullable: false),
                    FinalSOLConfig = table.Column<string>(nullable: true),
                    HWRSEWorkingTime = table.Column<double>(nullable: false),
                    HWRSEYearlyProduction = table.Column<double>(nullable: false),
                    HWRequiredEnYearly = table.Column<double>(nullable: false),
                    HeatLossFactor = table.Column<double>(nullable: false),
                    PM10Percent = table.Column<double>(nullable: false),
                    PM10Value = table.Column<double>(nullable: false),
                    PM25Percent = table.Column<double>(nullable: false),
                    PM25Value = table.Column<double>(nullable: false),
                    RSEEfficiency = table.Column<double>(nullable: false),
                    RSEEnYearlyConsumption = table.Column<double>(nullable: false),
                    RSEGrossPrice = table.Column<double>(nullable: false),
                    RSENetPrice = table.Column<double>(nullable: false),
                    RSETax = table.Column<double>(nullable: false),
                    RSEWorkingTime = table.Column<double>(nullable: false),
                    RSEYearlyProduction = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyResultCalculations", x => x.SurveyId);
                    table.ForeignKey(
                        name: "FK_SurveyResultCalculations_Survey_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Survey",
                        principalColumn: "SurveyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResultCalculations_SurveyId",
                table: "SurveyResultCalculations",
                column: "SurveyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SurveyResultCalculations");
        }
    }
}
