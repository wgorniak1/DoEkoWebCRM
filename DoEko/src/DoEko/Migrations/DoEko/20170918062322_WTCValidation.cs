using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class WTCValidation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tests1",
                table: "tests1");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tests",
                table: "tests");

            migrationBuilder.RenameTable(
                name: "tests1",
                newName: "Tests1");

            migrationBuilder.RenameTable(
                name: "tests",
                newName: "Tests");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "WageTypeCatalog",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "WageTypeCatalog",
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AmountMandatory",
                table: "WageTypeCatalog",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RateMandatory",
                table: "WageTypeCatalog",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tests1",
                table: "Tests1",
                column: "PaymentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tests",
                table: "Tests",
                column: "PaymentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tests1",
                table: "Tests1");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tests",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "AmountMandatory",
                table: "WageTypeCatalog");

            migrationBuilder.DropColumn(
                name: "RateMandatory",
                table: "WageTypeCatalog");

            migrationBuilder.RenameTable(
                name: "Tests1",
                newName: "tests1");

            migrationBuilder.RenameTable(
                name: "Tests",
                newName: "tests");

            migrationBuilder.AlterColumn<string>(
                name: "ShortDescription",
                table: "WageTypeCatalog",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "WageTypeCatalog",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 4,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tests1",
                table: "tests1",
                column: "PaymentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tests",
                table: "tests",
                column: "PaymentId");
        }
    }
}
