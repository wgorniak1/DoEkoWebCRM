using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class Investment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Investment",
                columns: table => new
                {
                    InvestmentId = table.Column<Guid>(nullable: false),
                    AddressId = table.Column<int>(nullable: false),
                    ContractId = table.Column<int>(nullable: false),
                    InspectionStatus = table.Column<int>(nullable: false),
                    LandRegisterNo = table.Column<string>(maxLength: 11, nullable: true),
                    PlotNumber = table.Column<string>(maxLength: 11, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Investment", x => x.InvestmentId);
                    table.ForeignKey(
                        name: "FK_Investment_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Investment_Contract_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contract",
                        principalColumn: "ContractId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Investment_AddressId",
                table: "Investment",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Investment_ContractId",
                table: "Investment",
                column: "ContractId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Investment");
        }
    }
}
