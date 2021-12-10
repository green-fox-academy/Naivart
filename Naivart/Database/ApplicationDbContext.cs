using Microsoft.EntityFrameworkCore;
using Naivart.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Naivart.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Kingdom> Kingdoms { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Troop> Troops { get; set; }
        public DbSet<Building> Buildings { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kingdom>()
                .HasOne<Location>(k => k.Location)
                .WithOne(l => l.Kingdom)
                .HasForeignKey<Kingdom>(k => k.LocationId)
                .IsRequired(false);

            modelBuilder.Entity<Player>()
                .HasOne<Kingdom>(p => p.Kingdom)
                .WithOne(k => k.Player)
                .HasForeignKey<Player>(p => p.KingdomId)
                .IsRequired(true);

            modelBuilder.Entity<Kingdom>()
                .HasMany<Building>(k=> k.Buildings)
                .WithOne(b => b.Kingdom)
                .HasForeignKey(b => b.KingdomId)
                .IsRequired(true);

            modelBuilder.Entity<Kingdom>()
                .HasMany<Troop>(k => k.Troops)
                .WithOne(l => l.Kingdom)
                .HasForeignKey(a => a.KingdomId)
                .IsRequired(true);

            modelBuilder.Entity<Resource>()
                .HasOne<Kingdom>(r => r.Kingdom)
                .WithMany(k => k.Resources)
                .HasForeignKey(r => r.KingdomId)
                .IsRequired(false);
        }
    }
}
