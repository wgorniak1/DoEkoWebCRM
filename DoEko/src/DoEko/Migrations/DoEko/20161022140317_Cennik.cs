using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class Cennik : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PriceLists",
                columns: table => new
                {
                    StateId = table.Column<int>(nullable: false),
                    DistrictId = table.Column<int>(nullable: false),
                    CommuneId = table.Column<int>(nullable: false),
                    CommuneType = table.Column<int>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: false),
                    ValidTo = table.Column<DateTime>(nullable: false),
                    SurveyType = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceLists", x => new { x.StateId, x.DistrictId, x.CommuneId, x.CommuneType, x.ValidFrom, x.ValidTo, x.SurveyType });
                    table.ForeignKey(
                        name: "FK_PriceLists_Commune_StateId_DistrictId_CommuneId_CommuneType",
                        columns: x => new { x.StateId, x.DistrictId, x.CommuneId, x.CommuneType },
                        principalTable: "Commune",
                        principalColumns: new[] { "StateId", "DistrictId", "CommuneId", "Type" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceLists_StateId_DistrictId_CommuneId_CommuneType",
                table: "PriceLists",
                columns: new[] { "StateId", "DistrictId", "CommuneId", "CommuneType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceLists");
        }
    }
}
