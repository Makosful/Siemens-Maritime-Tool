using Schwartz.Siemens.Core.Entities.Rigs;
using System.Collections.Generic;

namespace Schwartz.Siemens.Core.DomainServices.Repositories
{
    public interface ILocationRepository : ICrudRepository<Location>
    {
        void CreateRange(IEnumerable<Location> locations);
    }
}