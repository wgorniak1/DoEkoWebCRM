using DoEko.Models.DoEko.Addresses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using DoEko.Models.DoEko;
using System.Threading;
using System.Threading.Tasks;
using DoEko.Models.DoEko.Survey;
using Microsoft.EntityFrameworkCore.Metadata;
using DoEko.Models.Payroll;
using DoEko.Models.DoEko.ClusterImport;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DoEko.Models.DoEko
{
    public class DoEkoContext : DbContext
    {
        public DoEkoContext(DbContextOptions<DoEkoContext> options)
            : base(options)
        { }

        public Guid CurrentUserId { get; set; }

        public DbSet<Test> Tests { get; set; }
        public DbSet<Test1> Tests1 { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ControlParameter> Settings { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Investment> Investments { get; set; }
        public DbSet<BusinessPartner> BusinessPartners { get; set; }
        public DbSet<InvestmentOwner> InvestmentOwners { get; set; }
        public DbSet<BusinessPartnerPerson> BPPersons { get; set; }
        public DbSet<BusinessPartnerEntity> BPEntities { get; set; }
        public DbSet<Survey.Survey> Surveys { get; set; }
        public DbSet<SurveyHotWater> SurveysHW { get; set; }
        public DbSet<SurveyCentralHeating> SurveysCH { get; set; }
        public DbSet<SurveyEnergy> SurveysEN { get; set; }

        public DbSet<SurveyStatusHistory> SurveyStatusHistory { get; set; }

        public DbSet<PriceList> PriceLists { get; set; }

        public DbSet<Address> Addresses { get; set; }

        //Address Catalog
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Commune> Communes { get; set; }
        public DbSet<File> File { get; set; }

        //PAYROLL
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeBasicPay> EmployeesBasicPay { get; set; }
        public DbSet<EmployeeUser> EmployeesUsers { get; set; }
        public DbSet<WageTypeDefinition> WageTypeCatalog { get; set; }
        public DbSet<PayrollCluster> PayrollCluster { get; set; }

        //CLUSTER INVESTMENTS
        public DbSet<ClusterInvestment> ClusterInvestments { get; set; }

        //RSE PRICE CALCULATION
        public DbSet<RSEPriceTaxRule> RSEPriceTaxRules { get; set; }
        public DbSet<RSEPriceRule> RSEPriceRules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            //country
            modelBuilder.Entity<Country>().HasAlternateKey(p => p.Key);
            //state
            modelBuilder.Entity<State>().HasKey(s => s.StateId);
            modelBuilder.Entity<State>().HasAlternateKey(s => s.Key);
            //district
            modelBuilder.Entity<District>().HasKey(d => new { d.StateId, d.DistrictId });
            //commune
            modelBuilder.Entity<Commune>().HasKey(c => new { c.StateId, c.DistrictId, c.CommuneId, c.Type });

            // Investment n..n Owner
            modelBuilder.Entity<InvestmentOwner>().HasKey(t => new { t.InvestmentId, t.OwnerId });

            modelBuilder.Entity<InvestmentOwner>().HasOne(io => io.Owner)
                            .WithMany(o => o.InvestmentOwners)
                            .HasForeignKey(io => io.OwnerId)
                            .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            modelBuilder.Entity<InvestmentOwner>().HasOne(io => io.Investment)
                            .WithMany(i => i.InvestmentOwners)
                            .HasForeignKey(io => io.InvestmentId)
                            .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);


            // Adres - kaskadowe usuwanie zabronione
            modelBuilder.Entity<Address>().HasOne(a => a.Country).WithMany().HasForeignKey(c => c.CountryId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            modelBuilder.Entity<Address>().HasOne(a => a.State).WithMany().HasForeignKey(a => a.StateId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            modelBuilder.Entity<Address>().HasOne(a => a.District).WithMany().HasForeignKey(d => new { d.StateId, d.DistrictId }).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            modelBuilder.Entity<Address>().HasOne(a => a.Commune).WithMany().HasForeignKey(c => new { c.StateId, c.DistrictId, c.CommuneId, c.CommuneType }).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            
            // Inwestycja - relacja z adresem, bez usuwania
            modelBuilder.Entity<Investment>().HasOne(i => i.Address).WithMany().HasForeignKey(a => a.AddressId).OnDelete(DeleteBehavior.Restrict);

            //Kaskadowe usuwanie wszystkiego po kolei:
            //1. Projekt: Relacja Parent-Child + usuwanie podprojektów
            modelBuilder.Entity<Project>()
                .HasOne(p => p.ParentProject)
                .WithMany(p => p.ChildProjects)
                .HasForeignKey(p => p.ParentProjectId)
                .OnDelete(DeleteBehavior.Restrict);
            //2. Contract: usuwanie Projektu wymusza usuwanie contractów
            //modelBuilder.Entity<Contract>().HasOne(c => c.Project).WithMany().HasForeignKey(p => p.ProjectId).OnDelete(DeleteBehavior.Cascade);
            //3. Inwestycja: usuwanie umowy wymusza usuwanie inwestycji
            //modelBuilder.Entity<Investment>().HasOne(i => i.Contract).WithMany().HasForeignKey(c => c.ContractId).OnDelete(DeleteBehavior.Cascade);
            //4. Ankieta: usuwanie inwestycji wymusza usuwanie ankiet
            //modelBuilder.Entity<Survey.Survey>().HasOne(s => s.Investment).WithMany().HasForeignKey(i => i.InvestmentId).OnDelete(DeleteBehavior.Cascade);

            //Cennnik, klucz kompozytowy: gmina, okres, typ ankiety
            modelBuilder.Entity<PriceList>().HasKey(pl => new { pl.StateId, pl.DistrictId, pl.CommuneId, pl.CommuneType, pl.ValidFrom, pl.ValidTo, pl.SurveyType, pl.RSEType });
            //Cennik, klucz obcy: gmina
            modelBuilder.Entity<PriceList>().HasOne(pl => pl.Commune).WithMany().HasForeignKey(c => new { c.StateId, c.DistrictId, c.CommuneId, c.CommuneType }).OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Payment>().Property(p => p.CreatedAt).ValueGeneratedOnAdd();
            //modelBuilder.Entity<Payment>().Property(p => p.ChangedAt).ValueGeneratedOnAddOrUpdate();


            //
            modelBuilder.Entity<SurveyStatusHistory>().HasKey(d => new { d.SurveyId, d.Start});

            //PAYROLL
            modelBuilder.Entity<Payroll.EmployeeBasicPay>().HasKey(bp => new { bp.EmployeeId, bp.Start, bp.End, bp.Code });
            modelBuilder.Entity<Payroll.EmployeeUser>().HasKey(eu => new { eu.EmployeeId, eu.Start, eu.End, eu.UserId });

            //CLUSTER INVESTMENT

            //modelBuilder.Entity<ClusterInvestment>().HasOne(a => a.State).WithMany().HasForeignKey(a => a.StateId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            //modelBuilder.Entity<ClusterInvestment>().HasOne(a => a.District).WithMany().HasForeignKey(d => new { d.StateId, d.DistrictId }).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            //modelBuilder.Entity<ClusterInvestment>().HasOne(a => a.Commune).WithMany().HasForeignKey(c => new { c.StateId, c.DistrictId, c.CommuneId, c.CommuneType }).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);

            //RSE PRICE CALCULATION
            modelBuilder.Entity<RSEPriceRule>()
                .HasKey( s => new { s.ProjectId,
                                    s.SurveyType,
                                    s.RSEType,
                                    s.Unit,
                                    s.NumberMin,
                                    s.NumberMax });
            modelBuilder.Entity<RSEPriceTaxRule>().HasKey(s => new
            {
                s.ProjectId,
                s.SurveyType,
                s.RSEType,
                s.InstallationLocalization,
                s.BuildingPurpose,
                s.UsableAreaMin,
                s.UsableAreaMax
            });
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();

            var newEntries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added);

            UpdateProperty(newEntries, "ChangedAt", DateTime.UtcNow);
            UpdateProperty(newEntries, "ChangedBy", this.CurrentUserId);
            UpdateProperty(newEntries, "ChangedAt", DateTime.UtcNow);
            UpdateProperty(newEntries, "ChangedBy", this.CurrentUserId);

            UpdateAddress(newEntries.Where(e => e.Entity.GetType() == typeof(Address)));

            var modifiedEntries = this.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);

            UpdateProperty(modifiedEntries, "ChangedAt", DateTime.UtcNow);
            UpdateProperty(modifiedEntries, "ChangedBy", this.CurrentUserId);

            UpdateAddress(modifiedEntries.Where(e => e.Entity.GetType() == typeof(Address)));

            return base.SaveChanges();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException">If task cancellation is requested</exception>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) 
        {
            SaveChanges();

            return base.SaveChangesAsync(cancellationToken);
        }


        private void UpdateProperty( IEnumerable<EntityEntry> entities, string propertyName, object value)
        {

            foreach (var Property in entities.SelectMany(e=>e.Properties.Where(p=>p.Metadata.Name == propertyName)))
                Property.CurrentValue = value;

        }
        
        private void UpdateAddress( IEnumerable<EntityEntry> entities)
        {
            //var enumerator = entities.GetEnumerator();
            //while (enumerator.MoveNext() == true)
            //{

            //}
            for (var i = 0; i < entities.Count(); i++)
            {
                Address entry = (Address)(entities.ElementAt(i).Entity);

                this.Entry(entry).Reference(e => e.State).Load();
                this.Entry(entry).Reference(e => e.District).Load();
                this.Entry(entry).Reference(e => e.Commune).Load();

                entry.SearchTerm = Address.GetSearchTerm(entry);
            }
        }
    }
}