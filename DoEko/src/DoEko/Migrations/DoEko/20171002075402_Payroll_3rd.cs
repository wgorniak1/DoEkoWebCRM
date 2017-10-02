using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class Payroll_3rd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PayrollCluster",
                columns: table => new
                {
                    PayrollClusterId = table.Column<Guid>(nullable: false),
                    ChangedAt = table.Column<DateTime>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    EmployeeId = table.Column<Guid>(nullable: false),
                    PeriodFor = table.Column<DateTime>(nullable: false),
                    PeriodIn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollCluster", x => x.PayrollClusterId);
                    table.ForeignKey(
                        name: "FK_PayrollCluster_BusinessPartners_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "BusinessPartners",
                        principalColumn: "BusinessPartnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PayrollResult",
                columns: table => new
                {
                    PayrollResultId = table.Column<Guid>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true),
                    Number = table.Column<double>(nullable: false),
                    PayrollClusterId = table.Column<Guid>(nullable: false),
                    Rate = table.Column<double>(nullable: false),
                    ShortDescription = table.Column<string>(nullable: true),
                    SurveyId = table.Column<Guid>(nullable: false),
                    Unit = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollResult", x => x.PayrollResultId);
                    table.ForeignKey(
                        name: "FK_PayrollResult_PayrollCluster_PayrollClusterId",
                        column: x => x.PayrollClusterId,
                        principalTable: "PayrollCluster",
                        principalColumn: "PayrollClusterId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayrollCluster_EmployeeId",
                table: "PayrollCluster",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayrollResult_PayrollClusterId",
                table: "PayrollResult",
                column: "PayrollClusterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayrollResult");

            migrationBuilder.DropTable(
                name: "PayrollCluster");
        }
    }
}
