using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class ClusterContract2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ClusterDetails_StateId_DistrictId_CommuneId_CommuneType",
                table: "ClusterDetails",
                columns: new[] { "StateId", "DistrictId", "CommuneId", "CommuneType" });

            migrationBuilder.AddForeignKey(
                name: "FK_ClusterDetails_State_StateId",
                table: "ClusterDetails",
                column: "StateId",
                principalTable: "State",
                principalColumn: "StateId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClusterDetails_District_StateId_DistrictId",
                table: "ClusterDetails",
                columns: new[] { "StateId", "DistrictId" },
                principalTable: "District",
                principalColumns: new[] { "StateId", "DistrictId" },
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_ClusterDetails_Commune_StateId_DistrictId_CommuneId_CommuneType",
                table: "ClusterDetails",
                columns: new[] { "StateId", "DistrictId", "CommuneId", "CommuneType" },
                principalTable: "Commune",
                principalColumns: new[] { "StateId", "DistrictId", "CommuneId", "Type" },
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClusterDetails_State_StateId",
                table: "ClusterDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ClusterDetails_District_StateId_DistrictId",
                table: "ClusterDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ClusterDetails_Commune_StateId_DistrictId_CommuneId_CommuneType",
                table: "ClusterDetails");

            migrationBuilder.DropIndex(
                name: "IX_ClusterDetails_StateId_DistrictId_CommuneId_CommuneType",
                table: "ClusterDetails");
        }
    }
}
