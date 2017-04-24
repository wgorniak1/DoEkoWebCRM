using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class Neoenergetyka3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BenzoPirenPercent",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "BenzoPirenValue",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CO2DustEquivPercent",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CO2DustEquivValue",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CO2EquivValue",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CO2Percent",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CO2Value",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "FinalRSEPower",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "PM10Percent",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "PM10Value",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "PM25Percent",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "PM25Value",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "RSEEfficiency",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "RSEEnYearlyConsumption",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "RSEGrossPrice",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "RSENetPrice",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "RSETax",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "RSEWorkingTime",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "RSEYearlyProduction",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CHMaxRequiredEn",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CHRSEWorkingTime",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CHRSEYearlyProduction",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CHRequiredEn",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CHRequiredEnFactor",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "HWRSEWorkingTime",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "HWRSEYearlyProduction",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "HWRequiredEnYearly",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "HeatLossFactor",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "FinalSOLConfig",
                table: "Survey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "BenzoPirenPercent",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BenzoPirenValue",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CO2DustEquivPercent",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CO2DustEquivValue",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CO2EquivValue",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CO2Percent",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CO2Value",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FinalRSEPower",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PM10Percent",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PM10Value",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PM25Percent",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PM25Value",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RSEEfficiency",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RSEEnYearlyConsumption",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RSEGrossPrice",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RSENetPrice",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RSETax",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RSEWorkingTime",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RSEYearlyProduction",
                table: "Survey",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CHMaxRequiredEn",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CHRSEWorkingTime",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CHRSEYearlyProduction",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CHRequiredEn",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CHRequiredEnFactor",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "HWRSEWorkingTime",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "HWRSEYearlyProduction",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "HWRequiredEnYearly",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "HeatLossFactor",
                table: "Survey",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FinalSOLConfig",
                table: "Survey",
                nullable: true);
        }
    }
}
