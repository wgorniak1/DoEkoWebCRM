using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class Survey1stattempt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Survey",
                columns: table => new
                {
                    SurveyId = table.Column<Guid>(nullable: false),
                    InvestmentId = table.Column<Guid>(nullable: false),
                    IsPaid = table.Column<bool>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Survey", x => x.SurveyId);
                    table.ForeignKey(
                        name: "FK_Survey_Investment_InvestmentId",
                        column: x => x.InvestmentId,
                        principalTable: "Investment",
                        principalColumn: "InvestmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Survey_InvestmentId",
                table: "Survey",
                column: "InvestmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Survey");
        }
    }
}
