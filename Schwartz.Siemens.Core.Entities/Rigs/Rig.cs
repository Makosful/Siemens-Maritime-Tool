using System.Collections.Generic;

namespace Schwartz.Siemens.Core.Entities.Rigs
{
    public class Rig
    {
        public int Imo { get; set; }
        public string Name { get; set; }
        public List<Location> Location { get; set; }
    }
}