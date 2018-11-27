using Schwartz.Siemens.Core.Entities.Rigs;
using Schwartz.Siemens.Infrastructure.Data;
using System;
using System.Collections.Generic;

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
            var location1 = context.Locations.Add(new Location
            {
                Date = DateTime.Now,
                Latitude = 55.5224998,
                Longitude = 8.3934396,
            }).Entity;
            var location2 = context.Locations.Add(new Location
            {
                Date = DateTime.Now.AddHours(-12),
                Latitude = 55.5224998,
                Longitude = 8.4934396,
            }).Entity;
            var location3 = context.Locations.Add(new Location
            {
                Date = DateTime.Now.AddHours(-24),
                Latitude = 55.5224998,
                Longitude = 8.5934396,
            }).Entity;
            var location4 = context.Locations.Add(new Location
            {
                Date = DateTime.Now.AddHours(-36),
                Latitude = 55.5224998,
                Longitude = 8.6934396,
            }).Entity;
            context.Rigs.Add(new Rig(158372)
            {
                Name = "MAERSK INSPIRER",
                Location = new List<Location>
                {
                    location1,
                    location2,
                    location3,
                    location4,
                },
            });
        }
    }
}