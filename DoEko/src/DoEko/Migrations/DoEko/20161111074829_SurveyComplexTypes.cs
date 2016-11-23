using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class SurveyComplexTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SurveyDetAirCond",
                columns: table => new
                {
                    SurveyId = table.Column<Guid>(nullable: false),
                    Exists = table.Column<bool>(nullable: false),
                    MechVentilationExists = table.Column<bool>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    isPlanned = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyDetAirCond", x => x.SurveyId);
                    table.ForeignKey(
                        name: "FK_SurveyDetAirCond_Survey_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Survey",
                        principalColumn: "SurveyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetAirCond_SurveyId",
                table: "SurveyDetAirCond",
                column: "SurveyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SurveyDetAirCond");
        }
    }
}
