using Microsoft.EntityFrameworkCore;
using Schwartz.Siemens.Core.DomainServices;
using Schwartz.Siemens.Core.DomainServices.Repositories;
using Schwartz.Siemens.Core.Entities.Rigs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Schwartz.Siemens.Infrastructure.Data.Repositories
{
    public class RigRepository : IRigRepository
    {
        public RigRepository(MaritimeContext context, IWebSpider spider)
        {
            Context = context;
            Spider = spider;
        }

        private MaritimeContext Context { get; }
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
        /// <param name="item"></param>
        /// <returns></returns>
        public Rig Delete(Rig item)
        {
            var rig = Context.Rigs.Remove(item).Entity;
            Context.SaveChanges();
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
                .Include(r => r.Locations)
                .FirstOrDefault(r => r.Imo == id);
        }

        /// <summary>
        /// Retrieves the full list of Rig entities from the storage
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Rig> ReadAll()
        {
            return Context.Rigs.Include(r => r.Locations);
        }

        /// <summary>
        /// Update the information about an existing Rig entity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Rig Update(Rig item)
        {
            try
            {
                Context.Attach(item).State = EntityState.Modified;

                Context.SaveChanges();
                return item;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Location UpdateLocation(int imo)
        {
            return Spider.GetLatestLocation(imo);
        }

        public IEnumerable<Location> UpdateLocations(IEnumerable<int> imos)
        {
            return Spider.GetMultipleLocations(imos);
        }
    }
}