using DoEko.Models.DoEko.Addresses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using DoEko.Models.DoEko;
using System.Threading;
using System.Threading.Tasks;

namespace DoEko.Models.DoEko
{
    public class DoEkoContext : DbContext
    {
        public DoEkoContext(DbContextOptions<DoEkoContext> options)
            : base(options)
        { }

        public DbSet<test> tests { get; set; }
        public DbSet<test1> tests1 { get; set; }
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
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<SurveyHotWater> SurveysHW { get; set; }
        public DbSet<SurveyCentralHeating> SurveysCH { get; set; }
        public DbSet<SurveyEnergy> SurveysEN { get; set; }

        public DbSet<PriceList> PriceLists { get; set; }

        public DbSet<Address> Addresses { get; set; }

        //Address Catalog
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Commune> Communes { get; set; }
        public DbSet<File> File { get; set; }

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

            // Projekt Parent - Child relation
            modelBuilder.Entity<Project>()
                .HasOne(p => p.ParentProject)
                .WithMany(p => p.ChildProjects)
                .HasForeignKey(p => p.ParentProjectId);

            // Adres - kaskadowe usuwanie zabronione
            modelBuilder.Entity<Address>().HasOne(a => a.Country).WithMany().HasForeignKey(c => c.CountryId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            modelBuilder.Entity<Address>().HasOne(a => a.State).WithMany().HasForeignKey(a => a.StateId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            modelBuilder.Entity<Address>().HasOne(a => a.District).WithMany().HasForeignKey(d => new { d.StateId, d.DistrictId }).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            modelBuilder.Entity<Address>().HasOne(a => a.Commune).WithMany().HasForeignKey(c => new { c.StateId, c.DistrictId, c.CommuneId, c.CommuneType }).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);

            // Inwestycja - kaskadowe usuwanie zabrionione
            //modelBuilder.Entity<Investment>().HasOne(i => i.Address).WithMany().HasForeignKey(a => a.AddressId).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);
            //Cennnik, klucz kompozytowy: gmina, okres, typ ankiety
            modelBuilder.Entity<PriceList>().HasKey(pl => new { pl.StateId, pl.DistrictId, pl.CommuneId, pl.CommuneType, pl.ValidFrom, pl.ValidTo, pl.SurveyType });
            //Cennik, klucz obcy: gmina
            modelBuilder.Entity<PriceList>().HasOne(pl => pl.Commune).WithMany().HasForeignKey(c => new { c.StateId, c.DistrictId, c.CommuneId, c.CommuneType }).OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);

            //modelBuilder.Entity<Payment>().Property(p => p.CreatedAt).ValueGeneratedOnAdd();
            //modelBuilder.Entity<Payment>().Property(p => p.ChangedAt).ValueGeneratedOnAddOrUpdate();

        }

        public override int SaveChanges()
        {
            this.ChangeTracker.DetectChanges();
            var newEntries = this.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added);

            foreach (var entry in newEntries)
            {
                try
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
                catch (Exception)
                {
                }

                try
                {
                    entry.Property("ChangedAt").CurrentValue = DateTime.UtcNow;
                }
                catch (Exception)
                {
                }

                try
                {
                    entry.Property("CreatedBy").CurrentValue = this.CurrentUserId;
                }
                catch (Exception)
                {
                }

                try
                {
                    entry.Property("ChangedBy").CurrentValue = this.CurrentUserId;
                }
                catch (Exception)
                {
                }
            }

            var modifiedEntries = this.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified);
            foreach (var entry in modifiedEntries)
            {
                try
                {
                    entry.Property("ChangedAt").CurrentValue = DateTime.UtcNow;
                }
                catch (Exception)
                {
                }

                try
                {
                    entry.Property("ChangedBy").CurrentValue = this.CurrentUserId;
                }
                catch (Exception)
                {
                }
            }
            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ChangeTracker.DetectChanges();
            var newEntries = this.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added);

            foreach (var entry in newEntries)
            {
                try
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
                catch (Exception)
                {
                }

                try
                {
                    entry.Property("ChangedAt").CurrentValue = DateTime.UtcNow;
                }
                catch (Exception)
                {
                }

                try
                {
                    entry.Property("CreatedBy").CurrentValue = this.CurrentUserId;
                }
                catch (Exception)
                {
                }

                try
                {
                    entry.Property("ChangedBy").CurrentValue = this.CurrentUserId;
                }
                catch (Exception)
                {
                }
            }

            var modifiedEntries = this.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified);
            foreach (var entry in modifiedEntries)
            {
                try
                {
                    entry.Property("ChangedAt").CurrentValue = DateTime.UtcNow;
                }
                catch (Exception)
                {
                }

                try
                {
                    entry.Property("ChangedBy").CurrentValue = this.CurrentUserId;
                }
                catch (Exception)
                {
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public string CurrentUserId { get; set; }

        

    }
}