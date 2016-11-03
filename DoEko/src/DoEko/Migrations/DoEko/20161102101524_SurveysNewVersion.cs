using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class SurveysNewVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RSEType",
                table: "Survey",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PlotNumber",
                table: "Investment",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LandRegisterNo",
                table: "Investment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RSEType",
                table: "Survey");

            migrationBuilder.AlterColumn<string>(
                name: "PlotNumber",
                table: "Investment",
                maxLength: 19,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LandRegisterNo",
                table: "Investment",
                maxLength: 15,
                nullable: true);
        }
    }
}
