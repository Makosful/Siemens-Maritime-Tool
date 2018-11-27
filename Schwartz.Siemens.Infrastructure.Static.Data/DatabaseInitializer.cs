using Schwartz.Siemens.Core.Entities.Rigs;
using Schwartz.Siemens.Infrastructure.Data;

namespace Schwartz.Siemens.Infrastructure.Static.Data
{
    public static class DatabaseInitializer
    {
        public static void SeedDb(MaritimeContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            MockRigs(context);

            context.SaveChanges();
        }

        private static void MockRigs(MaritimeContext context)
        {
            context.Rigs.Add(new Rig(158372) { Name = "MAERSK INSPIRER" });
        }
    }
}