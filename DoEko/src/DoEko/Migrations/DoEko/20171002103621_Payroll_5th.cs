using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class Payroll_5th : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractId",
                table: "EmployeesBasicPay",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "EmployeesBasicPay",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "EmployeesBasicPay");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "EmployeesBasicPay");
        }
    }
}
