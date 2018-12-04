using System;

namespace Schwartz.Siemens.Core.Entities.Rigs
{
    public class Location
    {
        public int Id { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public DateTime Date { get; set; }
        public Rig Rig { get; set; }
        public string Area { get; set; } = "Unknown";
        public string Status { get; set; } = "Unknown";
    }
}