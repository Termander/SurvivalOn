namespace SurvivalOn.Models
{
    
    public class Equipment
    {
        public string Name { get; set; } = string.Empty;
        public BodyPart Slot { get; set; }
        public int ArmorBonus { get; set; } = 0;
        // Add more properties as needed (e.g., weight, effects, durability)
    }
}