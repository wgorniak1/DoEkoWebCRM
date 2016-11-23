using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class SurveyComplexTypesP2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SurveyDetBathroom",
                columns: table => new
                {
                    SurveyId = table.Column<Guid>(nullable: false),
                    BathExsists = table.Column<bool>(nullable: false),
                    BathVolume = table.Column<double>(nullable: false),
                    NumberOfBathrooms = table.Column<short>(nullable: false),
                    ShowerExists = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyDetBathroom", x => x.SurveyId);
                    table.ForeignKey(
                        name: "FK_SurveyDetBathroom_Survey_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Survey",
                        principalColumn: "SurveyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveyDetBoilerRoom",
                columns: table => new
                {
                    SurveyId = table.Column<Guid>(nullable: false),
                    AirVentilationExists = table.Column<bool>(nullable: false),
                    GroundedPowerSupply = table.Column<bool>(nullable: false),
                    Height = table.Column<double>(nullable: false),
                    HighVoltagePowerSupply = table.Column<bool>(nullable: false),
                    IsDoorSizeEnough = table.Column<bool>(nullable: false),
                    IsDryAndWarm = table.Column<bool>(nullable: false),
                    Length = table.Column<double>(nullable: false),
                    RoomExists = table.Column<bool>(nullable: false),
                    Volume = table.Column<double>(nullable: false),
                    Width = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyDetBoilerRoom", x => x.SurveyId);
                    table.ForeignKey(
                        name: "FK_SurveyDetBoilerRoom_Survey_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Survey",
                        principalColumn: "SurveyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveyDetBuilding",
                columns: table => new
                {
                    SurveyId = table.Column<Guid>(nullable: false),
                    BusinessActivity = table.Column<int>(nullable: false),
                    CompletionYear = table.Column<short>(nullable: false),
                    InstallationLocalization = table.Column<int>(nullable: false),
                    InsulationThickness = table.Column<double>(nullable: false),
                    InsulationType = table.Column<int>(nullable: false),
                    NumberOfOccupants = table.Column<short>(nullable: false),
                    Purpose = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TotalArea = table.Column<double>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    UsableArea = table.Column<double>(nullable: false),
                    Volume = table.Column<double>(nullable: false),
                    WallMaterial = table.Column<int>(nullable: false),
                    WallThickness = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyDetBuilding", x => x.SurveyId);
                    table.ForeignKey(
                        name: "FK_SurveyDetBuilding_Survey_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Survey",
                        principalColumn: "SurveyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveyDetEnergyAudit",
                columns: table => new
                {
                    SurveyId = table.Column<Guid>(nullable: false),
                    AdditionalHeatParams = table.Column<string>(nullable: true),
                    AdditionalHeatSource = table.Column<bool>(nullable: false),
                    AverageYearlyFuelConsumption = table.Column<double>(nullable: false),
                    AverageYearlyHeatingCosts = table.Column<decimal>(nullable: false),
                    BoilerMaxTemp = table.Column<double>(nullable: false),
                    BoilerNominalPower = table.Column<double>(nullable: false),
                    BoilerPlannedReplacement = table.Column<bool>(nullable: false),
                    BoilerProductionYear = table.Column<short>(nullable: false),
                    CHFRadiantFloorInstalled = table.Column<bool>(nullable: false),
                    CHRadiatorType = table.Column<int>(nullable: false),
                    CHRadiatorsInstalled = table.Column<bool>(nullable: false),
                    CentralHeatingFuel = table.Column<int>(nullable: false),
                    CentralHeatingType = table.Column<int>(nullable: false),
                    ElectricityAvgMonthlyCost = table.Column<decimal>(nullable: false),
                    ElectricityPower = table.Column<double>(nullable: false),
                    FirePlaceWithWater = table.Column<bool>(nullable: false),
                    HWCirculationInstalled = table.Column<bool>(nullable: false),
                    HWInstalled = table.Column<bool>(nullable: false),
                    HWPressureReductorExists = table.Column<bool>(nullable: false),
                    HWSourcePower = table.Column<double>(nullable: false),
                    HotWaterFuel = table.Column<int>(nullable: false),
                    HotWaterType = table.Column<int>(nullable: false),
                    PVPowerLevel = table.Column<double>(nullable: false),
                    PowerAvgYearlyConsumption = table.Column<double>(nullable: false),
                    PowerCompanyName = table.Column<string>(nullable: true),
                    PowerConsMeterLocation = table.Column<int>(nullable: false),
                    PowerSupplyType = table.Column<int>(nullable: false),
                    TankCoilSize = table.Column<double>(nullable: false),
                    TankExists = table.Column<bool>(nullable: false),
                    TankVolume = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyDetEnergyAudit", x => x.SurveyId);
                    table.ForeignKey(
                        name: "FK_SurveyDetEnergyAudit_Survey_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Survey",
                        principalColumn: "SurveyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveyDetGround",
                columns: table => new
                {
                    SurveyId = table.Column<Guid>(nullable: false),
                    Area = table.Column<double>(nullable: false),
                    FormerMilitary = table.Column<bool>(nullable: false),
                    OtherInstallation = table.Column<bool>(nullable: false),
                    Rocks = table.Column<bool>(nullable: false),
                    SlopeTerrain = table.Column<int>(nullable: false),
                    WetLand = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyDetGround", x => x.SurveyId);
                    table.ForeignKey(
                        name: "FK_SurveyDetGround_Survey_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Survey",
                        principalColumn: "SurveyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveyDetRoof",
                columns: table => new
                {
                    SurveyId = table.Column<Guid>(nullable: false),
                    Chimney = table.Column<bool>(nullable: false),
                    InstallationUnderPlane = table.Column<bool>(nullable: false),
                    LightingProtection = table.Column<bool>(nullable: false),
                    SkyLights = table.Column<bool>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Windows = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyDetRoof", x => x.SurveyId);
                    table.ForeignKey(
                        name: "FK_SurveyDetRoof_Survey_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Survey",
                        principalColumn: "SurveyId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetRoofPlane_SurveyId",
                table: "SurveyDetRoofPlane",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetBathroom_SurveyId",
                table: "SurveyDetBathroom",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetBoilerRoom_SurveyId",
                table: "SurveyDetBoilerRoom",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetBuilding_SurveyId",
                table: "SurveyDetBuilding",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetEnergyAudit_SurveyId",
                table: "SurveyDetEnergyAudit",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetGround_SurveyId",
                table: "SurveyDetGround",
                column: "SurveyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDetRoof_SurveyId",
                table: "SurveyDetRoof",
                column: "SurveyId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SurveyDetRoofPlane");

            migrationBuilder.DropTable(
                name: "SurveyDetBathroom");

            migrationBuilder.DropTable(
                name: "SurveyDetBoilerRoom");

            migrationBuilder.DropTable(
                name: "SurveyDetBuilding");

            migrationBuilder.DropTable(
                name: "SurveyDetEnergyAudit");

            migrationBuilder.DropTable(
                name: "SurveyDetGround");

            migrationBuilder.DropTable(
                name: "SurveyDetRoof");
        }
    }
}
