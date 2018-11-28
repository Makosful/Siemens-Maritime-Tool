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

        public Location Read(int id)
        {
            return LocationRepository.Read(id);
        }

        public List<Location> ReadAll()
        {
            return LocationRepository.ReadAll().ToList();
        }

        public Location Create(Location item)
        {
            return LocationRepository.Create(item);
        }

        public Location Update(int id, Location item)
        {
            return LocationRepository.Update(id, item);
        }

        public Location Delete(int id)
        {
            return LocationRepository.Delete(id);
        }
    }
}