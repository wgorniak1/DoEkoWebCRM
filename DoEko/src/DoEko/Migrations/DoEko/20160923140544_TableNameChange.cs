using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class TableNameChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_Address_AddressId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Companies_CompanyId",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Companies_CompanyId",
                table: "Project");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Companies",
                table: "Companies");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Company",
                table: "Companies",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_Address_AddressId",
                table: "Companies",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Company_CompanyId",
                table: "Contract",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Company_CompanyId",
                table: "Project",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.RenameIndex(
                name: "IX_Companies_AddressId",
                table: "Companies",
                newName: "IX_Company_AddressId");

            migrationBuilder.RenameTable(
                name: "Companies",
                newName: "Company");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_Address_AddressId",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Company_CompanyId",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Company_CompanyId",
                table: "Project");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Company",
                table: "Company");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Companies",
                table: "Company",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_Address_AddressId",
                table: "Company",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Companies_CompanyId",
                table: "Contract",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Companies_CompanyId",
                table: "Project",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.RenameIndex(
                name: "IX_Company_AddressId",
                table: "Company",
                newName: "IX_Companies_AddressId");

            migrationBuilder.RenameTable(
                name: "Company",
                newName: "Companies");
        }
    }
}
