using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class FKCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Contract_Companies_CompanyId",
            //    table: "Contract");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Project_Companies_CompanyId",
            //    table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_CompanyId",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Contract_CompanyId",
                table: "Contract");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Contract");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Project",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Contract",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Project_CompanyId",
                table: "Project",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_CompanyId",
                table: "Contract",
                column: "CompanyId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Contract_Companies_CompanyId",
            //    table: "Contract",
            //    column: "CompanyId",
            //    principalTable: "Companies",
            //    principalColumn: "CompanyId",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Project_Companies_CompanyId",
            //    table: "Project",
            //    column: "CompanyId",
            //    principalTable: "Companies",
            //    principalColumn: "CompanyId",
            //    onDelete: ReferentialAction.Cascade);
        }
    }
}
