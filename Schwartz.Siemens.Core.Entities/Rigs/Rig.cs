using System.Collections.Generic;

namespace Schwartz.Siemens.Core.Entities.Rigs
{
    public class Rig
    {
        /// <summary>
        /// ID Is for database only. Try ot rely on IMO instead
        /// </summary>
        public int Id { get; set; }

        public int Imo { get; set; }
        public string Name { get; set; }
        public List<Location> Locations { get; set; }
        public bool Outdated { get; set; }
    }
}