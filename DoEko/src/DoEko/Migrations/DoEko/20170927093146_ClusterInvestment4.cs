using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class ClusterInvestment4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractId",
                table: "ClusterInvestments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ClusterInvestments_ContractId",
                table: "ClusterInvestments",
                column: "ContractId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClusterInvestments_Contract_ContractId",
                table: "ClusterInvestments",
                column: "ContractId",
                principalTable: "Contract",
                principalColumn: "ContractId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClusterInvestments_Contract_ContractId",
                table: "ClusterInvestments");

            migrationBuilder.DropIndex(
                name: "IX_ClusterInvestments_ContractId",
                table: "ClusterInvestments");

            migrationBuilder.DropColumn(
                name: "ContractId",
                table: "ClusterInvestments");
        }
    }
}
