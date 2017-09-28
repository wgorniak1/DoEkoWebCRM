using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class ClusterContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClusterDetails",
                columns: table => new
                {
                    ContractId = table.Column<int>(nullable: false),
                    CommuneId = table.Column<int>(nullable: false),
                    CommuneType = table.Column<int>(nullable: false),
                    DistrictId = table.Column<int>(nullable: false),
                    StateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClusterDetails", x => x.ContractId);
                    table.ForeignKey(
                        name: "FK_ClusterDetails_Contract_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contract",
                        principalColumn: "ContractId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClusterDetails");
        }
    }
}
