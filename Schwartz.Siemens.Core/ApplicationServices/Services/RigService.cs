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
        public RigService(IRigRepository rigRepository)
        {
            RigRepository = rigRepository;
        }

        private IRigRepository RigRepository { get; }

        public Rig Create(Rig item)
        {
            if (item.Imo == 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(item),
                    "Rigs can't have an Id of 0. The ID should match the vessel registration number");
            }

            if (item.Imo < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(item),
                    "A Rig's ID cant be below 1. Make sure the id matches the vessel's registration number");
            }

            if (Read(item.Imo) != null)
            {
                throw new ArgumentException(
                    "A rig with the given ID has already been registered in the database",
                    nameof(item));
            }

            return RigRepository.Create(item);
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
                    UpdatePositionAsync(rig.Imo);
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
                    UpdatePositionAsync(rig.Imo);
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

        public Location UpdateLocation(int imo)
        {
            var rig = RigRepository.Read(imo);
            if (rig == null) return null;

            return RigRepository.UpdateLocation(imo);
        }

        public List<Location> UpdatePositions(List<int> imos)
        {
            return RigRepository.UpdateLocations(imos).ToList();
        }

        /// <summary>
        /// This method is meant to be Fire-and-Forget.
        /// It will not return any value unlike it's counterpart.
        /// </summary>
        /// <param name="imo"></param>
        public void UpdatePositionAsync(int imo)
        {
            Task.Run(() => UpdateLocation(imo));
        }

        public void UpdateLocationsAsync(List<int> imos)
        {
            Task.Run(() => UpdatePositions(imos));
        }
    }
}