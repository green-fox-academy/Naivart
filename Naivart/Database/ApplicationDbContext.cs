using Microsoft.EntityFrameworkCore;
using Naivart.Models.Entities;

namespace Naivart.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<AttackerTroops> AttackerTroops { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<BuildingType> BuildingTypes { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<Kingdom> Kingdoms { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<TroopsLost> TroopsLost { get; set; }
        public DbSet<TroopType> TroopTypes { get; set; }
        public DbSet<Troop> Troops { get; set; }

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

            modelBuilder.Entity<Resource>()
                .HasOne<Kingdom>(r => r.Kingdom)
                .WithMany(k => k.Resources)
                .HasForeignKey(r => r.KingdomId)
                .IsRequired(false);

            modelBuilder.Entity<Kingdom>()
                .HasMany<Troop>(k => k.Troops)
                .WithOne(l => l.Kingdom)
                .HasForeignKey(a => a.KingdomId)
                .IsRequired(true);

            modelBuilder.Entity<Kingdom>()
                .HasMany<Building>(k => k.Buildings)
                .WithOne(b => b.Kingdom)
                .HasForeignKey(b => b.KingdomId)
                .IsRequired(true);

            modelBuilder.Entity<Troop>()
                .HasOne<TroopType>(t => t.TroopType)
                .WithMany(t => t.Troops)
                .HasForeignKey(t => t.TroopTypeId)
                .IsRequired(true);
            
            modelBuilder.Entity<AttackerTroops>()
                .HasOne<Battle>(t => t.Battle)
                .WithMany(t => t.AttackingTroops)
                .HasForeignKey(t => t.BattleId)
                .IsRequired(true);
            
            modelBuilder.Entity<TroopsLost>()
                .HasOne<Battle>(t => t.Battle)
                .WithMany(t => t.DeadTroops)
                .HasForeignKey(t => t.BattleId)
                .IsRequired(true);

            modelBuilder.Entity<Building>()
                .HasOne<BuildingType>(b => b.BuildingType)
                .WithMany(bt => bt.Buildings)
                .HasForeignKey(b => b.BuildingTypeId)
                .IsRequired(true);
        }
    }
}
