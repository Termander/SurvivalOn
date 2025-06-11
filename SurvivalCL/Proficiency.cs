using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SurvivalCL
{
    public enum ProficiencyType
    {
        Combat,
        Build,
        General
    }

    public enum AbilityType
    {
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma
    }

    public class Proficiency
    {
        public Guid UniqueId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty; 
        public string Description { get; set; } = string.Empty; 
        public ProficiencyType Type { get; set; }
        public bool IsBasic { get; set; }
        // Ability prerequisites: key = ability, value = minimum required
        public Dictionary<AbilityType, int> AbilityPrerequisites { get; set; } = new();
        // Prerequisite proficiencies: key = proficiency name, value = required level
        public Dictionary<string, int> PrerequisiteProficiencies { get; set; } = new();
        // Bonus: key = ability, value = bonus amount (can have multiple)
        public Dictionary<AbilityType, int> BonusAbilities { get; set; } = new();
        public string ImagePath { get; set; } = string.Empty;
        public string BonusDescription { get; set; } = string.Empty;

        public Proficiency() { }

        // Loads the image from ImagePath and returns as byte array (for WebAPI)
        public byte[]? GetImageBytes()
        {
            if (string.IsNullOrWhiteSpace(ImagePath) || !File.Exists(ImagePath))
                return null;
            return File.ReadAllBytes(ImagePath);
        }

        public static List<Proficiency> GetAllBasicProficiencies(string jsonPath)
        {
            if (!File.Exists(jsonPath))
                return new List<Proficiency>();

            //var json = File.ReadAllText(jsonPath);
            //var allProficiencies = JsonSerializer.Deserialize<List<Proficiency>>(json, new JsonSerializerOptions
           // {
            //    PropertyNameCaseInsensitive = true
            //}) ?? new List<Proficiency>();

            var json = System.IO.File.ReadAllText(jsonPath);
            var allProficiencies = System.Text.Json.JsonSerializer.Deserialize<List<Proficiency>>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            }) ?? new List<Proficiency>();

            var result = new List<Proficiency>();
            
            foreach (var proficiency in allProficiencies)
            {
                if (proficiency.IsBasic)
                {
                    result.Add(proficiency);
                }
            }
            return result;
        }

        // If return list is empty, all prerequisites are met
        public List<string> GetFailedPrerequisiteReasons(SurvivalCL.Character character)
        {
            var reasons = new List<string>();

            // Proficiency prerequisite failure messages
            foreach (var prereq in PrerequisiteProficiencies)
            {
                if (!character.ProficiencyLevels.TryGetValue(prereq.Key, out int level))
                {
                    reasons.Add($"You have to learn {prereq.Key}.");
                }
                else if (level < prereq.Value)
                {
                    reasons.Add($"You have to work on {prereq.Key} more.");
                }
            }


            // Ability prerequisite failure messages
            if (reasons.Count < 1)
            {
                foreach (var prereq in AbilityPrerequisites)
                {
                    int abilityValue = prereq.Key switch
                    {
                        AbilityType.Strength => character.Strength,
                        AbilityType.Dexterity => character.Dexterity,
                        AbilityType.Constitution => character.Constitution,
                        AbilityType.Intelligence => character.Intelligence,
                        AbilityType.Wisdom => character.Wisdom,
                        AbilityType.Charisma => character.Charisma,
                        _ => 0
                    };

                    if (abilityValue < prereq.Value)
                    {
                        string reason = prereq.Key switch
                        {
                            AbilityType.Strength => "You are too weak for this.",
                            AbilityType.Constitution => "You are not tough enough.",
                            AbilityType.Dexterity => "You are not agile enough.",
                            AbilityType.Intelligence => "You are not smart enough.",
                            AbilityType.Wisdom => "You lack the wisdom for this.",
                            AbilityType.Charisma => "You are not charming enough.",
                            _ => "You do not meet the ability requirement."
                        };
                        reasons.Add(reason);
                    }
                }
            }

            return reasons;
        }
    }
}