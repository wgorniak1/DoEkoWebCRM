using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class SurveyStatusHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SurveyResultCalculations_SurveyId",
                table: "SurveyResultCalculations");

            migrationBuilder.DropIndex(
                name: "IX_SurveyDetWall_SurveyId",
                table: "SurveyDetWall");

            migrationBuilder.DropIndex(
                name: "IX_SurveyDetPlannedInstall_SurveyId",
                table: "SurveyDetPlannedInstall");

            migrationBuilder.DropIndex(
                name: "IX_SurveyDetGround_SurveyId",
                table: "SurveyDetGround");

            migrationBuilder.DropIndex(
                name: "IX_SurveyDetEnergyAudit_SurveyId",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropIndex(
                name: "IX_SurveyDetBuilding_SurveyId",
                table: "SurveyDetBuilding");

            migrationBuilder.DropIndex(
                name: "IX_SurveyDetBoilerRoom_SurveyId",
                table: "SurveyDetBoilerRoom");

            migrationBuilder.DropIndex(
                name: "IX_SurveyDetBathroom_SurveyId",
                table: "SurveyDetBathroom");

            migrationBuilder.DropIndex(
                name: "IX_SurveyDetAirCond_SurveyId",
                table: "SurveyDetAirCond");

            migrationBuilder.DropIndex(
                name: "IX_PriceLists_StateId_DistrictId_CommuneId_CommuneType",
                table: "PriceLists");

            migrationBuilder.DropIndex(
                name: "IX_InvestmentOwner_InvestmentId",
                table: "InvestmentOwner");

            migrationBuilder.DropIndex(
                name: "IX_District_StateId",
                table: "District");

            migrationBuilder.DropIndex(
                name: "IX_Commune_StateId_DistrictId",
                table: "Commune");

            migrationBuilder.DropIndex(
                name: "IX_Address_StateId",
                table: "Address");

            migrationBuilder.DropIndex(
                name: "IX_Address_StateId_DistrictId",
                table: "Address");

            migrationBuilder.CreateTable(
                name: "SurveyStatusHistory",
                columns: table => new
                {
                    SurveyId = table.Column<Guid>(nullable: false),
                    End = table.Column<DateTime>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyStatusHistory", x => x.SurveyId);
                    table.ForeignKey(
                        name: "FK_SurveyStatusHistory_Survey_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Survey",
                        principalColumn: "SurveyId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SurveyStatusHistory");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyResultCalculations_SurveyId",
                table: "SurveyResultCalculations",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetWall_SurveyId",
                table: "SurveyDetWall",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetPlannedInstall_SurveyId",
                table: "SurveyDetPlannedInstall",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetGround_SurveyId",
                table: "SurveyDetGround",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetEnergyAudit_SurveyId",
                table: "SurveyDetEnergyAudit",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetBuilding_SurveyId",
                table: "SurveyDetBuilding",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetBoilerRoom_SurveyId",
                table: "SurveyDetBoilerRoom",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetBathroom_SurveyId",
                table: "SurveyDetBathroom",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetAirCond_SurveyId",
                table: "SurveyDetAirCond",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceLists_StateId_DistrictId_CommuneId_CommuneType",
                table: "PriceLists",
                columns: new[] { "StateId", "DistrictId", "CommuneId", "CommuneType" });

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentOwner_InvestmentId",
                table: "InvestmentOwner",
                column: "InvestmentId");

            migrationBuilder.CreateIndex(
                name: "IX_District_StateId",
                table: "District",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Commune_StateId_DistrictId",
                table: "Commune",
                columns: new[] { "StateId", "DistrictId" });

            migrationBuilder.CreateIndex(
                name: "IX_Address_StateId",
                table: "Address",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_StateId_DistrictId",
                table: "Address",
                columns: new[] { "StateId", "DistrictId" });
        }
    }
}
