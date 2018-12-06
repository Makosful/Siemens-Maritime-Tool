using Schwartz.Siemens.Core.Entities.Rigs;

namespace Schwartz.Siemens.Core.ApplicationServices
{
    public interface IRigService : ICrudService<Rig>
    {
        Location UpdateLocation(int imo);

        void UpdateLocationAsync(int imo);
    }
}