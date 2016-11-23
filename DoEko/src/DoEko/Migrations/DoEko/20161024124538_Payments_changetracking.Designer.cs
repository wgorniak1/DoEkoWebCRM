using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using DoEko.Models.DoEko;

namespace DoEko.Migrations.DoEko
{
    [DbContext(typeof(DoEkoContext))]
    [Migration("20161024124538_Payments_changetracking")]
    partial class Payments_changetracking
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

                    b.Property<string>("LandRegisterNo")
                        .HasAnnotation("MaxLength", 15);

                    b.Property<string>("PlotNumber")
                        .HasAnnotation("MaxLength", 19);

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

                    b.Property<DateTime>("ChangedAt")
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<Guid>("ChangedBy");

                    b.Property<int>("ContractId");

                    b.Property<DateTime>("CreatedAt")
                        .ValueGeneratedOnAdd();

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

            modelBuilder.Entity("DoEko.Models.DoEko.Survey", b =>
                {
                    b.Property<Guid>("SurveyId")
                        .ValueGeneratedOnAdd();

                    b.Property<short>("BuildingCompletionYear");

                    b.Property<double>("BuildingCurrentEnergyTotal");

                    b.Property<short>("BuildingNumberOfHosts");

                    b.Property<double>("BuildingOverallArea");

                    b.Property<int>("BuildingState");

                    b.Property<int>("BuildingType");

                    b.Property<int>("BuildingType2");

                    b.Property<double>("BuildingUsableArea");

                    b.Property<int>("BusinessActivity");

                    b.Property<string>("CancelComments");

                    b.Property<int?>("CancelType");

                    b.Property<int>("CentralHeatingFuel");

                    b.Property<int>("CentralHeatingType");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<int>("HotWaterFuel");

                    b.Property<int>("HotWaterType");

                    b.Property<int>("InstallationLocalization");

                    b.Property<bool>("InternetConnectionAvailable");

                    b.Property<Guid>("InvestmentId");

                    b.Property<bool>("IsPaid");

                    b.Property<int>("Status");

                    b.Property<int>("Type");

                    b.HasKey("SurveyId");

                    b.HasIndex("InvestmentId");

                    b.ToTable("Survey");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Survey");
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

            modelBuilder.Entity("DoEko.Models.DoEko.SurveyCentralHeating", b =>
                {
                    b.HasBaseType("DoEko.Models.DoEko.Survey");


                    b.ToTable("SurveyCentralHeating");

                    b.HasDiscriminator().HasValue("SurveyCentralHeating");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.SurveyEnergy", b =>
                {
                    b.HasBaseType("DoEko.Models.DoEko.Survey");


                    b.ToTable("SurveyEnergy");

                    b.HasDiscriminator().HasValue("SurveyEnergy");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.SurveyHotWater", b =>
                {
                    b.HasBaseType("DoEko.Models.DoEko.Survey");

                    b.Property<bool>("AirVentilationExists");

                    b.Property<double>("Azimuth");

                    b.Property<bool>("BoilerStationExists");

                    b.Property<double>("BoilerStationSizeX");

                    b.Property<double>("BoilerStationSizeY");

                    b.Property<double>("BoilerStationSizeZ");

                    b.Property<double>("BuildingSizeX");

                    b.Property<double>("BuildingSizeY");

                    b.Property<double>("BuildingSizeZ");

                    b.Property<bool>("ChimneysExists");

                    b.Property<bool>("CirculationExists");

                    b.Property<double>("Current");

                    b.Property<bool>("GroundedSocketsExists");

                    b.Property<bool>("InstalationExists");

                    b.Property<double>("InstallationSpace");

                    b.Property<bool>("LightingRodExists");

                    b.Property<bool>("PresureRegulator");

                    b.Property<double>("RoofEdgeWeight");

                    b.Property<double>("RoofHeight");

                    b.Property<double>("RoofInclinationAngle");

                    b.Property<bool>("RoofLightsExists");

                    b.Property<int>("RoofMaterial");

                    b.Property<double>("RoofRidgeWeight");

                    b.Property<double>("RoofWidth");

                    b.Property<bool>("RoofWindowsExists");

                    b.Property<int>("TargetHotWaterType");

                    b.Property<bool>("UnderRoofInstallationExists");

                    b.Property<bool>("isDoorSizeEnough");

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

            modelBuilder.Entity("DoEko.Models.DoEko.Survey", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Investment", "Investment")
                        .WithMany("Surveys")
                        .HasForeignKey("InvestmentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
