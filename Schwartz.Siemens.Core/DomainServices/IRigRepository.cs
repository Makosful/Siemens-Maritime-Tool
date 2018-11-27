using Schwartz.Siemens.Core.Entities.Rigs;
using System.Collections.Generic;

namespace Schwartz.Siemens.Core.DomainServices
{
    public interface IRigRepository : ICrudRepository<Rig>
    {
        IEnumerable<Location> UpdatePositions(IEnumerable<int> validIds);
    }
}