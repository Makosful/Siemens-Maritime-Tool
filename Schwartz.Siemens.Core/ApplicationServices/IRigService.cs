using Schwartz.Siemens.Core.Entities.Rigs;
using System.Collections.Generic;

namespace Schwartz.Siemens.Core.ApplicationServices
{
    public interface IRigService : ICrudService<Rig>
    {
        List<Location> UpdatePositions(List<int> ids);
    }
}