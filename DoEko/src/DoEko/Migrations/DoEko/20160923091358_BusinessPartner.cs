using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DoEko.Migrations.DoEko
{
    public partial class BusinessPartner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvestmentOwner");

            migrationBuilder.DropTable(
                name: "Survey");

            migrationBuilder.DropTable(
                name: "Owner");

            migrationBuilder.DropTable(
                name: "Investment");

            migrationBuilder.CreateTable(
                name: "BusinessPartners",
                columns: table => new
                {
                    BusinessPartnerId = table.Column<Guid>(nullable: false),
                    AddressId = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: false),
                    TaxId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 30, nullable: true),
                    Name2 = table.Column<string>(maxLength: 30, nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: true),
                    FirstName = table.Column<string>(maxLength: 30, nullable: true),
                    IdNumber = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(maxLength: 30, nullable: true),
                    Pesel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessPartners", x => x.BusinessPartnerId);
                    table.ForeignKey(
                        name: "FK_BusinessPartners_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddressId = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    KRSId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 30, nullable: false),
                    Name2 = table.Column<string>(maxLength: 30, nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    RegonId = table.Column<string>(nullable: true),
                    TaxId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyId);
                    table.ForeignKey(
                        name: "FK_Companies_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Project",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Contract",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Project_CompanyId",
                table: "Project",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_CompanyId",
                table: "Contract",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPartners_AddressId",
                table: "BusinessPartners",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_AddressId",
                table: "Companies",
                column: "AddressId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Contract_Companies_CompanyId",
            //    table: "Contract",
            //    column: "CompanyId",
            //    principalTable: "Companies",
            //    principalColumn: "CompanyId",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Project_Companies_CompanyId",
            //    table: "Project",
            //    column: "CompanyId",
            //    principalTable: "Companies",
            //    principalColumn: "CompanyId",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Companies_CompanyId",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Companies_CompanyId",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_CompanyId",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Contract_CompanyId",
                table: "Contract");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Contract");

            migrationBuilder.DropTable(
                name: "BusinessPartners");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.CreateTable(
                name: "Investment",
                columns: table => new
                {
                    InvestmentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddressId = table.Column<int>(nullable: false),
                    ContractId = table.Column<int>(nullable: false),
                    InspectionStatus = table.Column<int>(nullable: false),
                    LandRegisterNo = table.Column<string>(maxLength: 11, nullable: true),
                    PlotNumber = table.Column<string>(maxLength: 11, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Investment", x => x.InvestmentId);
                    table.ForeignKey(
                        name: "FK_Investment_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Investment_Contract_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contract",
                        principalColumn: "ContractId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Owner",
                columns: table => new
                {
                    OwnerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AddressId = table.Column<int>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: false),
                    TaxId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 30, nullable: true),
                    Name2 = table.Column<string>(maxLength: 30, nullable: true),
                    FirstName = table.Column<string>(maxLength: 30, nullable: true),
                    IdNumber = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(maxLength: 30, nullable: true),
                    Pesel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owner", x => x.OwnerId);
                    table.ForeignKey(
                        name: "FK_Owner_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Survey",
                columns: table => new
                {
                    SurveyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InvestmentId = table.Column<int>(nullable: true),
                    version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Survey", x => x.SurveyId);
                    table.ForeignKey(
                        name: "FK_Survey_Investment_InvestmentId",
                        column: x => x.InvestmentId,
                        principalTable: "Investment",
                        principalColumn: "InvestmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentOwner",
                columns: table => new
                {
                    InvestmentId = table.Column<int>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
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
                        name: "FK_InvestmentOwner_Owner_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Owner",
                        principalColumn: "OwnerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Investment_AddressId",
                table: "Investment",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Investment_ContractId",
                table: "Investment",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentOwner_InvestmentId",
                table: "InvestmentOwner",
                column: "InvestmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentOwner_OwnerId",
                table: "InvestmentOwner",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Owner_AddressId",
                table: "Owner",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Survey_InvestmentId",
                table: "Survey",
                column: "InvestmentId");
        }
    }
}
