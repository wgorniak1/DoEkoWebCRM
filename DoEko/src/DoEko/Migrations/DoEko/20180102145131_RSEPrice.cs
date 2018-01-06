using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class RSEPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RSEPriceRules",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    SurveyType = table.Column<int>(nullable: false),
                    RSEType = table.Column<int>(nullable: false),
                    Number = table.Column<double>(nullable: false),
                    NetPrice = table.Column<decimal>(nullable: false),
                    Unit = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RSEPriceRules", x => new { x.ProjectId, x.SurveyType, x.RSEType, x.Number });
                    table.ForeignKey(
                        name: "FK_RSEPriceRules_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RSEPriceTaxRules",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    SurveyType = table.Column<int>(nullable: false),
                    RSEType = table.Column<int>(nullable: false),
                    InstallationLocalization = table.Column<int>(nullable: false),
                    BuildingPurpose = table.Column<int>(nullable: false),
                    UsableAreaMin = table.Column<double>(nullable: false),
                    UsableAreaMax = table.Column<double>(nullable: false),
                    VAT = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RSEPriceTaxRules", x => new { x.ProjectId, x.SurveyType, x.RSEType, x.InstallationLocalization, x.BuildingPurpose, x.UsableAreaMin, x.UsableAreaMax });
                    table.ForeignKey(
                        name: "FK_RSEPriceTaxRules_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RSEPriceRules");

            migrationBuilder.DropTable(
                name: "RSEPriceTaxRules");
        }
    }
}
