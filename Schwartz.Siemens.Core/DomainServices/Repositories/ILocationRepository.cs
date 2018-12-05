using Schwartz.Siemens.Core.Entities.Rigs;
using System.Collections.Generic;

namespace Schwartz.Siemens.Core.DomainServices.Repositories
{
    public interface ILocationRepository : ICrudRepository<Location>
    {
        List<Location> CreateRange(IEnumerable<Location> locations);
    }
}