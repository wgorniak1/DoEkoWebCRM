using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class InvestmentOwners : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvestmentOwner",
                columns: table => new
                {
                    InvestmentId = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<Guid>(nullable: false),
                    Sponsor = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentOwner", x => new { x.InvestmentId, x.OwnerId });
                    table.ForeignKey(
                        name: "FK_InvestmentOwner_Investment_InvestmentId",
                        column: x => x.InvestmentId,
                        principalTable: "Investment",
                        principalColumn: "InvestmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvestmentOwner_BusinessPartners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "BusinessPartners",
                        principalColumn: "BusinessPartnerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentOwner_InvestmentId",
                table: "InvestmentOwner",
                column: "InvestmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentOwner_OwnerId",
                table: "InvestmentOwner",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvestmentOwner");
        }
    }
}
