using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class Neo2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Calculate",
                table: "Investment",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PayrollComment",
                columns: table => new
                {
                    PayrollCommentId = table.Column<Guid>(nullable: false),
                    PayrollResultId = table.Column<Guid>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollComment", x => x.PayrollCommentId);
                    table.ForeignKey(
                        name: "FK_PayrollComment_PayrollResult_PayrollResultId",
                        column: x => x.PayrollResultId,
                        principalTable: "PayrollResult",
                        principalColumn: "PayrollResultId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayrollComment_PayrollResultId",
                table: "PayrollComment",
                column: "PayrollResultId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayrollComment");

            migrationBuilder.DropColumn(
                name: "Calculate",
                table: "Investment");
        }
    }
}
