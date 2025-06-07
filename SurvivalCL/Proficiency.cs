using System;

namespace SurvivalCL
{
    public class Proficiency
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Proficiency(string name, string description = "")
        {
            Name = name;
            Description = description;
        }
    }
}