using Schwartz.Siemens.Core.DomainServices.Repositories;
using Schwartz.Siemens.Core.Entities.Rigs;
using System.Collections.Generic;
using System.Linq;

namespace Schwartz.Siemens.Core.ApplicationServices.Services
{
    public class LocationService : ILocationService
    {
        public LocationService(ILocationRepository repository)
        {
            LocationRepository = repository;
        }

        private ILocationRepository LocationRepository { get; }

        /// <summary>
        /// Gets a single Location entity without it's Rig object attached
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Location Read(int id)
        {
            return LocationRepository.Read(id);
        }

        /// <summary>
        /// Gets a list of all locations without Rig objects attached
        /// </summary>
        /// <returns></returns>
        public List<Location> ReadAll()
        {
            return LocationRepository.ReadAll().ToList();
        }

        /// <summary>
        /// Creates a new Location entity in the storage
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Location Create(Location item)
        {
            return LocationRepository.Create(item);
        }

        /// <summary>
        /// Updates the information of a single Location entity
        /// </summary>
        /// <param name="id">Id of the Location entity to update</param>
        /// <param name="item">An entity containing the updated information. This ID is ignored</param>
        /// <returns></returns>
        public Location Update(int id, Location item)
        {
            return LocationRepository.Update(id, item);
        }

        /// <summary>
        /// Deletes a single Location entity from the storage
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Location Delete(int id)
        {
            var location = Read(id);
            return LocationRepository.Delete(location);
        }
    }
}