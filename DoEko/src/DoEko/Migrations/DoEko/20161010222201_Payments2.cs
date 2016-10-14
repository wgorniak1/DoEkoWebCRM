using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class Payments2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Payment");

            migrationBuilder.AddColumn<int>(
                name: "ContractId",
                table: "Payment",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "NotNeeded",
                table: "Payment",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RseFotovoltaic",
                table: "Payment",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RseHeatPump",
                table: "Payment",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RseSolar",
                table: "Payment",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "NotNeeded",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "RseFotovoltaic",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "RseHeatPump",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "RseSolar",
                table: "Payment");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Payment",
                nullable: false,
                defaultValue: 0);
        }
    }
}
