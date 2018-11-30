using Schwartz.Siemens.Core.DomainServices.Repositories;
using Schwartz.Siemens.Core.Entities.Rigs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Schwartz.Siemens.Core.ApplicationServices.Services
{
    public class RigService : IRigService
    {
        public RigService(IRigRepository rigRepository)
        {
            RigRepository = rigRepository;
        }

        private IRigRepository RigRepository { get; }

        public Rig Read(int id)
        {
            return RigRepository.Read(id);
        }

        public List<Rig> ReadAll()
        {
            return RigRepository.ReadAll().ToList();
        }

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

        public Rig Update(int id, Rig item)
        {
            if (id < 1) throw new ArgumentOutOfRangeException(nameof(id), "The given IMO cannot be 0 or below. Make sure the rig exists and you have the right IMO");

            if (Read(id) == null) throw new KeyNotFoundException("No Rig with the given IMO was found. Make sure the Rig exists and you have the correct IMO");

            if (item == null) throw new ArgumentNullException(nameof(item), "The Rig entity passed in as an argument is null.");

            return RigRepository.Update(id, item);
        }

        public Rig Delete(int id)
        {
            if (id < 1) throw new ArgumentOutOfRangeException(nameof(id), "IMO at 0 and below are invalid and can't be created, therefor, not deleted");

            var rig = Read(id);
            if (rig == null) throw new KeyNotFoundException($"No Rig was found with the given IMO: {id}. The rig wasn't deleted");

            return RigRepository.Delete(rig);
        }

        public List<Location> UpdatePositions(List<int> ids)
        {
            return RigRepository.UpdatePositions(ids).ToList();
        }
    }
}