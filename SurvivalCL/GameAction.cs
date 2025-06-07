using System;
using System.Collections.Generic;

namespace SurvivalCL
{
    public enum Direction
    {
        North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest
    }

    public class ActionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object>? Data { get; set; }

        public ActionResult(bool success, string message)
        {
            Success = success;
            Message = message;
            Data = new Dictionary<string, object>();
        }
    }

    public abstract class GameAction
    {
        public abstract int MinutesCost { get; }
        public abstract ActionResult Execute(GameState state, Player player, object? context = null);
    }

    // Example: Move Action
    public class MoveAction : GameAction
    {
        public Direction Direction { get; }
        public string BiomeType { get; }
        public override int MinutesCost { get; }

        public MoveAction(Direction direction, string biomeType)
        {
            Direction = direction;
            BiomeType = biomeType;

            // Diagonal moves cost more time
            bool isDiagonal = Direction == Direction.NorthEast || Direction == Direction.SouthEast ||
                              Direction == Direction.SouthWest || Direction == Direction.NorthWest;
            int baseMinutes = BiomeType switch
            {
                "Forest" => 15,
                "Mountain" => 25,
                "Plains" => 10,
                "Swamp" => 20,
                _ => 12
            };
            MinutesCost = isDiagonal ? (int)(baseMinutes * 1.4) : baseMinutes;
        }

        public override ActionResult Execute(GameState state, Player player, object? context = null)
        {
            state.AdvanceTime(MinutesCost);
            // Update player position here (not implemented)
            return new ActionResult(true, $"Moved {Direction} through {BiomeType} in {MinutesCost} minutes.");
        }
    }

    // Example: Make Fire Action
    public class MakeFireAction : GameAction
    {
        public override int MinutesCost => 20;

        public override ActionResult Execute(GameState state, Player player, object? context = null)
        {
            if (!player.Inventory.Contains("Tinder") || !player.Inventory.Contains("Wood"))
                return new ActionResult(false, "You need tinder and wood to make a fire.");

            player.Inventory.Remove("Tinder");
            player.Inventory.Remove("Wood");
            state.SetTemperature(10); // Restore warmth
            state.AdvanceTime(MinutesCost);
            return new ActionResult(true, "You made a fire and restored some warmth.");
        }
    }

    // Example: Eat Action
    public class EatAction : GameAction
    {
        public string FoodItem { get; }
        public override int MinutesCost => 5;
        public EatAction(string foodItem) => FoodItem = foodItem;

        public override ActionResult Execute(GameState state, Player player, object? context = null)
        {
            if (!player.Inventory.Contains(FoodItem))
                return new ActionResult(false, $"You don't have {FoodItem} to eat.");

            player.Inventory.Remove(FoodItem);
            player.Hunger = Math.Max(0, player.Hunger - 30); // Restore hunger
            state.AdvanceTime(MinutesCost);
            return new ActionResult(true, $"You ate {FoodItem} and restored some hunger.");
        }
    }

    // Example: Cook Action
    public class CookAction : GameAction
    {
        public string Recipe { get; }
        public List<string> Ingredients { get; }
        public bool RequiresFire { get; }
        public override int MinutesCost => 30;
        public CookAction(string recipe, List<string> ingredients, bool requiresFire)
        {
            Recipe = recipe;
            Ingredients = ingredients;
            RequiresFire = requiresFire;
        }

        public override ActionResult Execute(GameState state, Player player, object? context = null)
        {
            if (RequiresFire && !player.HasFire)
                return new ActionResult(false, "You need a fire to cook this recipe.");

            foreach (var ingredient in Ingredients)
            {
                if (!player.Inventory.Contains(ingredient))
                    return new ActionResult(false, $"Missing ingredient: {ingredient}");
            }

            foreach (var ingredient in Ingredients)
                player.Inventory.Remove(ingredient);

            player.Inventory.Add(Recipe);
            state.AdvanceTime(MinutesCost);
            return new ActionResult(true, $"You cooked {Recipe}.");
        }
    }

    // Add other actions: Search, Build, Gather, Hunt, Sleep, etc. similarly

    // Example Player class (simplified)
    public class Player
    {
        public List<string> Inventory { get; set; } = new();
        public int Hunger { get; set; } = 100;
        public bool HasFire { get; set; } = false;
        // Add more player state as needed
    }
}