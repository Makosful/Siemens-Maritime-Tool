using Microsoft.EntityFrameworkCore;
using Schwartz.Siemens.Core.Entities.Rigs;
using Schwartz.Siemens.Core.Entities.UserBase;

namespace Schwartz.Siemens.Infrastructure.Data
{
    public class MaritimeContext : DbContext
    {
        public MaritimeContext(DbContextOptions<MaritimeContext> options) : base(options)
        {
        }

        public DbSet<Rig> Rigs { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rig>().HasKey(r => r.Imo);

            modelBuilder.Entity<Location>()
                .HasOne(loc => loc.Rig)
                .WithMany(rig => rig.Locations)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}