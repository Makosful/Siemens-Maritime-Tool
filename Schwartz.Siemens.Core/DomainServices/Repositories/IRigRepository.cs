using Schwartz.Siemens.Core.Entities.Rigs;
using System.Collections.Generic;

namespace Schwartz.Siemens.Core.DomainServices.Repositories
{
    public interface IRigRepository : ICrudRepository<Rig>
    {
        Location UpdateLocation(int imo);

        IEnumerable<Location> UpdateLocations(IEnumerable<int> imos);
    }
}