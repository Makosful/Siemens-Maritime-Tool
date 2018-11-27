using Schwartz.Siemens.Core.DomainServices;
using Schwartz.Siemens.Core.Entities.Rigs;
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