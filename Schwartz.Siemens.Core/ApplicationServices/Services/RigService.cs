using Schwartz.Siemens.Core.DomainServices;
using Schwartz.Siemens.Core.DomainServices.Repositories;
using Schwartz.Siemens.Core.Entities.Rigs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Schwartz.Siemens.Core.ApplicationServices.Services
{
    public class RigService : IRigService
    {
        public RigService(IRigRepository rigRepository,
            ILocationRepository locationRepository,
            IWebSpider spider)
        {
            RigRepository = rigRepository;
            Spider = spider;
            LocationRepository = locationRepository;
        }

        private ILocationRepository LocationRepository { get; }
        private IRigRepository RigRepository { get; }
        private IWebSpider Spider { get; }

        public Rig Create(Rig item)
        {
            if (item.Imo == 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(item.Imo),
                    "Rigs can't have an IMO of 0. This value should match the vessel's International Maritime Organization number");
            }

            if (item.Imo < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(item.Imo),
                    "A Rig's IMO can't be below 1. this value should match the vessel's International Maritime Organization number");
            }

            if (Read(item.Imo) != null)
            {
                throw new ArgumentException(
                    "A Rig with the same IMO has already been found in the database.",
                    nameof(item.Imo));
            }

            var rig = Spider.GetRig(item.Imo);

            return RigRepository.Create(rig);
        }

        public Rig Delete(int id)
        {
            if (id < 1) throw new ArgumentOutOfRangeException(nameof(id), "IMO at 0 and below are invalid and can't be created, therefor, not deleted");

            var rig = Read(id);
            if (rig == null) throw new KeyNotFoundException($"No Rig was found with the given IMO: {id}. The rig wasn't deleted");

            return RigRepository.Delete(rig);
        }

        public Rig Read(int id)
        {
            var rig = RigRepository.Read(id);
            if (rig == null) return null;

            rig.Locations = rig.Locations.OrderByDescending(location => location.Date).ToList();

            if (rig.Locations.Count > 0)
            {
                var latestDate = rig.Locations[0].Date;
                if (DateTime.Now.Subtract(latestDate).TotalHours > 12)
                {
                    UpdateLocationAsync(rig.Imo);
                    rig.Outdated = true;
                }
                else
                {
                    rig.Outdated = false;
                }
            }

            return rig;
        }

        public List<Rig> ReadAll()
        {
            var rigs = RigRepository.ReadAll().ToList();

            foreach (var rig in rigs)
            {
                var locations = rig.Locations;
                var locationsOrdered = locations.OrderByDescending(l => l.Date).ToList();

                var latestDate = locationsOrdered[0].Date;
                if (DateTime.Now.Subtract(latestDate).Hours > 12)
                {
                    // Fire and forget method
                    UpdateLocationAsync(rig.Imo);
                    rig.Outdated = true;
                }
                else
                {
                    rig.Outdated = false;
                }

                rig.Locations = locationsOrdered.ToList();
            }

            return rigs;
        }

        public Rig Update(int id, Rig item)
        {
            if (id < 1) throw new ArgumentOutOfRangeException(nameof(id), "The given IMO cannot be 0 or below. Make sure the rig exists and you have the right IMO");

            if (Read(id) == null) throw new KeyNotFoundException("No Rig with the given IMO was found. Make sure the Rig exists and you have the correct IMO");

            if (item == null) throw new ArgumentNullException(nameof(item), "The Rig entity passed in as an argument is null.");

            return RigRepository.Update(id, item);
        }

        #region Location Update

        // This section should probably be moved to LocationService instead

        /// <summary>
        /// Fetches the latest Location for the Rig with the given IMO
        /// </summary>
        /// <param name="imo"></param>
        /// <returns></returns>
        public Location UpdateLocation(int imo)
        {
            var rig = RigRepository.Read(imo);
            if (rig == null) return null;

            var location = Spider.GetLatestLocation(imo);
            return LocationRepository.Create(location);
        }

        /// <summary>
        /// This method is meant to be Fire-and-Forget.
        /// It will not return any value unlike it's counterpart.
        /// </summary>
        /// <param name="imo"></param>
        public void UpdateLocationAsync(int imo)
        {
            Task.Run(() => UpdateLocation(imo));
        }

        /// <summary>
        /// Fetches the latest Location for the Rig entities with the given IMOs
        /// </summary>
        /// <param name="imoList"></param>
        /// <returns></returns>
        public List<Location> UpdateLocations(List<int> imoList)
        {
            var rigs = RigRepository.ReadAll().ToList();
            var valid = new List<Rig>();
            foreach (var rig in rigs)
                if (imoList.Contains(rig.Imo))
                    valid.Add(rig);

            var locations = Spider.GetMultipleLocations(valid.Select(rig => rig.Imo));

            return LocationRepository.CreateRange(locations);
        }

        /// <summary>
        /// This method is meant as a Fire-and-Forget.
        /// It will not return anything, unlike it's counterpart
        /// </summary>
        /// <param name="imos"></param>
        public void UpdateLocationsAsync(List<int> imos)
        {
            Task.Run(() => UpdateLocations(imos));
        }

        #endregion Location Update
    }
}