using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using DoEko.Models.DoEko;

namespace DoEko.Migrations.DoEko
{
    public partial class FinalSurveyDefinition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CentralHeatingFuel",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "CentralHeatingType",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "CentralHeatingTypeOther",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "HotWaterFuel",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "HotWaterType",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "BusinessActivity",
                table: "SurveyDetBuilding");

            migrationBuilder.DropColumn(
                name: "CompletionYear",
                table: "SurveyDetBuilding");

            migrationBuilder.DropColumn(
                name: "HeatedArea",
                table: "SurveyDetBuilding");

            migrationBuilder.DropColumn(
                name: "NumberOfOccupants",
                table: "SurveyDetBuilding");

            migrationBuilder.DropColumn(
                name: "Stage",
                table: "SurveyDetBuilding");

            migrationBuilder.DropColumn(
                name: "TotalArea",
                table: "SurveyDetBuilding");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SurveyDetBuilding");

            migrationBuilder.DropColumn(
                name: "UsableArea",
                table: "SurveyDetBuilding");

            migrationBuilder.AddColumn<int>(
                name: "BusinessActivity",
                table: "Investment",
                nullable: false,
                defaultValue: BusinessActivity.None);

            migrationBuilder.AddColumn<int>(
                name: "CentralHeatingFuel",
                table: "Investment",
                nullable: false,
                defaultValue: FuelType.NotApplicable);

            migrationBuilder.AddColumn<int>(
                name: "CentralHeatingType",
                table: "Investment",
                nullable: false,
                defaultValue: CentralHeatingType.None);

            migrationBuilder.AddColumn<string>(
                name: "CentralHeatingTypeOther",
                table: "Investment",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "CompletionYear",
                table: "Investment",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<double>(
                name: "HeatedArea",
                table: "Investment",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "HotWaterFuel",
                table: "Investment",
                nullable: false,
                defaultValue: FuelType.NotApplicable);

            migrationBuilder.AddColumn<int>(
                name: "HotWaterType",
                table: "Investment",
                nullable: false,
                defaultValue: HotWaterType.None);

            migrationBuilder.AddColumn<short>(
                name: "NumberOfOccupants",
                table: "Investment",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "Stage",
                table: "Investment",
                nullable: false,
                defaultValue: BuildingStage.Completed);

            migrationBuilder.AddColumn<double>(
                name: "TotalArea",
                table: "Investment",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Investment",
                nullable: false,
                defaultValue: BuildingType.DetachedHouse);

            migrationBuilder.AddColumn<double>(
                name: "UsableArea",
                table: "Investment",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessActivity",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "CentralHeatingFuel",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "CentralHeatingType",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "CentralHeatingTypeOther",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "CompletionYear",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "HeatedArea",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "HotWaterFuel",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "HotWaterType",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "NumberOfOccupants",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "Stage",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "TotalArea",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "UsableArea",
                table: "Investment");

            migrationBuilder.AddColumn<int>(
                name: "CentralHeatingFuel",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CentralHeatingType",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CentralHeatingTypeOther",
                table: "SurveyDetEnergyAudit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HotWaterFuel",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HotWaterType",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BusinessActivity",
                table: "SurveyDetBuilding",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "CompletionYear",
                table: "SurveyDetBuilding",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<double>(
                name: "HeatedArea",
                table: "SurveyDetBuilding",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<short>(
                name: "NumberOfOccupants",
                table: "SurveyDetBuilding",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<int>(
                name: "Stage",
                table: "SurveyDetBuilding",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TotalArea",
                table: "SurveyDetBuilding",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "SurveyDetBuilding",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "UsableArea",
                table: "SurveyDetBuilding",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
