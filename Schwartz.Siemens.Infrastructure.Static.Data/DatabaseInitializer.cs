using Schwartz.Siemens.Infrastructure.Data;

namespace Schwartz.Siemens.Infrastructure.Static.Data
{
    public static class DatabaseInitializer
    {
        public static void SeedDb(MaritimeContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}