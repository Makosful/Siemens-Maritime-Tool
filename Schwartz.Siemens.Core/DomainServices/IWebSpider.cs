using Schwartz.Siemens.Core.Entities.Rigs;
using System.Collections.Generic;

namespace Schwartz.Siemens.Core.DomainServices
{
    public interface IWebSpider
    {
        /// <summary>
        /// This method will get the latest known location of the Rig with the specified ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Location GetLatestLocation(int id);

        /// <summary>
        /// This method will get a collection of Locations for the Rigs with the specified IDs
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        IEnumerable<Location> GetMultipleLocations(IEnumerable<int> ids);
    }
}