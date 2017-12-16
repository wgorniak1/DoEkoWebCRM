using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using DoEko.Models.DoEko;
using DoEko.Models.DoEko.Addresses;
using DoEko.Models.DoEko.ClusterImport;
using DoEko.Models.DoEko.Survey;
using DoEko.Models.Payroll;

namespace DoEko.Migrations.DoEko
{
    [DbContext(typeof(DoEkoContext))]
    [Migration("20171216010116_Neo2")]
    partial class Neo2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DoEko.Models.DoEko.Addresses.Address", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApartmentNo")
                        .HasMaxLength(5);

                    b.Property<string>("BuildingNo")
                        .IsRequired()
                        .HasMaxLength(5);

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("CommuneId");

                    b.Property<int>("CommuneType");

                    b.Property<int>("CountryId");

                    b.Property<int>("DistrictId");

                    b.Property<string>("PostOfficeLocation")
                        .HasMaxLength(50);

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasMaxLength(6);

                    b.Property<int>("StateId");

                    b.Property<string>("Street")
                        .HasMaxLength(50);

                    b.HasKey("AddressId");

                    b.HasIndex("CountryId");

                    b.HasIndex("StateId", "DistrictId", "CommuneId", "CommuneType");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Addresses.Commune", b =>
                {
                    b.Property<int>("StateId");

                    b.Property<int>("DistrictId");

                    b.Property<int>("CommuneId");

                    b.Property<int>("Type");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("StateId", "DistrictId", "CommuneId", "Type");

                    b.ToTable("Commune");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Addresses.Country", b =>
                {
                    b.Property<int>("CountryId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(2);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("CountryId");

                    b.HasAlternateKey("Key");

                    b.ToTable("Country");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Addresses.District", b =>
                {
                    b.Property<int>("StateId");

                    b.Property<int>("DistrictId");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("StateId", "DistrictId");

                    b.ToTable("District");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Addresses.State", b =>
                {
                    b.Property<int>("StateId");

                    b.Property<string>("CapitalCity")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(5);

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("StateId");

                    b.HasAlternateKey("Key");

                    b.ToTable("State");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.BusinessPartner", b =>
                {
                    b.Property<Guid>("BusinessPartnerId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AddressId");

                    b.Property<bool>("DataProcessingConfirmation");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email");

                    b.Property<string>("PartnerName1");

                    b.Property<string>("PartnerName2");

                    b.Property<string>("PhoneNumber")
                        .IsRequired();

                    b.Property<string>("TaxId");

                    b.Property<int>("Type");

                    b.HasKey("BusinessPartnerId");

                    b.HasIndex("AddressId");

                    b.ToTable("BusinessPartners");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BusinessPartner");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.ClusterDetails", b =>
                {
                    b.Property<int>("ContractId");

                    b.Property<int>("CommuneId");

                    b.Property<int>("CommuneType");

                    b.Property<int>("DistrictId");

                    b.Property<int>("StateId");

                    b.HasKey("ContractId");

                    b.HasIndex("StateId", "DistrictId", "CommuneId", "CommuneType");

                    b.ToTable("ClusterDetails");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.ClusterImport.ClusterInvestment", b =>
                {
                    b.Property<Guid>("ClustInvestmentId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AddressId");

                    b.Property<int>("CompanySize");

                    b.Property<int>("ContractId");

                    b.Property<string>("Description")
                        .HasMaxLength(1000);

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<double>("EnYearlyConsumption");

                    b.Property<int>("MemberType");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("Name2")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<bool>("NewInstallation");

                    b.Property<string>("Phone")
                        .IsRequired();

                    b.Property<double>("PvPower");

                    b.Property<double>("PvYearlyProduction");

                    b.Property<string>("TaxId");

                    b.Property<int>("Type");

                    b.HasKey("ClustInvestmentId");

                    b.HasIndex("AddressId");

                    b.HasIndex("ContractId");

                    b.ToTable("ClusterInvestments");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Company", b =>
                {
                    b.Property<int>("CompanyId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AddressId");

                    b.Property<string>("Email");

                    b.Property<string>("KRSId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("Name2")
                        .HasMaxLength(30);

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("RegonId");

                    b.Property<string>("TaxId")
                        .IsRequired()
                        .HasMaxLength(13);

                    b.HasKey("CompanyId");

                    b.HasIndex("AddressId");

                    b.ToTable("Company");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Contract", b =>
                {
                    b.Property<int>("ContractId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CompanyId");

                    b.Property<DateTime>("ContractDate");

                    b.Property<DateTime?>("FullfilmentDate");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<int>("ProjectId");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("Status");

                    b.Property<int>("Type");

                    b.HasKey("ContractId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Contract");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.ControlParameter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.File", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ChangedAt");

                    b.Property<Guid>("ChangedBy");

                    b.Property<int?>("ContractId");

                    b.Property<string>("Name");

                    b.Property<Guid?>("ParentId");

                    b.Property<string>("ParentType");

                    b.Property<int?>("ProjectId");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("File");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Investment", b =>
                {
                    b.Property<Guid>("InvestmentId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AddressId");

                    b.Property<int>("BusinessActivity");

                    b.Property<bool>("Calculate");

                    b.Property<int>("CentralHeatingFuel");

                    b.Property<int>("CentralHeatingType");

                    b.Property<string>("CentralHeatingTypeOther");

                    b.Property<short>("CompletionYear");

                    b.Property<int>("ContractId");

                    b.Property<string>("GeoPortal");

                    b.Property<double>("HeatedArea");

                    b.Property<int>("HotWaterFuel");

                    b.Property<int>("HotWaterType");

                    b.Property<int>("InspectionStatus");

                    b.Property<Guid?>("InspectorId");

                    b.Property<bool>("InternetAvailable");

                    b.Property<string>("LandRegisterNo");

                    b.Property<short>("NumberOfOccupants");

                    b.Property<string>("PlotAreaNumber");

                    b.Property<string>("PlotNumber");

                    b.Property<long>("PriorityIndex");

                    b.Property<int>("Stage");

                    b.Property<int>("Status");

                    b.Property<double>("TotalArea");

                    b.Property<int>("Type");

                    b.Property<double>("UsableArea");

                    b.HasKey("InvestmentId");

                    b.HasIndex("AddressId");

                    b.HasIndex("ContractId");

                    b.ToTable("Investment");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.InvestmentOwner", b =>
                {
                    b.Property<Guid>("InvestmentId");

                    b.Property<Guid>("OwnerId");

                    b.Property<int>("OwnershipType");

                    b.Property<bool>("Sponsor");

                    b.HasKey("InvestmentId", "OwnerId");

                    b.HasIndex("OwnerId");

                    b.ToTable("InvestmentOwner");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Payment", b =>
                {
                    b.Property<Guid>("PaymentId")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("Amount")
                        .HasColumnType("money");

                    b.Property<DateTime>("ChangedAt");

                    b.Property<Guid>("ChangedBy");

                    b.Property<int>("ContractId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<Guid>("CreatedBy");

                    b.Property<Guid?>("InvestmentId");

                    b.Property<bool>("NotNeeded");

                    b.Property<DateTime>("PaymentDate");

                    b.Property<DateTime>("PostingDate");

                    b.Property<bool>("RseFotovoltaic");

                    b.Property<bool>("RseHeatPump");

                    b.Property<bool>("RseSolar");

                    b.Property<string>("SourceRow");

                    b.HasKey("PaymentId");

                    b.HasIndex("InvestmentId");

                    b.ToTable("Payment");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.PriceList", b =>
                {
                    b.Property<int>("StateId");

                    b.Property<int>("DistrictId");

                    b.Property<int>("CommuneId");

                    b.Property<int>("CommuneType");

                    b.Property<DateTime>("ValidFrom");

                    b.Property<DateTime>("ValidTo");

                    b.Property<int>("SurveyType");

                    b.Property<int>("RSEType");

                    b.Property<decimal>("Price");

                    b.HasKey("StateId", "DistrictId", "CommuneId", "CommuneType", "ValidFrom", "ValidTo", "SurveyType", "RSEType");

                    b.ToTable("PriceLists");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Project", b =>
                {
                    b.Property<int>("ProjectId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ChangedAt");

                    b.Property<Guid>("ChangedBy");

                    b.Property<int>("ClimateZone");

                    b.Property<int>("CompanyId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<Guid>("CreatedBy");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<DateTime>("EndDate");

                    b.Property<bool>("GrossNetFundsType");

                    b.Property<int?>("ParentProjectId");

                    b.Property<DateTime?>("RealEnd");

                    b.Property<DateTime?>("RealStart");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<DateTime>("StartDate");

                    b.Property<int>("Status");

                    b.Property<int>("UEFundsLevel");

                    b.HasKey("ProjectId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("ParentProjectId");

                    b.ToTable("Project");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.Survey", b =>
                {
                    b.Property<Guid>("SurveyId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CancelComments")
                        .IsRequired();

                    b.Property<int?>("CancelType");

                    b.Property<DateTime>("ChangedAt");

                    b.Property<Guid>("ChangedBy");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<Guid>("CreatedBy");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<DateTime>("FirstEditAt");

                    b.Property<Guid>("FirstEditBy");

                    b.Property<string>("FreeCommments");

                    b.Property<DateTime?>("InspectionDateTime")
                        .IsRequired();

                    b.Property<Guid>("InvestmentId");

                    b.Property<bool>("IsPaid");

                    b.Property<string>("RejectComments");

                    b.Property<int>("Status");

                    b.Property<int>("Type");

                    b.HasKey("SurveyId");

                    b.HasIndex("InvestmentId");

                    b.ToTable("Survey");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Survey");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetAirCond", b =>
                {
                    b.Property<Guid>("SurveyId");

                    b.Property<bool>("Exists");

                    b.Property<bool>("MechVentilationExists");

                    b.Property<int>("Type");

                    b.Property<bool>("isPlanned");

                    b.HasKey("SurveyId");

                    b.ToTable("SurveyDetAirCond");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetBathroom", b =>
                {
                    b.Property<Guid>("SurveyId");

                    b.Property<bool>("BathExsists");

                    b.Property<double>("BathVolume");

                    b.Property<short>("NumberOfBathrooms");

                    b.Property<bool>("ShowerExists");

                    b.HasKey("SurveyId");

                    b.ToTable("SurveyDetBathroom");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetBoilerRoom", b =>
                {
                    b.Property<Guid>("SurveyId");

                    b.Property<bool>("AirVentilationExists");

                    b.Property<double>("DoorHeight");

                    b.Property<bool>("HWCirculationInstalled");

                    b.Property<bool>("HWInstalled");

                    b.Property<bool>("HWPressureReductorExists");

                    b.Property<double>("Height");

                    b.Property<bool>("HighVoltagePowerSupply");

                    b.Property<bool>("IsDryAndWarm");

                    b.Property<double>("Length");

                    b.Property<bool>("RoomExists");

                    b.Property<bool>("ThreePowerSuppliesExists");

                    b.Property<double>("Volume");

                    b.Property<double>("Width");

                    b.HasKey("SurveyId");

                    b.ToTable("SurveyDetBoilerRoom");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetBuilding", b =>
                {
                    b.Property<Guid>("SurveyId");

                    b.Property<double>("InsulationThickness");

                    b.Property<int>("InsulationType");

                    b.Property<string>("InsulationTypeOther");

                    b.Property<int>("TechnologyType");

                    b.Property<double>("Volume");

                    b.Property<int>("WallMaterial");

                    b.Property<string>("WallMaterialOther");

                    b.Property<double>("WallThickness");

                    b.HasKey("SurveyId");

                    b.ToTable("SurveyDetBuilding");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetEnergyAudit", b =>
                {
                    b.Property<Guid>("SurveyId");

                    b.Property<string>("AdditionalHeatParams");

                    b.Property<bool>("AdditionalHeatSource");

                    b.Property<double>("AverageYearlyFuelConsumption");

                    b.Property<decimal>("AverageYearlyHeatingCosts");

                    b.Property<double>("BoilerMaxTemp");

                    b.Property<double>("BoilerNominalPower");

                    b.Property<bool>("BoilerPlannedReplacement");

                    b.Property<short>("BoilerProductionYear");

                    b.Property<bool>("CHFRadiantFloorInstalled");

                    b.Property<bool>("CHIsHPOnlySource");

                    b.Property<int>("CHRadiantFloorAreaPerc");

                    b.Property<int>("CHRadiatorType");

                    b.Property<bool>("CHRadiatorsInstalled");

                    b.Property<bool>("ComplexAgreement");

                    b.Property<bool>("ENAdditionalConsMeter");

                    b.Property<bool>("ENIsGround");

                    b.Property<double>("ENPowerLevel");

                    b.Property<decimal>("ElectricityAvgMonthlyCost");

                    b.Property<double>("ElectricityPower");

                    b.Property<double>("HWSourcePower");

                    b.Property<int>("PhaseCount");

                    b.Property<double>("PowerAvgYearlyConsumption");

                    b.Property<int>("PowerCompanyName");

                    b.Property<int>("PowerConsMeterLocation");

                    b.Property<int>("PowerSupplyType");

                    b.Property<double>("TankCoilSize");

                    b.Property<bool>("TankExists");

                    b.Property<double>("TankVolume");

                    b.HasKey("SurveyId");

                    b.ToTable("SurveyDetEnergyAudit");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetGround", b =>
                {
                    b.Property<Guid>("SurveyId");

                    b.Property<double>("Area");

                    b.Property<bool>("FormerMilitary");

                    b.Property<bool>("OtherInstallation");

                    b.Property<string>("OtherInstallationType");

                    b.Property<bool>("Rocks");

                    b.Property<int>("SlopeTerrain");

                    b.Property<bool>("WetLand");

                    b.HasKey("SurveyId");

                    b.ToTable("SurveyDetGround");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetPlannedInstall", b =>
                {
                    b.Property<Guid>("SurveyId");

                    b.Property<int>("Configuration");

                    b.Property<int>("Localization");

                    b.Property<bool>("OnWallPlacementAvailable");

                    b.Property<int>("Purpose");

                    b.HasKey("SurveyId");

                    b.ToTable("SurveyDetPlannedInstall");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetRoof", b =>
                {
                    b.Property<Guid>("RoofPlaneId")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("BuildingHeight");

                    b.Property<bool>("Chimney");

                    b.Property<double>("EdgeLength");

                    b.Property<bool>("InstallationUnderPlane");

                    b.Property<double>("Length");

                    b.Property<bool>("LightingProtection");

                    b.Property<double>("OkapHeight");

                    b.Property<double>("RidgeWeight");

                    b.Property<double>("RoofLength");

                    b.Property<int>("RoofMaterial");

                    b.Property<bool>("SkyLights");

                    b.Property<double>("SlopeAngle");

                    b.Property<double>("SurfaceArea");

                    b.Property<double>("SurfaceAzimuth");

                    b.Property<Guid>("SurveyId");

                    b.Property<int>("Type");

                    b.Property<double>("Width");

                    b.Property<bool>("Windows");

                    b.HasKey("RoofPlaneId");

                    b.HasIndex("SurveyId");

                    b.ToTable("SurveyDetRoof");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetWall", b =>
                {
                    b.Property<Guid>("SurveyId");

                    b.Property<double>("Azimuth");

                    b.Property<double>("Height");

                    b.Property<double>("UsableArea");

                    b.Property<double>("Width");

                    b.HasKey("SurveyId");

                    b.ToTable("SurveyDetWall");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyResultCalculations", b =>
                {
                    b.Property<Guid>("SurveyId");

                    b.Property<double>("BenzoPirenPercent");

                    b.Property<double>("BenzoPirenValue");

                    b.Property<double>("CHMaxRequiredEn");

                    b.Property<double>("CHRSEWorkingTime");

                    b.Property<double>("CHRSEYearlyProduction");

                    b.Property<double>("CHRequiredEn");

                    b.Property<double>("CHRequiredEnFactor");

                    b.Property<double>("CO2DustEquivPercent");

                    b.Property<double>("CO2DustEquivValue");

                    b.Property<double>("CO2EquivValue");

                    b.Property<double>("CO2Percent");

                    b.Property<double>("CO2Value");

                    b.Property<double>("FinalPVConfig");

                    b.Property<double>("FinalRSEPower");

                    b.Property<string>("FinalSOLConfig");

                    b.Property<double>("HWRSEWorkingTime");

                    b.Property<double>("HWRSEYearlyProduction");

                    b.Property<double>("HWRequiredEnYearly");

                    b.Property<double>("HeatLossFactor");

                    b.Property<double>("PM10Percent");

                    b.Property<double>("PM10Value");

                    b.Property<double>("PM25Percent");

                    b.Property<double>("PM25Value");

                    b.Property<double>("RSEEfficiency");

                    b.Property<double>("RSEEnYearlyConsumption");

                    b.Property<double>("RSEGrossPrice");

                    b.Property<double>("RSENetPrice");

                    b.Property<double>("RSETax");

                    b.Property<double>("RSEWorkingTime");

                    b.Property<double>("RSEYearlyProduction");

                    b.HasKey("SurveyId");

                    b.ToTable("SurveyResultCalculations");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyStatusHistory", b =>
                {
                    b.Property<Guid>("SurveyId");

                    b.Property<DateTime>("Start");

                    b.Property<DateTime>("End");

                    b.Property<int>("Status");

                    b.Property<Guid>("UserId");

                    b.HasKey("SurveyId", "Start");

                    b.ToTable("SurveyStatusHistory");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Test", b =>
                {
                    b.Property<Guid>("PaymentId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ChangedAt");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<bool>("checkme");

                    b.Property<string>("dfg");

                    b.HasKey("PaymentId");

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Test1", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ChangedAt");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<bool>("checkme");

                    b.Property<string>("dfg");

                    b.HasKey("PaymentId");

                    b.ToTable("Tests1");
                });

            modelBuilder.Entity("DoEko.Models.Payroll.EmployeeBasicPay", b =>
                {
                    b.Property<Guid>("EmployeeId");

                    b.Property<DateTime>("Start");

                    b.Property<DateTime>("End");

                    b.Property<string>("Code");

                    b.Property<double>("Amount");

                    b.Property<int?>("ContractId");

                    b.Property<string>("Currency");

                    b.Property<double>("Number");

                    b.Property<int?>("ProjectId");

                    b.Property<double>("Rate");

                    b.Property<string>("ShortDescription");

                    b.Property<int>("Unit");

                    b.HasKey("EmployeeId", "Start", "End", "Code");

                    b.ToTable("EmployeesBasicPay");
                });

            modelBuilder.Entity("DoEko.Models.Payroll.EmployeeUser", b =>
                {
                    b.Property<Guid>("EmployeeId");

                    b.Property<DateTime>("Start");

                    b.Property<DateTime>("End");

                    b.Property<Guid>("UserId");

                    b.HasKey("EmployeeId", "Start", "End", "UserId");

                    b.ToTable("EmployeesUsers");
                });

            modelBuilder.Entity("DoEko.Models.Payroll.PayrollCluster", b =>
                {
                    b.Property<Guid>("PayrollClusterId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ChangedAt");

                    b.Property<Guid>("ChangedBy");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<Guid>("CreatedBy");

                    b.Property<Guid>("EmployeeId");

                    b.Property<DateTime>("PeriodFor");

                    b.Property<DateTime>("PeriodIn");

                    b.Property<short>("SequenceNo");

                    b.HasKey("PayrollClusterId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("PayrollCluster");
                });

            modelBuilder.Entity("DoEko.Models.Payroll.PayrollComment", b =>
                {
                    b.Property<Guid>("PayrollCommentId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("PayrollResultId");

                    b.Property<string>("Text");

                    b.HasKey("PayrollCommentId");

                    b.HasIndex("PayrollResultId");

                    b.ToTable("PayrollComment");
                });

            modelBuilder.Entity("DoEko.Models.Payroll.PayrollResult", b =>
                {
                    b.Property<Guid>("PayrollResultId")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Amount");

                    b.Property<string>("Code");

                    b.Property<string>("Currency");

                    b.Property<double>("Number");

                    b.Property<Guid>("PayrollClusterId");

                    b.Property<double>("Rate");

                    b.Property<string>("ShortDescription");

                    b.Property<Guid>("SurveyId");

                    b.Property<int>("Unit");

                    b.HasKey("PayrollResultId");

                    b.HasIndex("PayrollClusterId");

                    b.ToTable("PayrollResult");
                });

            modelBuilder.Entity("DoEko.Models.Payroll.WageTypeDefinition", b =>
                {
                    b.Property<int>("WageTypeDefinitionId")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Amount");

                    b.Property<bool>("AmountMandatory");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(4);

                    b.Property<string>("Currency");

                    b.Property<double>("Number");

                    b.Property<double>("Rate");

                    b.Property<bool>("RateMandatory");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("Unit");

                    b.HasKey("WageTypeDefinitionId");

                    b.ToTable("WageTypeCatalog");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.BusinessPartnerEntity", b =>
                {
                    b.HasBaseType("DoEko.Models.DoEko.BusinessPartner");

                    b.Property<int>("CompanySize");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("Name2")
                        .HasMaxLength(30);

                    b.ToTable("BusinessPartnerEntity");

                    b.HasDiscriminator().HasValue("BusinessPartnerEntity");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.BusinessPartnerPerson", b =>
                {
                    b.HasBaseType("DoEko.Models.DoEko.BusinessPartner");

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("IdNumber");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(30);

                    b.Property<string>("Pesel");

                    b.ToTable("BusinessPartnerPerson");

                    b.HasDiscriminator().HasValue("BusinessPartnerPerson");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyCentralHeating", b =>
                {
                    b.HasBaseType("DoEko.Models.DoEko.Survey.Survey");

                    b.Property<int>("RSEType");

                    b.ToTable("SurveyCentralHeating");

                    b.HasDiscriminator().HasValue("SurveyCentralHeating");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyEnergy", b =>
                {
                    b.HasBaseType("DoEko.Models.DoEko.Survey.Survey");

                    b.Property<int>("RSEType");

                    b.ToTable("SurveyEnergy");

                    b.HasDiscriminator().HasValue("SurveyEnergy");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyHotWater", b =>
                {
                    b.HasBaseType("DoEko.Models.DoEko.Survey.Survey");

                    b.Property<int>("RSEType");

                    b.ToTable("SurveyHotWater");

                    b.HasDiscriminator().HasValue("SurveyHotWater");
                });

            modelBuilder.Entity("DoEko.Models.Payroll.Employee", b =>
                {
                    b.HasBaseType("DoEko.Models.DoEko.BusinessPartnerPerson");

                    b.Property<Guid>("EmployeeId");

                    b.ToTable("Employee");

                    b.HasDiscriminator().HasValue("Employee");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Addresses.Address", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Addresses.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId");

                    b.HasOne("DoEko.Models.DoEko.Addresses.State", "State")
                        .WithMany()
                        .HasForeignKey("StateId");

                    b.HasOne("DoEko.Models.DoEko.Addresses.District", "District")
                        .WithMany()
                        .HasForeignKey("StateId", "DistrictId");

                    b.HasOne("DoEko.Models.DoEko.Addresses.Commune", "Commune")
                        .WithMany()
                        .HasForeignKey("StateId", "DistrictId", "CommuneId", "CommuneType");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Addresses.Commune", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Addresses.District", "District")
                        .WithMany("Communes")
                        .HasForeignKey("StateId", "DistrictId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Addresses.District", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Addresses.State", "State")
                        .WithMany("Districts")
                        .HasForeignKey("StateId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.BusinessPartner", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Addresses.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.ClusterDetails", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Contract", "Contract")
                        .WithOne("ClusterDetails")
                        .HasForeignKey("DoEko.Models.DoEko.ClusterDetails", "ContractId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DoEko.Models.DoEko.Addresses.State", "State")
                        .WithMany()
                        .HasForeignKey("StateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DoEko.Models.DoEko.Addresses.District", "District")
                        .WithMany()
                        .HasForeignKey("StateId", "DistrictId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DoEko.Models.DoEko.Addresses.Commune", "Commune")
                        .WithMany()
                        .HasForeignKey("StateId", "DistrictId", "CommuneId", "CommuneType")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.ClusterImport.ClusterInvestment", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Addresses.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DoEko.Models.DoEko.Contract", "Contract")
                        .WithMany()
                        .HasForeignKey("ContractId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Company", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Addresses.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Contract", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DoEko.Models.DoEko.Project", "Project")
                        .WithMany("Contracts")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Investment", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Addresses.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId");

                    b.HasOne("DoEko.Models.DoEko.Contract", "Contract")
                        .WithMany("Investments")
                        .HasForeignKey("ContractId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.InvestmentOwner", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Investment", "Investment")
                        .WithMany("InvestmentOwners")
                        .HasForeignKey("InvestmentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DoEko.Models.DoEko.BusinessPartner", "Owner")
                        .WithMany("InvestmentOwners")
                        .HasForeignKey("OwnerId");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Payment", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Investment")
                        .WithMany("Payments")
                        .HasForeignKey("InvestmentId");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.PriceList", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Addresses.Commune", "Commune")
                        .WithMany()
                        .HasForeignKey("StateId", "DistrictId", "CommuneId", "CommuneType");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Project", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DoEko.Models.DoEko.Project", "ParentProject")
                        .WithMany("ChildProjects")
                        .HasForeignKey("ParentProjectId");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.Survey", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Investment", "Investment")
                        .WithMany("Surveys")
                        .HasForeignKey("InvestmentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetAirCond", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Survey.Survey", "Survey")
                        .WithOne("AirCondition")
                        .HasForeignKey("DoEko.Models.DoEko.Survey.SurveyDetAirCond", "SurveyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetBathroom", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Survey.Survey", "Survey")
                        .WithOne("BathRoom")
                        .HasForeignKey("DoEko.Models.DoEko.Survey.SurveyDetBathroom", "SurveyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetBoilerRoom", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Survey.Survey", "Survey")
                        .WithOne("BoilerRoom")
                        .HasForeignKey("DoEko.Models.DoEko.Survey.SurveyDetBoilerRoom", "SurveyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetBuilding", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Survey.Survey", "Survey")
                        .WithOne("Building")
                        .HasForeignKey("DoEko.Models.DoEko.Survey.SurveyDetBuilding", "SurveyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetEnergyAudit", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Survey.Survey", "Survey")
                        .WithOne("Audit")
                        .HasForeignKey("DoEko.Models.DoEko.Survey.SurveyDetEnergyAudit", "SurveyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetGround", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Survey.Survey", "Survey")
                        .WithOne("Ground")
                        .HasForeignKey("DoEko.Models.DoEko.Survey.SurveyDetGround", "SurveyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetPlannedInstall", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Survey.Survey", "Survey")
                        .WithOne("PlannedInstall")
                        .HasForeignKey("DoEko.Models.DoEko.Survey.SurveyDetPlannedInstall", "SurveyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetRoof", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Survey.Survey", "Survey")
                        .WithMany("RoofPlanes")
                        .HasForeignKey("SurveyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetWall", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Survey.Survey", "Survey")
                        .WithOne("Wall")
                        .HasForeignKey("DoEko.Models.DoEko.Survey.SurveyDetWall", "SurveyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyResultCalculations", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Survey.Survey", "Survey")
                        .WithOne("ResultCalculation")
                        .HasForeignKey("DoEko.Models.DoEko.Survey.SurveyResultCalculations", "SurveyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyStatusHistory", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Survey.Survey", "Survey")
                        .WithMany()
                        .HasForeignKey("SurveyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.Payroll.EmployeeBasicPay", b =>
                {
                    b.HasOne("DoEko.Models.Payroll.Employee", "Employee")
                        .WithMany("BasicPay")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.Payroll.EmployeeUser", b =>
                {
                    b.HasOne("DoEko.Models.Payroll.Employee", "Employee")
                        .WithMany("Users")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.Payroll.PayrollCluster", b =>
                {
                    b.HasOne("DoEko.Models.Payroll.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.Payroll.PayrollComment", b =>
                {
                    b.HasOne("DoEko.Models.Payroll.PayrollResult", "PayrollResult")
                        .WithMany("Comments")
                        .HasForeignKey("PayrollResultId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.Payroll.PayrollResult", b =>
                {
                    b.HasOne("DoEko.Models.Payroll.PayrollCluster", "PayrollCluster")
                        .WithMany("Results")
                        .HasForeignKey("PayrollClusterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
