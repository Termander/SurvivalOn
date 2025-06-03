using System;
using System.Collections.Generic;

namespace SurvivalOn.Models
{
    public enum PhysicalCondition
    {
        Healthy,
        Sick,
        Injured,
        PassedOut,
        Sleeping
    }

    public enum BodyPart
    {
        Head,
        Face,
        Neck,
        UpperBody,
        LowerBody,
        Hands,
        Feet,
        LeftHand,
        RightHand,
        LeftFoot,
        RightFoot,
        LeftRing1,
        LeftRing2,
        RightRing1,
        RightRing2,
        LeftBracelet,
        RightBracelet
    }

  
    public class Character
    {
        // Basic Info
        public string Name { get; set; } = string.Empty;

        // Attributes
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Stamina { get; set; }
        public int MaxStamina { get; set; }
        public PhysicalCondition Condition { get; set; } = PhysicalCondition.Healthy;

        public int Strength { get; set; }
        public int Constitution { get; set; }
        public int Dexterity { get; set; }
        public int Wisdom { get; set; }
        public int Intelligence { get; set; }
        public int Charisma { get; set; }

        public double HeightCm { get; set; }
        public double WeightKg { get; set; }

        // Carrying capacity (example formula: (Str + Con) * 5)
        public double CarryingCapacity => (Strength + Constitution) * 5.0;

        // Equipment slots
        public Dictionary<BodyPart, Equipment?> Equipped { get; set; } = new()
        {
            { BodyPart.Head, null },
            { BodyPart.Face, null },
            { BodyPart.Neck, null },
            { BodyPart.UpperBody, null },
            { BodyPart.LowerBody, null },
            { BodyPart.Hands, null },
            { BodyPart.Feet, null },
            { BodyPart.LeftHand, null },
            { BodyPart.RightHand, null },
            { BodyPart.LeftFoot, null },
            { BodyPart.RightFoot, null },
            { BodyPart.LeftRing1, null },
            { BodyPart.LeftRing2, null },
            { BodyPart.RightRing1, null },
            { BodyPart.RightRing2, null },
            { BodyPart.LeftBracelet, null },
            { BodyPart.RightBracelet, null }
        };
        // Proficiencies
        public List<Proficiency> Proficiencies { get; set; } = new();

        // Armor Class: base 10 + Dex modifier + sum of equipped armor bonuses
        public int ArmorClass
        {
            get
            {
                int dexMod = (Dexterity - 10) / 2;
                int armorBonus = 0;
                foreach (var eq in Equipped.Values)
                {
                    if (eq != null)
                        armorBonus += eq.ArmorBonus;
                }
                return 10 + dexMod + armorBonus;
            }
        }
    }
}