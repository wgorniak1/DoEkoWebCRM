using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class RoofPlaneIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SurveyDetRoof",
                table: "SurveyDetRoof");

            migrationBuilder.AddColumn<Guid>(
                name: "RoofPlaneId",
                table: "SurveyDetRoof",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_SurveyDetRoof",
                table: "SurveyDetRoof",
                column: "RoofPlaneId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SurveyDetRoof",
                table: "SurveyDetRoof");

            migrationBuilder.DropColumn(
                name: "RoofPlaneId",
                table: "SurveyDetRoof");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SurveyDetRoof",
                table: "SurveyDetRoof",
                column: "SurveyId");
        }
    }
}
