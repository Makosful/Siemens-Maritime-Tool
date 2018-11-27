using Microsoft.EntityFrameworkCore;
using Schwartz.Siemens.Core.Entities.Rigs;

namespace Schwartz.Siemens.Infrastructure.Data
{
    public class MaritimeContext : DbContext
    {
        public MaritimeContext(DbContextOptions<MaritimeContext> options) : base(options)
        {
        }

        public DbSet<Rig> Rigs { get; set; }
        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rig>().HasKey(r => r.Id);
            modelBuilder.Entity<Location>().HasKey(loc => loc.Id);

            modelBuilder.Entity<Location>()
                .HasOne(loc => loc.Rig)
                .WithMany(rig => rig.Location)
                .IsRequired()
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}