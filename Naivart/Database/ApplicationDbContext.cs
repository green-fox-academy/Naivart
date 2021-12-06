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
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Kingdom>()
                .HasOne<Location>(k => k.Location)
                .WithOne(l => l.Kingdom)
                .HasForeignKey<Kingdom>(k => k.LocationId)
                .IsRequired(true);
        }
    }
}
