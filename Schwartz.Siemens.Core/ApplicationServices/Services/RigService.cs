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

            //return item;
            return RigRepository.Create(item);
        }

        public Rig Update(int id, Rig item)
        {
            return RigRepository.Update(id, item);
        }

        public Rig Delete(int id)
        {
            return RigRepository.Delete(id);
        }

        public List<Location> UpdatePositions(List<int> ids)
        {
            return RigRepository.UpdatePositions(ids).ToList();
        }
    }
}