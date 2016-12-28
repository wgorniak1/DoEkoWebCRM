using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class BusinessPartnerSorting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InsulationTypeOther",
                table: "SurveyDetBuilding",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WallMaterialOther",
                table: "SurveyDetBuilding",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerName1",
                table: "BusinessPartners",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerName2",
                table: "BusinessPartners",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "BusinessPartners",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InsulationTypeOther",
                table: "SurveyDetBuilding");

            migrationBuilder.DropColumn(
                name: "WallMaterialOther",
                table: "SurveyDetBuilding");

            migrationBuilder.DropColumn(
                name: "PartnerName1",
                table: "BusinessPartners");

            migrationBuilder.DropColumn(
                name: "PartnerName2",
                table: "BusinessPartners");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "BusinessPartners",
                nullable: false);
        }
    }
}
