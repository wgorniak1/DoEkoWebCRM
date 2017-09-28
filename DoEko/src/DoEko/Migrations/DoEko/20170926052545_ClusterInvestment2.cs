using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class ClusterInvestment2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClusterInvestments_State_StateId",
                table: "ClusterInvestments");

            migrationBuilder.DropForeignKey(
                name: "FK_ClusterInvestments_District_StateId_DistrictId",
                table: "ClusterInvestments");

            migrationBuilder.DropForeignKey(
                name: "FK_ClusterInvestments_Commune_StateId_DistrictId_CommuneId_CommuneType",
                table: "ClusterInvestments");

            migrationBuilder.DropIndex(
                name: "IX_ClusterInvestments_StateId_DistrictId_CommuneId_CommuneType",
                table: "ClusterInvestments");

            migrationBuilder.DropColumn(
                name: "ApartmentNo",
                table: "ClusterInvestments");

            migrationBuilder.DropColumn(
                name: "BuildingNo",
                table: "ClusterInvestments");

            migrationBuilder.DropColumn(
                name: "City",
                table: "ClusterInvestments");

            migrationBuilder.DropColumn(
                name: "CommuneId",
                table: "ClusterInvestments");

            migrationBuilder.DropColumn(
                name: "CommuneType",
                table: "ClusterInvestments");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "ClusterInvestments");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "ClusterInvestments");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "ClusterInvestments");

            migrationBuilder.RenameColumn(
                name: "StateId",
                table: "ClusterInvestments",
                newName: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ClusterInvestments_AddressId",
                table: "ClusterInvestments",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClusterInvestments_Address_AddressId",
                table: "ClusterInvestments",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClusterInvestments_Address_AddressId",
                table: "ClusterInvestments");

            migrationBuilder.DropIndex(
                name: "IX_ClusterInvestments_AddressId",
                table: "ClusterInvestments");

            migrationBuilder.RenameColumn(
                name: "AddressId",
                table: "ClusterInvestments",
                newName: "StateId");

            migrationBuilder.AddColumn<string>(
                name: "ApartmentNo",
                table: "ClusterInvestments",
                maxLength: 5,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuildingNo",
                table: "ClusterInvestments",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "ClusterInvestments",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CommuneId",
                table: "ClusterInvestments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CommuneType",
                table: "ClusterInvestments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "ClusterInvestments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "ClusterInvestments",
                maxLength: 6,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "ClusterInvestments",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClusterInvestments_StateId_DistrictId_CommuneId_CommuneType",
                table: "ClusterInvestments",
                columns: new[] { "StateId", "DistrictId", "CommuneId", "CommuneType" });

            migrationBuilder.AddForeignKey(
                name: "FK_ClusterInvestments_State_StateId",
                table: "ClusterInvestments",
                column: "StateId",
                principalTable: "State",
                principalColumn: "StateId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClusterInvestments_District_StateId_DistrictId",
                table: "ClusterInvestments",
                columns: new[] { "StateId", "DistrictId" },
                principalTable: "District",
                principalColumns: new[] { "StateId", "DistrictId" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClusterInvestments_Commune_StateId_DistrictId_CommuneId_CommuneType",
                table: "ClusterInvestments",
                columns: new[] { "StateId", "DistrictId", "CommuneId", "CommuneType" },
                principalTable: "Commune",
                principalColumns: new[] { "StateId", "DistrictId", "CommuneId", "Type" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
