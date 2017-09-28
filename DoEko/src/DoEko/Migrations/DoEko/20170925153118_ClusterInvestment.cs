using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class ClusterInvestment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClusterInvestments",
                columns: table => new
                {
                    ClustInvestmentId = table.Column<Guid>(nullable: false),
                    ApartmentNo = table.Column<string>(maxLength: 5, nullable: true),
                    BuildingNo = table.Column<string>(maxLength: 5, nullable: false),
                    City = table.Column<string>(maxLength: 50, nullable: false),
                    CommuneId = table.Column<int>(nullable: false),
                    CommuneType = table.Column<int>(nullable: false),
                    CompanySize = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 1000, nullable: true),
                    DistrictId = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    EnYearlyConsumption = table.Column<double>(nullable: false),
                    MemberType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    Name2 = table.Column<string>(maxLength: 30, nullable: false),
                    NewInstallation = table.Column<bool>(nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    PostalCode = table.Column<string>(maxLength: 6, nullable: false),
                    PvPower = table.Column<double>(nullable: false),
                    PvYearlyProduction = table.Column<double>(nullable: false),
                    StateId = table.Column<int>(nullable: false),
                    Street = table.Column<string>(maxLength: 50, nullable: true),
                    TaxId = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClusterInvestments", x => x.ClustInvestmentId);
                    table.ForeignKey(
                        name: "FK_ClusterInvestments_State_StateId",
                        column: x => x.StateId,
                        principalTable: "State",
                        principalColumn: "StateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClusterInvestments_District_StateId_DistrictId",
                        columns: x => new { x.StateId, x.DistrictId },
                        principalTable: "District",
                        principalColumns: new[] { "StateId", "DistrictId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClusterInvestments_Commune_StateId_DistrictId_CommuneId_CommuneType",
                        columns: x => new { x.StateId, x.DistrictId, x.CommuneId, x.CommuneType },
                        principalTable: "Commune",
                        principalColumns: new[] { "StateId", "DistrictId", "CommuneId", "Type" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClusterInvestments_StateId_DistrictId_CommuneId_CommuneType",
                table: "ClusterInvestments",
                columns: new[] { "StateId", "DistrictId", "CommuneId", "CommuneType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClusterInvestments");
        }
    }
}
