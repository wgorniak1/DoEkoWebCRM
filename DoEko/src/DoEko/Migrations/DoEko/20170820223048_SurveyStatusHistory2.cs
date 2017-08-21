using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class SurveyStatusHistory2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SurveyStatusHistory",
                table: "SurveyStatusHistory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SurveyStatusHistory",
                table: "SurveyStatusHistory",
                columns: new[] { "SurveyId", "Start" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SurveyStatusHistory",
                table: "SurveyStatusHistory");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SurveyStatusHistory",
                table: "SurveyStatusHistory",
                column: "SurveyId");
        }
    }
}
