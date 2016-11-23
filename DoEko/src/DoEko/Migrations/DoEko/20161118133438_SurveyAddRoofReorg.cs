using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using DoEko.Models.DoEko.Survey;

namespace DoEko.Migrations.DoEko
{
    public partial class SurveyAddRoofReorg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SurveyDetRoof_SurveyId",
                table: "SurveyDetRoof");

            migrationBuilder.DropColumn(
                name: "OtherInstallationTypw",
                table: "SurveyDetGround");

            migrationBuilder.DropColumn(
                name: "HWCirculationInstalled",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "HWInstalled",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "HWPressureReductorExists",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "PVIsGround",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "PVPowerLevel",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "GroundedPowerSupply",
                table: "SurveyDetBoilerRoom");

            migrationBuilder.DropTable(
                name: "SurveyDetRoofPlane");

            migrationBuilder.CreateTable(
                name: "SurveyDetWall",
                columns: table => new
                {
                    SurveyId = table.Column<Guid>(nullable: false),
                    Azimuth = table.Column<double>(nullable: false),
                    Height = table.Column<double>(nullable: false),
                    UsableArea = table.Column<double>(nullable: false),
                    Width = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyDetWall", x => x.SurveyId);
                    table.ForeignKey(
                        name: "FK_SurveyDetWall_Survey_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Survey",
                        principalColumn: "SurveyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<double>(
                name: "BuildingHeight",
                table: "SurveyDetRoof",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EdgeLength",
                table: "SurveyDetRoof",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Length",
                table: "SurveyDetRoof",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "OkapHeight",
                table: "SurveyDetRoof",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RidgeWeight",
                table: "SurveyDetRoof",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RoofLength",
                table: "SurveyDetRoof",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "RoofMaterial",
                table: "SurveyDetRoof",
                nullable: false,
                defaultValue: RoofMaterial.Material_1);

            migrationBuilder.AddColumn<double>(
                name: "SlopeAngle",
                table: "SurveyDetRoof",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SurfaceArea",
                table: "SurveyDetRoof",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SurfaceAzimuth",
                table: "SurveyDetRoof",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Width",
                table: "SurveyDetRoof",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "OtherInstallationType",
                table: "SurveyDetGround",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ENAdditionalConsMeter",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ENIsGround",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "ENPowerLevel",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "HWCirculationInstalled",
                table: "SurveyDetBoilerRoom",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HWInstalled",
                table: "SurveyDetBoilerRoom",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HWPressureReductorExists",
                table: "SurveyDetBoilerRoom",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ThreePowerSuppliesExists",
                table: "SurveyDetBoilerRoom",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetRoof_SurveyId",
                table: "SurveyDetRoof",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetWall_SurveyId",
                table: "SurveyDetWall",
                column: "SurveyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SurveyDetRoof_SurveyId",
                table: "SurveyDetRoof");

            migrationBuilder.DropColumn(
                name: "BuildingHeight",
                table: "SurveyDetRoof");

            migrationBuilder.DropColumn(
                name: "EdgeLength",
                table: "SurveyDetRoof");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "SurveyDetRoof");

            migrationBuilder.DropColumn(
                name: "OkapHeight",
                table: "SurveyDetRoof");

            migrationBuilder.DropColumn(
                name: "RidgeWeight",
                table: "SurveyDetRoof");

            migrationBuilder.DropColumn(
                name: "RoofLength",
                table: "SurveyDetRoof");

            migrationBuilder.DropColumn(
                name: "RoofMaterial",
                table: "SurveyDetRoof");

            migrationBuilder.DropColumn(
                name: "SlopeAngle",
                table: "SurveyDetRoof");

            migrationBuilder.DropColumn(
                name: "SurfaceArea",
                table: "SurveyDetRoof");

            migrationBuilder.DropColumn(
                name: "SurfaceAzimuth",
                table: "SurveyDetRoof");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "SurveyDetRoof");

            migrationBuilder.DropColumn(
                name: "OtherInstallationType",
                table: "SurveyDetGround");

            migrationBuilder.DropColumn(
                name: "ENAdditionalConsMeter",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "ENIsGround",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "ENPowerLevel",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "HWCirculationInstalled",
                table: "SurveyDetBoilerRoom");

            migrationBuilder.DropColumn(
                name: "HWInstalled",
                table: "SurveyDetBoilerRoom");

            migrationBuilder.DropColumn(
                name: "HWPressureReductorExists",
                table: "SurveyDetBoilerRoom");

            migrationBuilder.DropColumn(
                name: "ThreePowerSuppliesExists",
                table: "SurveyDetBoilerRoom");

            migrationBuilder.DropTable(
                name: "SurveyDetWall");

            migrationBuilder.CreateTable(
                name: "SurveyDetRoofPlane",
                columns: table => new
                {
                    RoofPlaneId = table.Column<Guid>(nullable: false),
                    BuildingHeight = table.Column<double>(nullable: false),
                    EdgeLength = table.Column<double>(nullable: false),
                    Length = table.Column<double>(nullable: false),
                    OkapHeight = table.Column<double>(nullable: false),
                    RidgeWeight = table.Column<double>(nullable: false),
                    RoofLength = table.Column<double>(nullable: false),
                    RoofMaterial = table.Column<int>(nullable: false),
                    SlopeAngle = table.Column<double>(nullable: false),
                    SurfaceArea = table.Column<double>(nullable: false),
                    SurfaceAzimuth = table.Column<double>(nullable: false),
                    SurveyId = table.Column<Guid>(nullable: false),
                    Width = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyDetRoofPlane", x => x.RoofPlaneId);
                    table.ForeignKey(
                        name: "FK_SurveyDetRoofPlane_SurveyDetRoof_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "SurveyDetRoof",
                        principalColumn: "SurveyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<string>(
                name: "OtherInstallationTypw",
                table: "SurveyDetGround",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HWCirculationInstalled",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HWInstalled",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HWPressureReductorExists",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PVIsGround",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "PVPowerLevel",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "GroundedPowerSupply",
                table: "SurveyDetBoilerRoom",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetRoof_SurveyId",
                table: "SurveyDetRoof",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetRoofPlane_SurveyId",
                table: "SurveyDetRoofPlane",
                column: "SurveyId");
        }
    }
}
