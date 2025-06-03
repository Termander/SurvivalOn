namespace SurvivalOn.Models
{
    public class Creature
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsHostile { get; set; }
    }
}