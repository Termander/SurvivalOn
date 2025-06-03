using System;

namespace SurvivalOn.Models
{
    public class Area
    {
        public string Name { get; set; } = string.Empty;
        public Locations[,] Location { get; set; } = new Locations[10, 10];
    }
}