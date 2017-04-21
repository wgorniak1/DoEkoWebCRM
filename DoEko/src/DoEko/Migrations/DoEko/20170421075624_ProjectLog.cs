using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DoEko.Migrations.DoEko
{
    public partial class ProjectLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ChangedAt",
                table: "Project",
                nullable: false,
                defaultValue: DateTime.UtcNow );

            migrationBuilder.AddColumn<Guid>(
                name: "ChangedBy",
                table: "Project",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Project",
                nullable: false,
                defaultValue: DateTime.UtcNow );

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Project",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangedAt",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ChangedBy",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Project");
        }
    }
}
