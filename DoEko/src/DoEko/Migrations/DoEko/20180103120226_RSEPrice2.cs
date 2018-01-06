using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class RSEPrice2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RSEPriceRules",
                table: "RSEPriceRules");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "RSEPriceRules",
                newName: "NumberMax");

            migrationBuilder.AddColumn<double>(
                name: "NumberMin",
                table: "RSEPriceRules",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "Multiply",
                table: "RSEPriceRules",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSEPriceRules",
                table: "RSEPriceRules",
                columns: new[] { "ProjectId", "SurveyType", "RSEType", "Unit", "NumberMin", "NumberMax" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_RSEPriceRules",
                table: "RSEPriceRules");

            migrationBuilder.DropColumn(
                name: "NumberMin",
                table: "RSEPriceRules");

            migrationBuilder.DropColumn(
                name: "Multiply",
                table: "RSEPriceRules");

            migrationBuilder.RenameColumn(
                name: "NumberMax",
                table: "RSEPriceRules",
                newName: "Number");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RSEPriceRules",
                table: "RSEPriceRules",
                columns: new[] { "ProjectId", "SurveyType", "RSEType", "Number" });
        }
    }
}
