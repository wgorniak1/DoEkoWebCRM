using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class Payment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ChangedAt = table.Column<DateTime>(nullable: false),
                    ChangedBy = table.Column<Guid>(nullable: false),
                    ContractId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true),
                    ParentType = table.Column<string>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                });

            migrationBuilder.AddColumn<Guid>(
                name: "InvestmentId",
                table: "Payment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Payment",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.CreateIndex(
                name: "IX_Payment_InvestmentId",
                table: "Payment",
                column: "InvestmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Investment_InvestmentId",
                table: "Payment",
                column: "InvestmentId",
                principalTable: "Investment",
                principalColumn: "InvestmentId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Investment_InvestmentId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_InvestmentId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "InvestmentId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Payment");

            migrationBuilder.DropTable(
                name: "File");
        }
    }
}
