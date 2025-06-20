using System;
using System.Collections.Generic;
using System.Text.Json;

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
        public int MaxHP { get; set; } = 100; // Default max HP
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

        private int _bodyWarmth;
        public int BodyWarmth
        {
            get => _bodyWarmth;
            set => _bodyWarmth = value;
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
            Fatigue = 100;
            Stamina = 100;
            Hunger = 100;
            Sleep = 100;
            BodyWarmth = 100;
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

        public static void SaveCharacterToFile(Character character, GameState? gameState   )
        {
            if(gameState == null)
                gameState = new GameState(true); // Create a new GameState if not provided

            string directory = "DynamicData/Character/" + character.Id;
            Directory.CreateDirectory(directory);
            var charFile = Path.Combine(directory, $"{character.Id}.json");

            var wrapper = new CharacterGameStateWrapper
            {
                Character = character,
                GameState = gameState
            };

            var json = JsonSerializer.Serialize(wrapper, new JsonSerializerOptions { WriteIndented = true });
            var encrypted = CryptoHelper.Encrypt(json);
            File.WriteAllText(charFile, encrypted);
        }

        public static (Character? character, GameState? gameState) LoadCharacterWithGameState(Guid id)
        {
            string directory = "DynamicData/Character/" + id;
            var charFile = Path.Combine(directory, $"{id}.json");
            if (!File.Exists(charFile))
                return (null, null);

            var encrypted = File.ReadAllText(charFile);
            var json = CryptoHelper.Decrypt(encrypted);
            var wrapper = JsonSerializer.Deserialize<CharacterGameStateWrapper>(json);
            return (wrapper?.Character, wrapper?.GameState);
        }

        public static void UpdateCharacter(Character character, GameState? gameState)
        {
            SaveCharacterToFile(character, gameState);
        }
        public static Character? LoadCharacter(Guid id)
        {
            var (character, _) = LoadCharacterWithGameState(id);
            return character;
        }
        public void UpdateBodyWarmth(float ambientTemperature)
        {
            // Ideal comfort range: 20–23°C
            if (ambientTemperature >= 20f && ambientTemperature <= 23f)
            {
                BodyWarmth = 100;
            }
            else if (ambientTemperature < 20f)
            {
                BodyWarmth = (int)(100 - (20f - ambientTemperature) * 4);
            }
            else // ambientTemperature > 23°C
            {
                BodyWarmth = (int)(100 - (ambientTemperature - 23f) * 3);
            }
        }

        public static string GetBodyWarmthDescription(int warmth)
        {
            if (warmth < -40) return "You are frozen solid, life is slipping away with every second.";
            if (warmth < -30) return "Your body is in deep hypothermia, barely clinging to life.";
            if (warmth < -20) return "You are in critical hypothermia, shivering uncontrollably.";
            if (warmth < -10) return "You are dangerously cold, numbness spreading through your limbs.";
            if (warmth < 0) return "You are severely cold, your body is shutting down.";
            if (warmth < 10) return "You are on the verge of hypothermia, shivering violently.";
            if (warmth < 20) return "You are dangerously cold, every breath is a struggle.";

            if (warmth < 30) return "You are uncomfortably cold, your teeth chatter and movement is slow.";
            if (warmth < 40) return "You feel cold, your fingers and toes ache from the chill.";
            if (warmth < 50) return "You are chilly, wishing for more warmth.";
            if (warmth < 60) return "You feel a persistent coolness, but it's bearable.";

            if (warmth < 70) return "You are comfortably warm, your body feels at ease.";
            if (warmth < 80) return "You are in the ideal warmth zone, perfectly comfortable.";
            if (warmth < 90) return "You feel perfectly warm and content, energy flows easily.";

            if (warmth < 100) return "You are slightly warm, but still comfortable.";
            if (warmth < 110) return "You are getting a bit hot, sweat beads on your skin.";
            if (warmth < 120) return "You are uncomfortably hot, your body struggles to cool down.";
            if (warmth < 130) return "You are very hot, dizziness and fatigue set in.";

            if (warmth < 140) return "You are dangerously overheated, your vision blurs and heart races.";
            if (warmth < 150) return "You are in severe heat stress, confusion and weakness overwhelm you.";
            if (warmth < 160) return "You are on the verge of heatstroke, your body is failing.";
            if (warmth < 170) return "You are suffering from heatstroke, collapse is imminent.";
            if (warmth < 180) return "You are critically overheated, organ failure is likely.";
            if (warmth < 190) return "You are at the edge of survival, your body is shutting down from heat.";
            if (warmth < 200) return "You are moments from death by heat, consciousness fading.";

            return "You have surpassed all human limits; survival is impossible at this warmth.";
        }

        public void ApplyWarmthEffects()
        {
            if (BodyWarmth > 100)
            {
                // Overheated: apply negative effects
                Condition = CharacterCondition.Sick;
                Fatigue = Math.Max(Fatigue - 5, 0);
                Stamina = Math.Max(Stamina - 5, 0);
            }
            else if (BodyWarmth < 0)
            {
                // Severely cold: apply negative effects
                Condition = CharacterCondition.Sick;
                Fatigue = Math.Max(Fatigue - 10, 0);
                Stamina = Math.Max(Stamina - 10, 0);
            }
            else if (BodyWarmth < 30)
            {
                // Chilly: apply mild negative effects
                Condition = CharacterCondition.Tired;
                Fatigue = Math.Max(Fatigue - 2, 0);
            }
            else if (BodyWarmth > 90 && BodyWarmth <= 100)
            {
                // Ideal: restore some fatigue/stamina
                Fatigue = Math.Min(Fatigue + 2, 100);
                Stamina = Math.Min(Stamina + 2, 100);
                Condition = CharacterCondition.Healthy;
            }
            // You can expand this logic for more nuanced effects
        }
    }
 
    public class CharacterGameStateWrapper
    {
        public Character Character { get; set; } = null!;
        public GameState GameState { get; set; } = null!;
    }

    public static class CharacterStatusDescriptions
{
    public static string GetFatigueDescription(int fatigue)
    {
        if (fatigue >= 91) return "You feel invigorated, as if you could conquer mountains.";
        if (fatigue >= 81) return "A spring in your step and a clear mind—fatigue is a distant memory.";
        if (fatigue >= 71) return "You move with purpose, only a hint of weariness in your limbs.";
        if (fatigue >= 61) return "A gentle tiredness tugs at you, but you press on with ease.";
        if (fatigue >= 51) return "Your muscles ache slightly, and you long for a moment's rest.";
        if (fatigue >= 41) return "Fatigue weighs on you, slowing your movements and dulling your senses.";
        if (fatigue >= 31) return "Every action feels heavier, your body yearning for respite.";
        if (fatigue >= 21) return "Exhaustion gnaws at your will, each step a test of resolve.";
        if (fatigue >= 11) return "You stagger, vision blurring, as your strength nearly fails you.";
        return "Utterly spent, you collapse, unable to continue without rest.";
    }

