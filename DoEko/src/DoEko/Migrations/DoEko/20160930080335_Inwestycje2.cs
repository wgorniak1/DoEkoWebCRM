using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class Inwestycje2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PlotNumber",
                table: "Investment",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LandRegisterNo",
                table: "Investment",
                maxLength: 11,
                nullable: true);
        }
    }
}
