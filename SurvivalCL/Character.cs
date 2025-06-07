using System;
using System.Collections.Generic;

namespace SurvivalCL
{
    public enum CharacterType
    {
        PC,
        NPC
    }

    public enum CharacterCondition
    {
        Healthy,
        Tired,
        Dizzy,
        Sick,
        Surprised,
        Motivated,
        Unconscious
    }

    public class Character
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public CharacterType Type { get; set; }
        public string Name { get; set; } = string.Empty;

        // Basic stats
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        // State
        public int MaxHP { get; set; } = 100;
        private int _hp;
        public int HP
        {
            get => _hp;
            set => _hp = Math.Clamp(value, 0, MaxHP);
        }

        private int _fatigue;
        public int Fatigue
        {
            get => _fatigue;
            set => _fatigue = Math.Clamp(value, 0, 100);
        }

        private int _stamina;
        public int Stamina
        {
            get => _stamina;
            set => _stamina = Math.Clamp(value, 0, 100);
        }

        private int _hunger;
        public int Hunger
        {
            get => _hunger;
            set => _hunger = Math.Clamp(value, 0, 100);
        }

        private int _sleep;
        public int Sleep
        {
            get => _sleep;
            set => _sleep = Math.Clamp(value, 0, 100);
        }

        public CharacterCondition Condition { get; set; } = CharacterCondition.Healthy;
        public int Experience { get; set; }

        // Proficiency levels: key = proficiency name, value = level
        public Dictionary<string, int> ProficiencyLevels { get; set; } = new();

        public Character(CharacterType type, string name)
        {
            Type = type;
            Name = name;
            MaxHP = 100;
            HP = MaxHP;
            Fatigue = 0;
            Stamina = 100;
            Hunger = 0;
            Sleep = 0;
        }

        // Add or update a proficiency level
        public void SetProficiencyLevel(string name, int level)
        {
            ProficiencyLevels[name] = level;
        }

        // Get proficiency level (0 if not present)
        public int GetProficiencyLevel(string name)
        {
            return ProficiencyLevels.TryGetValue(name, out var level) ? level : 0;
        }
    }
}