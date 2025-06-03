using System.Collections.Generic;

namespace SurvivalOn.Models
{
    public class Proficiency
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Level { get; set; } = 1;

        public List<ProficiencyPrerequisite> Prerequisites { get; set; } = new();
        public List<ProficiencyBonus> Bonuses { get; set; } = new();
    }

    public class ProficiencyPrerequisite
    {
        // Type: "Ability" or "Proficiency"
        public string Type { get; set; } = string.Empty;
        // Name of the required ability or proficiency
        public string Name { get; set; } = string.Empty;
        // Minimum required level/value
        public int Level { get; set; } = 1;
    }

    public class ProficiencyBonus
    {
        // Type: "Ability", "Proficiency", "StateCheck", etc.
        public string Type { get; set; } = string.Empty;
        // Name of the ability/proficiency/state affected
        public string Name { get; set; } = string.Empty;
        // Level or value of the bonus
        public int Level { get; set; } = 1;
        // Human-readable description of the bonus
        public string Description { get; set; } = string.Empty;
    }
}