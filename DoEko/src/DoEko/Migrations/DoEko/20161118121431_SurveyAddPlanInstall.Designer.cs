using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using DoEko.Models.DoEko;

namespace DoEko.Migrations.DoEko
{
    [DbContext(typeof(DoEkoContext))]
    [Migration("20161118121431_SurveyAddPlanInstall")]
    partial class SurveyAddPlanInstall
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DoEko.Models.DoEko.Addresses.Address", b =>
                {
                    b.Property<int>("AddressId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApartmentNo")
                        .HasAnnotation("MaxLength", 5);

                    b.Property<string>("BuildingNo")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 5);

                    b.Property<string>("City")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<int>("CommuneId");

                    b.Property<int>("CommuneType");

                    b.Property<int>("CountryId");

                    b.Property<int>("DistrictId");

                    b.Property<string>("PostOfficeLocation")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 6);

                    b.Property<int>("StateId");

                    b.Property<string>("Street")
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("AddressId");

                    b.HasIndex("CountryId");

                    b.HasIndex("StateId");

                    b.HasIndex("StateId", "DistrictId");

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
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("StateId", "DistrictId", "CommuneId", "Type");

                    b.HasIndex("StateId", "DistrictId");

                    b.ToTable("Commune");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Addresses.Country", b =>
                {
                    b.Property<int>("CountryId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 2);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

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
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("StateId", "DistrictId");

                    b.HasIndex("StateId");

                    b.ToTable("District");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Addresses.State", b =>
                {
                    b.Property<int>("StateId");

                    b.Property<string>("CapitalCity")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 5);

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.HasKey("StateId");

                    b.HasAlternateKey("Key");

                    b.ToTable("State");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.BusinessPartner", b =>
                {
                    b.Property<Guid>("BusinessPartnerId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AddressId");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("PhoneNumber")
                        .IsRequired();

                    b.Property<string>("TaxId");

                    b.HasKey("BusinessPartnerId");

                    b.HasIndex("AddressId");

                    b.ToTable("BusinessPartners");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BusinessPartner");
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
                        .HasAnnotation("MaxLength", 30);

                    b.Property<string>("Name2")
                        .HasAnnotation("MaxLength", 30);

                    b.Property<string>("PhoneNumber");

                    b.Property<string>("RegonId");

                    b.Property<string>("TaxId")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 13);

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
                        .HasAnnotation("MaxLength", 20);

                    b.Property<int>("ProjectId");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

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

                    b.Property<int>("ContractId");

                    b.Property<int>("InspectionStatus");

                    b.Property<Guid?>("InspectorId");

                    b.Property<bool>("InternetAvailable");

                    b.Property<string>("LandRegisterNo");

                    b.Property<string>("PlotAreaNumber");

                    b.Property<string>("PlotNumber");

                    b.Property<long>("PriorityIndex");

                    b.Property<int>("Status");

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

                    b.HasIndex("InvestmentId");

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

                    b.Property<decimal>("Price");

                    b.HasKey("StateId", "DistrictId", "CommuneId", "CommuneType", "ValidFrom", "ValidTo", "SurveyType");

                    b.HasIndex("StateId", "DistrictId", "CommuneId", "CommuneType");

                    b.ToTable("PriceLists");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Project", b =>
                {
                    b.Property<int>("ProjectId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CompanyId");

                    b.Property<string>("Description")
                        .HasAnnotation("MaxLength", 200);

                    b.Property<DateTime>("EndDate");

                    b.Property<int?>("ParentProjectId");

                    b.Property<DateTime?>("RealEnd");

                    b.Property<DateTime?>("RealStart");

                    b.Property<string>("ShortDescription")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<DateTime>("StartDate");

                    b.Property<int>("Status");

                    b.Property<int>("UEFundsLevel");

                    b.HasKey("ProjectId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("ParentProjectId");

                    b.ToTable("Project");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.RoofPlane", b =>
                {
                    b.Property<Guid>("RoofPlaneId")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("BuildingHeight");

                    b.Property<double>("EdgeLength");

                    b.Property<double>("Length");

                    b.Property<double>("OkapHeight");

                    b.Property<double>("RidgeWeight");

                    b.Property<double>("RoofLength");

                    b.Property<int>("RoofMaterial");

                    b.Property<double>("SlopeAngle");

                    b.Property<double>("SurfaceArea");

                    b.Property<double>("SurfaceAzimuth");

                    b.Property<Guid>("SurveyId");

                    b.Property<double>("Width");

                    b.HasKey("RoofPlaneId");

                    b.HasIndex("SurveyId");

                    b.ToTable("SurveyDetRoofPlane");
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

                    b.Property<Guid>("InvestmentId");

                    b.Property<bool>("IsPaid");

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

                    b.HasIndex("SurveyId")
                        .IsUnique();

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

                    b.HasIndex("SurveyId")
                        .IsUnique();

                    b.ToTable("SurveyDetBathroom");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetBoilerRoom", b =>
                {
                    b.Property<Guid>("SurveyId");

                    b.Property<bool>("AirVentilationExists");

                    b.Property<double>("DoorHeight");

                    b.Property<bool>("GroundedPowerSupply");

                    b.Property<double>("Height");

                    b.Property<bool>("HighVoltagePowerSupply");

                    b.Property<bool>("IsDryAndWarm");

                    b.Property<double>("Length");

                    b.Property<bool>("RoomExists");

                    b.Property<double>("Volume");

                    b.Property<double>("Width");

                    b.HasKey("SurveyId");

                    b.HasIndex("SurveyId")
                        .IsUnique();

                    b.ToTable("SurveyDetBoilerRoom");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetBuilding", b =>
                {
                    b.Property<Guid>("SurveyId");

                    b.Property<int>("BusinessActivity");

                    b.Property<short>("CompletionYear");

                    b.Property<double>("HeatedArea");

                    b.Property<double>("InsulationThickness");

                    b.Property<int>("InsulationType");

                    b.Property<short>("NumberOfOccupants");

                    b.Property<int>("Stage");

                    b.Property<int>("TechnologyType");

                    b.Property<double>("TotalArea");

                    b.Property<int>("Type");

                    b.Property<double>("UsableArea");

                    b.Property<double>("Volume");

                    b.Property<int>("WallMaterial");

                    b.Property<double>("WallThickness");

                    b.HasKey("SurveyId");

                    b.HasIndex("SurveyId")
                        .IsUnique();

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

                    b.Property<int>("CentralHeatingFuel");

                    b.Property<int>("CentralHeatingType");

                    b.Property<string>("CentralHeatingTypeOther");

                    b.Property<bool>("ComplexAgreement");

                    b.Property<decimal>("ElectricityAvgMonthlyCost");

                    b.Property<double>("ElectricityPower");

                    b.Property<bool>("HWCirculationInstalled");

                    b.Property<bool>("HWInstalled");

                    b.Property<bool>("HWPressureReductorExists");

                    b.Property<double>("HWSourcePower");

                    b.Property<int>("HotWaterFuel");

                    b.Property<int>("HotWaterType");

                    b.Property<bool>("PVIsGround");

                    b.Property<double>("PVPowerLevel");

                    b.Property<int>("PhaseCount");

                    b.Property<double>("PowerAvgYearlyConsumption");

                    b.Property<int>("PowerCompanyName");

                    b.Property<int>("PowerConsMeterLocation");

                    b.Property<int>("PowerSupplyType");

                    b.Property<double>("TankCoilSize");

                    b.Property<bool>("TankExists");

                    b.Property<double>("TankVolume");

                    b.HasKey("SurveyId");

                    b.HasIndex("SurveyId")
                        .IsUnique();

                    b.ToTable("SurveyDetEnergyAudit");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetGround", b =>
                {
                    b.Property<Guid>("SurveyId");

                    b.Property<double>("Area");

                    b.Property<bool>("FormerMilitary");

                    b.Property<bool>("OtherInstallation");

                    b.Property<string>("OtherInstallationTypw");

                    b.Property<bool>("Rocks");

                    b.Property<int>("SlopeTerrain");

                    b.Property<bool>("WetLand");

                    b.HasKey("SurveyId");

                    b.HasIndex("SurveyId")
                        .IsUnique();

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

                    b.HasIndex("SurveyId")
                        .IsUnique();

                    b.ToTable("SurveyDetPlannedInstall");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.SurveyDetRoof", b =>
                {
                    b.Property<Guid>("SurveyId");

                    b.Property<bool>("Chimney");

                    b.Property<bool>("InstallationUnderPlane");

                    b.Property<bool>("LightingProtection");

                    b.Property<bool>("SkyLights");

                    b.Property<int>("Type");

                    b.Property<bool>("Windows");

                    b.HasKey("SurveyId");

                    b.HasIndex("SurveyId")
                        .IsUnique();

                    b.ToTable("SurveyDetRoof");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.test", b =>
                {
                    b.Property<Guid>("PaymentId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ChangedAt");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<bool>("checkme");

                    b.Property<string>("dfg");

                    b.HasKey("PaymentId");

                    b.ToTable("tests");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.test1", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ChangedAt");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<bool>("checkme");

                    b.Property<string>("dfg");

                    b.HasKey("PaymentId");

                    b.ToTable("tests1");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.BusinessPartnerEntity", b =>
                {
                    b.HasBaseType("DoEko.Models.DoEko.BusinessPartner");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 30);

                    b.Property<string>("Name2")
                        .HasAnnotation("MaxLength", 30);

                    b.ToTable("BusinessPartnerEntity");

                    b.HasDiscriminator().HasValue("BusinessPartnerEntity");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.BusinessPartnerPerson", b =>
                {
                    b.HasBaseType("DoEko.Models.DoEko.BusinessPartner");

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 30);

                    b.Property<string>("IdNumber");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 30);

                    b.Property<string>("Pesel")
                        .IsRequired();

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
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity("DoEko.Models.DoEko.Survey.RoofPlane", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Survey.SurveyDetRoof", "Roof")
                        .WithMany("Planes")
                        .HasForeignKey("SurveyId")
                        .OnDelete(DeleteBehavior.Cascade);
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
                    b.HasOne("DoEko.Models.DoEko.Survey.Survey", "Surevy")
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
                        .WithOne("Roof")
                        .HasForeignKey("DoEko.Models.DoEko.Survey.SurveyDetRoof", "SurveyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
