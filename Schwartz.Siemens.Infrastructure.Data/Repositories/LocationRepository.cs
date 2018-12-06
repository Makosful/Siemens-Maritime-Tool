using Microsoft.EntityFrameworkCore;
using Schwartz.Siemens.Core.DomainServices.Repositories;
using Schwartz.Siemens.Core.Entities.Rigs;
using System.Collections.Generic;
using System.Linq;

namespace Schwartz.Siemens.Infrastructure.Data.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        public LocationRepository(MaritimeContext context)
        {
            Context = context;
        }

        private MaritimeContext Context { get; }

        public Location Read(int id)
        {
            return Context.Locations
                .Include(loc => loc.Rig)
                .FirstOrDefault(loc => loc.Id == id);
        }

        public IEnumerable<Location> ReadAll()
        {
            return Context.Locations
                .Include(loc => loc.Rig);
        }

        public Location Create(Location item)
        {
            var location = Context.Locations.Add(item).Entity;
            Context.SaveChanges();
            return location;
        }

        public Location Update(int id, Location item)
        {
            item.Id = id;
            var location = Context.Locations.Update(item).Entity;
            Context.SaveChanges();
            return location;
        }

        public Location Delete(Location item)
        {
            Context.Locations.Remove(item);
            Context.SaveChanges();
            return item;
        }

        public List<Location> CreateRange(IEnumerable<Location> locations)
        {
            var retur = new List<Location>();
            foreach (var location in locations)
            {
                retur.Add(Context.Locations.Add(location).Entity);
            }

            Context.SaveChanges();

            return retur;
        }
    }
}