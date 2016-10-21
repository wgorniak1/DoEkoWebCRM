using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using DoEko.Models.DoEko;

namespace DoEko.Migrations.DoEko
{
    public partial class SurveyDetailsHW : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "BuildingCompletionYear",
                table: "Survey",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<double>(
                name: "BuildingCurrentEnergyTotal",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<short>(
                name: "BuildingNumberOfHosts",
                table: "Survey",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<double>(
                name: "BuildingOverallArea",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "BuildingState",
                table: "Survey",
                nullable: false,
                defaultValue: BuildingState.Completed);

            migrationBuilder.AddColumn<int>(
                name: "BuildingType",
                table: "Survey",
                nullable: false,
                defaultValue: BuildingType.Business);

            migrationBuilder.AddColumn<int>(
                name: "BuildingType2",
                table: "Survey",
                nullable: false,
                defaultValue: BuildingType2.Type_1);

            migrationBuilder.AddColumn<double>(
                name: "BuildingUsableArea",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "BusinessActivity",
                table: "Survey",
                nullable: false,
                defaultValue: BusinessActivity.Office);

            migrationBuilder.AddColumn<string>(
                name: "CancelComments",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CancelType",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CentralHeatingFuel",
                table: "Survey",
                nullable: false,
                defaultValue: FuelType.Coal);

            migrationBuilder.AddColumn<int>(
                name: "CentralHeatingType",
                table: "Survey",
                nullable: false,
                defaultValue: CentralHeatingType.LiquidFuelHeater);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Survey",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "HotWaterFuel",
                table: "Survey",
                nullable: false,
                defaultValue: FuelType.Coal);

            migrationBuilder.AddColumn<int>(
                name: "HotWaterType",
                table: "Survey",
                nullable: false,
                defaultValue: HotWaterType.SolidFuelHeater);

            migrationBuilder.AddColumn<int>(
                name: "InstallationLocalization",
                table: "Survey",
                nullable: false,
                defaultValue: InstallationLocalization.Roof);

            migrationBuilder.AddColumn<bool>(
                name: "InternetConnectionAvailable",
                table: "Survey",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AirVentilationExists",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Azimuth",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BoilerStationExists",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoilerStationSizeX",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoilerStationSizeY",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BoilerStationSizeZ",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BuildingSizeX",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BuildingSizeY",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BuildingSizeZ",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ChimneysExists",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CirculationExists",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Current",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GroundedSocketsExists",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "InstalationExists",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "InstallationSpace",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LightingRodExists",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PresureRegulator",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RoofEdgeWeight",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RoofHeight",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RoofInclinationAngle",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RoofLightsExists",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoofMaterial",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RoofRidgeWeight",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RoofWidth",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RoofWindowsExists",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TargetHotWaterType",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UnderRoofInstallationExists",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isDoorSizeEnough",
                table: "Survey",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuildingCompletionYear",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "BuildingCurrentEnergyTotal",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "BuildingNumberOfHosts",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "BuildingOverallArea",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "BuildingState",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "BuildingType",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "BuildingType2",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "BuildingUsableArea",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "BusinessActivity",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CancelComments",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CancelType",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CentralHeatingFuel",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CentralHeatingType",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "HotWaterFuel",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "HotWaterType",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "InstallationLocalization",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "InternetConnectionAvailable",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "AirVentilationExists",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "Azimuth",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "BoilerStationExists",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "BoilerStationSizeX",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "BoilerStationSizeY",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "BoilerStationSizeZ",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "BuildingSizeX",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "BuildingSizeY",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "BuildingSizeZ",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "ChimneysExists",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CirculationExists",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "Current",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "GroundedSocketsExists",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "InstalationExists",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "InstallationSpace",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "LightingRodExists",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "PresureRegulator",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "RoofEdgeWeight",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "RoofHeight",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "RoofInclinationAngle",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "RoofLightsExists",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "RoofMaterial",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "RoofRidgeWeight",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "RoofWidth",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "RoofWindowsExists",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "TargetHotWaterType",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "UnderRoofInstallationExists",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "isDoorSizeEnough",
                table: "Survey");
        }
    }
}