    public static string GetStaminaDescription(int stamina)
    {
        if (stamina >= 91) return "Energy surges through you, ready for any challenge.";
        if (stamina >= 81) return "You feel robust, able to take on strenuous tasks with ease.";
        if (stamina >= 71) return "Your stamina is strong, though you sense it slowly waning.";
        if (stamina >= 61) return "You can keep going, but a break would be welcome.";
        if (stamina >= 51) return "You’re starting to feel the burn, but you push through.";
        if (stamina >= 41) return "Your body protests, each effort more taxing than the last.";
        if (stamina >= 31) return "You move sluggishly, your reserves running low.";
        if (stamina >= 21) return "Every movement is a struggle, your stamina nearly depleted.";
        if (stamina >= 11) return "You gasp for breath, barely able to continue.";
        return "Your body gives out, unable to muster the strength to go on.";
    }

    public static string GetHungerDescription(int hunger)
    {
        if (hunger >= 91) return "You are well-fed and content, ready for anything.";
        if (hunger >= 81) return "A pleasant fullness lingers, hunger is but a memory.";
        if (hunger >= 71) return "You feel satisfied, though a snack wouldn’t hurt.";
        if (hunger >= 61) return "A mild hunger stirs, but you remain focused.";
        if (hunger >= 51) return "Your stomach rumbles, reminding you it’s time to eat.";
        if (hunger >= 41) return "Hunger distracts you, sapping your concentration.";
        if (hunger >= 31) return "You feel weak, your body craving nourishment.";
        if (hunger >= 21) return "Starvation sets in, your strength fading fast.";
        if (hunger >= 11) return "You are dizzy and frail, desperate for food.";
        return "On the brink of collapse, you can barely move from hunger.";
    }

    public static string GetSleepDescription(int sleep)
    {
        if (sleep >= 91) return "You are wide awake, mind sharp and alert.";
        if (sleep >= 81) return "Rested and refreshed, you greet the day with vigor.";
        if (sleep >= 71) return "You feel good, though a nap would be welcome.";
        if (sleep >= 61) return "A gentle drowsiness creeps in, but you remain attentive.";
        if (sleep >= 51) return "You yawn, eyelids heavy, longing for sleep.";
        if (sleep >= 41) return "Fatigue clouds your mind, focus slipping away.";
        if (sleep >= 31) return "You struggle to stay awake, thoughts muddled by exhaustion.";
        if (sleep >= 21) return "Sleep deprivation takes its toll, your body begging for rest.";
        if (sleep >= 11) return "You are barely conscious, fighting to keep your eyes open.";
        return "Sleep claims you, and you drift into unconsciousness.";
    }

    public static string GetConditionDescription(CharacterCondition condition)
    {
        return condition switch
        {
            CharacterCondition.Healthy => "You are in peak condition, ready to face whatever comes your way.",
            CharacterCondition.Tired => "Weariness drapes over you, slowing your every move.",
            CharacterCondition.Dizzy => "The world spins, your balance and focus slipping away.",
            CharacterCondition.Sick => "Illness wracks your body, draining your strength and resolve.",
            CharacterCondition.Surprised => "You are caught off guard, scrambling to react.",
            CharacterCondition.Motivated => "A surge of inspiration fills you, driving you to excel.",
            CharacterCondition.Unconscious => "You are lost to the world, senses and will faded into darkness.",
            _ => "You feel... indescribable."
        };
    }
}
}