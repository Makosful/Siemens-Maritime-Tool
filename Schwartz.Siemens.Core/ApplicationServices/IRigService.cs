using Schwartz.Siemens.Core.Entities.Rigs;
using System.Collections.Generic;

namespace Schwartz.Siemens.Core.ApplicationServices
{
    public interface IRigService : ICrudService<Rig>
    {
        Location UpdateLocation(int imo);

        List<Location> UpdateLocations(List<int> imoList);

        void UpdateLocationAsync(int imo);

        void UpdateLocationsAsync(List<int> imos);
    }
}