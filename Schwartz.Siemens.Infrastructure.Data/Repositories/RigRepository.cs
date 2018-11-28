using Microsoft.EntityFrameworkCore;
using Schwartz.Siemens.Core.DomainServices;
using Schwartz.Siemens.Core.DomainServices.Repositories;
using Schwartz.Siemens.Core.Entities.Rigs;
using System.Collections.Generic;
using System.Linq;

namespace Schwartz.Siemens.Infrastructure.Data.Repositories
{
    public class RigRepository : IRigRepository
    {
        public RigRepository(MaritimeContext context, IWebSpider spider, ILocationRepository locationRepository)
        {
            Context = context;
            Spider = spider;
            LocationRepository = locationRepository;
        }

        private MaritimeContext Context { get; }
        private ILocationRepository LocationRepository { get; }
        private IWebSpider Spider { get; }

        /// <summary>
        /// Saves a new Rig entity to the storage.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Rig Create(Rig item)
        {
            var rig = Context.Rigs.Add(item).Entity;
            Context.SaveChanges();
            return rig;
        }

        /// <summary>
        /// Removes a Rig Entity from the storage
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Rig Delete(int id)
        {
            var rig = Read(id);
            Context.Rigs.Remove(rig);
            return rig;
        }

        /// <summary>
        /// Returns the first Rig entity found in the storage, which match the specified ID.
        /// If no Rig entity is found, returns null
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Rig Read(int id)
        {
            return Context.Rigs
                .Include(r => r.Location)
                .FirstOrDefault(r => r.Id == id);
        }

        /// <summary>
        /// Retrieves the full list of Rig entities from the storage
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Rig> ReadAll()
        {
            return Context.Rigs
                .Include(r => r.Location);
        }

        /// <summary>
        /// Update the information about an existing Rig entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public Rig Update(int id, Rig item)
        {
            item.Id = id;
            var rig = Context.Rigs.Update(item).Entity;
            Context.SaveChanges();
            return rig;
        }

        public IEnumerable<Location> UpdatePositions(IEnumerable<int> validIds)
        {
            var locations = Spider.GetMultipleLocations(validIds).ToList();
            var rigs = ReadAll().ToList();

            foreach (var location in locations)
            {
                rigs.FirstOrDefault(r => r.Id == location.Rig.Id)?.Location.Add(location);
            }

            Context.Rigs.UpdateRange(rigs);

            return locations;
        }
    }
}