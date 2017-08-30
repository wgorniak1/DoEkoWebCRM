using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DoEko.Migrations.DoEko
{
    public partial class Payroll_1st : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "BusinessPartners",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmployeesBasicPay",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                    Number = table.Column<double>(nullable: false),
                    Rate = table.Column<double>(nullable: false),
                    ShortDescription = table.Column<string>(nullable: true),
                    Unit = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesBasicPay", x => new { x.EmployeeId, x.Start, x.End, x.Code });
                    table.ForeignKey(
                        name: "FK_EmployeesBasicPay_BusinessPartners_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "BusinessPartners",
                        principalColumn: "BusinessPartnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmployeesUsers",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeesUsers", x => new { x.EmployeeId, x.Start, x.End, x.UserId });
                    table.ForeignKey(
                        name: "FK_EmployeesUsers_BusinessPartners_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "BusinessPartners",
                        principalColumn: "BusinessPartnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WageTypeCatalog",
                columns: table => new
                {
                    WageTypeDefinitionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    ShortDescription = table.Column<string>(nullable: true),
                    Number = table.Column<double>(nullable: false),
                    Unit = table.Column<int>(nullable: false),
                    Rate = table.Column<double>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    Currency = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WageTypeCatalog", x => x.WageTypeDefinitionId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeesBasicPay");

            migrationBuilder.DropTable(
                name: "EmployeesUsers");

            migrationBuilder.DropTable(
                name: "WageTypeCatalog");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "BusinessPartners");
        }
    }
}
