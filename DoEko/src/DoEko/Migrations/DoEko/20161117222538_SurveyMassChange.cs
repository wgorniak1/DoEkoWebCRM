using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using DoEko.Models.DoEko.Survey;

namespace DoEko.Migrations.DoEko
{
    public partial class SurveyMassChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirePlaceWithWater",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "IsDoorSizeEnough",
                table: "SurveyDetBoilerRoom");

            migrationBuilder.DropColumn(
                name: "OnWallPlacementAvailable",
                table: "Survey");

            migrationBuilder.AddColumn<string>(
                name: "OtherInstallationTypw",
                table: "SurveyDetGround",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CHIsHPOnlySource",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CHRadiantFloorAreaPerc",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CentralHeatingTypeOther",
                table: "SurveyDetEnergyAudit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ComplexAgreement",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PVIsGround",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PhaseCount",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: PhaseCount.One);

            migrationBuilder.AddColumn<double>(
                name: "HeatedArea",
                table: "SurveyDetBuilding",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "OnWallPlacementAvailable",
                table: "SurveyDetBuilding",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TechnologyType",
                table: "SurveyDetBuilding",
                nullable: false,
                defaultValue: BuildTechnologyType.Type_1);

            migrationBuilder.AddColumn<double>(
                name: "DoorHeight",
                table: "SurveyDetBoilerRoom",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ChangedAt",
                table: "Survey",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "Survey",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Survey",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Survey",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstEditAt",
                table: "Survey",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "FirstEditBy",
                table: "Survey",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "FreeCommments",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlotAreaNumber",
                table: "Investment",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PriorityIndex",
                table: "Investment",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "PostOfficeLocation",
                table: "Address",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PowerCompanyName",
                table: "SurveyDetEnergyAudit",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "CancelComments",
                table: "Survey",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherInstallationTypw",
                table: "SurveyDetGround");

            migrationBuilder.DropColumn(
                name: "CHIsHPOnlySource",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "CHRadiantFloorAreaPerc",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "CentralHeatingTypeOther",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "ComplexAgreement",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "PVIsGround",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "PhaseCount",
                table: "SurveyDetEnergyAudit");

            migrationBuilder.DropColumn(
                name: "HeatedArea",
                table: "SurveyDetBuilding");

            migrationBuilder.DropColumn(
                name: "OnWallPlacementAvailable",
                table: "SurveyDetBuilding");

            migrationBuilder.DropColumn(
                name: "TechnologyType",
                table: "SurveyDetBuilding");

            migrationBuilder.DropColumn(
                name: "DoorHeight",
                table: "SurveyDetBoilerRoom");

            migrationBuilder.DropColumn(
                name: "ChangedAt",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "FirstEditAt",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "FirstEditBy",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "FreeCommments",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "PlotAreaNumber",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "PriorityIndex",
                table: "Investment");

            migrationBuilder.DropColumn(
                name: "PostOfficeLocation",
                table: "Address");

            migrationBuilder.AddColumn<bool>(
                name: "FirePlaceWithWater",
                table: "SurveyDetEnergyAudit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDoorSizeEnough",
                table: "SurveyDetBoilerRoom",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OnWallPlacementAvailable",
                table: "Survey",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PowerCompanyName",
                table: "SurveyDetEnergyAudit",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CancelComments",
                table: "Survey",
                nullable: true);
        }
    }
}
