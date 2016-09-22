using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using DoEko.Models.DoEko;

namespace DoEko.Migrations.DoEko
{
    [DbContext(typeof(DoEkoContext))]
    partial class DoEkoContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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
                        .HasAnnotation("MaxLength", 10);

                    b.Property<int>("StateId");

                    b.Property<string>("Street")
                        .IsRequired()
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

            modelBuilder.Entity("DoEko.Models.DoEko.Contract", b =>
                {
                    b.Property<int>("ContractId")
                        .ValueGeneratedOnAdd();

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

            modelBuilder.Entity("DoEko.Models.DoEko.Investment", b =>
                {
                    b.Property<int>("InvestmentId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AddressId")
                        .IsRequired();

                    b.Property<int?>("ContractId")
                        .IsRequired();

                    b.Property<int>("InspectionStatus");

                    b.Property<string>("LandRegisterNo")
                        .HasAnnotation("MaxLength", 11);

                    b.Property<string>("PlotNumber")
                        .HasAnnotation("MaxLength", 11);

                    b.Property<int>("Status");

                    b.HasKey("InvestmentId");

                    b.HasIndex("AddressId");

                    b.HasIndex("ContractId");

                    b.ToTable("Investment");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.InvestmentOwner", b =>
                {
                    b.Property<int>("InvestmentId");

                    b.Property<int>("OwnerId");

                    b.Property<bool>("Sponsor");

                    b.HasKey("InvestmentId", "OwnerId");

                    b.HasIndex("InvestmentId");

                    b.HasIndex("OwnerId");

                    b.ToTable("InvestmentOwner");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Owner", b =>
                {
                    b.Property<int>("OwnerId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AddressId");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("PhoneNumber")
                        .IsRequired();

                    b.Property<string>("TaxId");

                    b.HasKey("OwnerId");

                    b.HasIndex("AddressId");

                    b.ToTable("Owner");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Owner");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Project", b =>
                {
                    b.Property<int>("ProjectId")
                        .ValueGeneratedOnAdd();

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

                    b.HasIndex("ParentProjectId");

                    b.ToTable("Project");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey", b =>
                {
                    b.Property<int>("SurveyId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("InvestmentId");

                    b.Property<string>("version");

                    b.HasKey("SurveyId");

                    b.HasIndex("InvestmentId");

                    b.ToTable("Survey");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.OwnerInstitution", b =>
                {
                    b.HasBaseType("DoEko.Models.DoEko.Owner");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 30);

                    b.Property<string>("Name2")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 30);

                    b.ToTable("Owner");

                    b.HasDiscriminator().HasValue("OwnerInstitution");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.OwnerPerson", b =>
                {
                    b.HasBaseType("DoEko.Models.DoEko.Owner");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 30);

                    b.Property<string>("IdNumber");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 30);

                    b.Property<string>("Pesel");

                    b.ToTable("Owner");

                    b.HasDiscriminator().HasValue("OwnerPerson");
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

            modelBuilder.Entity("DoEko.Models.DoEko.Contract", b =>
                {
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
                        .HasForeignKey("InvestmentId");

                    b.HasOne("DoEko.Models.DoEko.Owner", "Owner")
                        .WithMany("InvestmentOwners")
                        .HasForeignKey("OwnerId");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Owner", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Addresses.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Project", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Project", "ParentProject")
                        .WithMany("ChildProjects")
                        .HasForeignKey("ParentProjectId");
                });

            modelBuilder.Entity("DoEko.Models.DoEko.Survey", b =>
                {
                    b.HasOne("DoEko.Models.DoEko.Investment", "Investment")
                        .WithMany("Surveys")
                        .HasForeignKey("InvestmentId");
                });
        }
    }
}
