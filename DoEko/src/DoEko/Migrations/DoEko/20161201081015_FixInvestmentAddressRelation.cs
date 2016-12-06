using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class FixInvestmentAddressRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investment_Address_AddressId",
                table: "Investment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceLists",
                table: "PriceLists");

            migrationBuilder.AddColumn<int>(
                name: "RSEType",
                table: "PriceLists",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceLists",
                table: "PriceLists",
                columns: new[] { "StateId", "DistrictId", "CommuneId", "CommuneType", "ValidFrom", "ValidTo", "SurveyType", "RSEType" });

            migrationBuilder.AddForeignKey(
                name: "FK_Investment_Address_AddressId",
                table: "Investment",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Investment_Address_AddressId",
                table: "Investment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceLists",
                table: "PriceLists");

            migrationBuilder.DropColumn(
                name: "RSEType",
                table: "PriceLists");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceLists",
                table: "PriceLists",
                columns: new[] { "StateId", "DistrictId", "CommuneId", "CommuneType", "ValidFrom", "ValidTo", "SurveyType" });

            migrationBuilder.AddForeignKey(
                name: "FK_Investment_Address_AddressId",
                table: "Investment",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
