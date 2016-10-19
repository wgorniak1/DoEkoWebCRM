using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class InvestmentDeleteCascadeIOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentOwner_Investment_InvestmentId",
                table: "InvestmentOwner");

            migrationBuilder.AlterColumn<string>(
                name: "TaxId",
                table: "Company",
                maxLength: 13,
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentOwner_Investment_InvestmentId",
                table: "InvestmentOwner",
                column: "InvestmentId",
                principalTable: "Investment",
                principalColumn: "InvestmentId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvestmentOwner_Investment_InvestmentId",
                table: "InvestmentOwner");

            migrationBuilder.AlterColumn<string>(
                name: "TaxId",
                table: "Company",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InvestmentOwner_Investment_InvestmentId",
                table: "InvestmentOwner",
                column: "InvestmentId",
                principalTable: "Investment",
                principalColumn: "InvestmentId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
