using System;
using System.Collections.Generic;

namespace SurvivalOn.Models
{
    public class Locations
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public Biome Biome { get; set; } = new Biome();
        public List<Material> VisibleMaterials { get; set; } = new();
        public List<Material> UnvisibleMaterials { get; set; } = new();
        public List<Creature> VisibleCreatures { get; set; } = new();
        public List<Creature> UnvisibleCreatures { get; set; } = new();
        public List<PC> PCs { get; set; } = new();
        public List<NPC> NPCs { get; set; } = new();
        public List<Command> ActionCommands { get; set; } = new();

        // Coordinates in the area grid
        public int X { get; set; }
        public int Y { get; set; }
    }
}