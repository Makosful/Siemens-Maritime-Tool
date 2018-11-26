using Microsoft.EntityFrameworkCore;
using Schwartz.Siemens.Core.Entities;

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
            modelBuilder.Entity<Rig>()
                .HasMany(r => r.Location)
                .WithOne(l => l.Rig);
        }
    }
}