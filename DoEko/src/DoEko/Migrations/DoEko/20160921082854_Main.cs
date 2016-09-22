using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DoEko.Migrations.DoEko
{
    public partial class Main : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    CountryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(maxLength: 2, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.CountryId);
                    table.UniqueConstraint("AK_Country_Key", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "State",
                columns: table => new
                {
                    StateId = table.Column<int>(nullable: false),
                    CapitalCity = table.Column<string>(maxLength: 50, nullable: false),
                    Key = table.Column<string>(maxLength: 5, nullable: false),
                    Text = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State", x => x.StateId);
                    table.UniqueConstraint("AK_State_Key", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    EndDate = table.Column<DateTime>(nullable: false),
                    ParentProjectId = table.Column<int>(nullable: true),
                    RealEnd = table.Column<DateTime>(nullable: true),
                    RealStart = table.Column<DateTime>(nullable: true),
                    ShortDescription = table.Column<string>(maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    UEFundsLevel = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK_Project_Project_ParentProjectId",
                        column: x => x.ParentProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "District",
                columns: table => new
                {
                    StateId = table.Column<int>(nullable: false),
                    DistrictId = table.Column<int>(nullable: false),
                    Text = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_District", x => new { x.StateId, x.DistrictId });
                    table.ForeignKey(
                        name: "FK_District_State_StateId",
                        column: x => x.StateId,
                        principalTable: "State",
                        principalColumn: "StateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contract",
                columns: table => new
                {
                    ContractId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContractDate = table.Column<DateTime>(nullable: false),
                    FullfilmentDate = table.Column<DateTime>(nullable: true),
                    Number = table.Column<string>(maxLength: 20, nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    ShortDescription = table.Column<string>(maxLength: 50, nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contract", x => x.ContractId);
                    table.ForeignKey(
                        name: "FK_Contract_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Commune",
                columns: table => new
                {
                    StateId = table.Column<int>(nullable: false),
                    DistrictId = table.Column<int>(nullable: false),
                    CommuneId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Text = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commune", x => new { x.StateId, x.DistrictId, x.CommuneId, x.Type });
                    table.ForeignKey(
                        name: "FK_Commune_District_StateId_DistrictId",
                        columns: x => new { x.StateId, x.DistrictId },
                        principalTable: "District",
                        principalColumns: new[] { "StateId", "DistrictId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    AddressId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApartmentNo = table.Column<string>(maxLength: 5, nullable: true),
                    BuildingNo = table.Column<string>(maxLength: 5, nullable: false),
                    City = table.Column<string>(maxLength: 50, nullable: false),
                    CommuneId = table.Column<int>(nullable: false),
                    CommuneType = table.Column<int>(nullable: false),
                    CountryId = table.Column<int>(nullable: false),
                    DistrictId = table.Column<int>(nullable: false),
                    PostalCode = table.Column<string>(maxLength: 10, nullable: false),
                    StateId = table.Column<int>(nullable: false),
                    Street = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Address_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Address_State_StateId",
                        column: x => x.StateId,
                        principalTable: "State",
                        principalColumn: "StateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Address_District_StateId_DistrictId",
                        columns: x => new { x.StateId, x.DistrictId },
                        principalTable: "District",
                        principalColumns: new[] { "StateId", "DistrictId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Address_Commune_StateId_DistrictId_CommuneId_CommuneType",
                        columns: x => new { x.StateId, x.DistrictId, x.CommuneId, x.CommuneType },
                        principalTable: "Commune",
                        principalColumns: new[] { "StateId", "DistrictId", "CommuneId", "Type" },
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_Address_CountryId",
                table: "Address",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_StateId",
                table: "Address",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_StateId_DistrictId",
                table: "Address",
                columns: new[] { "StateId", "DistrictId" });

            migrationBuilder.CreateIndex(
                name: "IX_Address_StateId_DistrictId_CommuneId_CommuneType",
                table: "Address",
                columns: new[] { "StateId", "DistrictId", "CommuneId", "CommuneType" });

            migrationBuilder.CreateIndex(
                name: "IX_Commune_StateId_DistrictId",
                table: "Commune",
                columns: new[] { "StateId", "DistrictId" });

            migrationBuilder.CreateIndex(
                name: "IX_District_StateId",
                table: "District",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_ProjectId",
                table: "Contract",
                column: "ProjectId");

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
                name: "IX_Project_ParentProjectId",
                table: "Project",
                column: "ParentProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Survey_InvestmentId",
                table: "Survey",
                column: "InvestmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvestmentOwner");

            migrationBuilder.DropTable(
                name: "Survey");

            migrationBuilder.DropTable(
                name: "Owner");

            migrationBuilder.DropTable(
                name: "Investment");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Contract");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "Commune");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "District");

            migrationBuilder.DropTable(
                name: "State");
        }
    }
}
