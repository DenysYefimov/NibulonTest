using Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Db
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Aup> Aups { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Region> Regions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Aup>()
                .ToTable("AUP")
                .HasKey(a => a.Id);

            modelBuilder.Entity<Aup>()
                .Property(a => a.Id)
                .HasColumnName("ID")
                .HasMaxLength(10);

            modelBuilder.Entity<Aup>()
                .Property(a => a.Postcode)
                .HasColumnName("INDEX_A")
                .HasMaxLength(6);

            modelBuilder.Entity<Aup>()
                .Property(a => a.CityCode)
                .HasColumnName("CITY")
                .HasMaxLength(20);

            modelBuilder.Entity<Aup>()
                .Property(a => a.CityName)
                .HasColumnName("NCITY")
                .HasMaxLength(200);

            modelBuilder.Entity<Aup>()
                .Property(a => a.RegionCode)
                .HasColumnName("OBL")
                .HasMaxLength(4);

            modelBuilder.Entity<Aup>()
                .Property(a => a.RegionName)
                .HasColumnName("NOBL")
                .HasMaxLength(200);

            modelBuilder.Entity<Aup>()
                .Property(a => a.DistrictCode)
                .HasColumnName("RAJ")
                .HasMaxLength(5);

            modelBuilder.Entity<Aup>()
                .Property(a => a.DistrictName)
                .HasColumnName("NRAJ")
                .HasMaxLength(200);

            modelBuilder.Entity<Aup>()
                .HasOne(a => a.City)
                .WithMany(c => c.Aups)
                .HasForeignKey(a => a.CityCode);

            modelBuilder.Entity<Aup>()
                .HasOne(a => a.District)
                .WithMany(d => d.Aups)
                .HasForeignKey(a => a.DistrictCode);

            modelBuilder.Entity<Aup>()
                .HasOne(a => a.Region)
                .WithMany(r => r.Aups)
                .HasForeignKey(a => a.RegionCode);

            modelBuilder.Entity<City>()
                .ToTable("CITY")
                .HasKey(c => c.Code);

            modelBuilder.Entity<City>()
                .Property(c => c.Name)
                .HasColumnName("CITY")
                .HasMaxLength(50);

            modelBuilder.Entity<City>()
                .Property(c => c.DistrictCode)
                .HasColumnName("KRAJ")
                .HasMaxLength(5);

            modelBuilder.Entity<City>()
                .Property(c => c.RegionCode)
                .HasColumnName("OBL")
                .HasMaxLength(4);

            modelBuilder.Entity<City>()
                .Property(c => c.Code)
                .HasColumnName("CITY_KOD")
                .HasMaxLength(20);

            modelBuilder.Entity<City>()
                .HasOne(c => c.District)
                .WithMany(d => d.Cities)
                .HasForeignKey(c => c.DistrictCode);

            modelBuilder.Entity<City>()
                .HasOne(c => c.Region)
                .WithMany(r => r.Cities)
                .HasForeignKey(c => c.RegionCode);

            modelBuilder.Entity<District>()
                .ToTable("KRAJ")
                .HasKey(d => d.Code);

            modelBuilder.Entity<District>()
                .Property(d => d.Code)
                .HasColumnName("KRAJ")
                .HasMaxLength(5);

            modelBuilder.Entity<District>()
                .Property(d => d.Name)
                .HasColumnName("NRAJ")
                .HasMaxLength(200);

            modelBuilder.Entity<Region>()
                .ToTable("OBL")
                .HasKey(r => r.Code);

            modelBuilder.Entity<Region>()
                .Property(r => r.Code)
                .HasColumnType("smallint")
                .HasColumnName("OBL")
                .HasMaxLength(4);

            modelBuilder.Entity<Region>()
                .Property(r => r.Name)
                .HasColumnName("NOBL")
                .HasMaxLength(200);
        }

        override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
