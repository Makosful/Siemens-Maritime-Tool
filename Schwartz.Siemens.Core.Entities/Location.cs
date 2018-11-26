using System;

namespace Schwartz.Siemens.Core.Entities
{
    public class Location
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime Date { get; set; }
        public Rig Rig { get; set; }
    }
}