using Microsoft.AspNetCore.Mvc;
using SurvivalCL;
using System.Collections.Generic;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace SurvivalOnWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        // In-memory store for demonstration. Replace with your data access logic.
        private static readonly List<Character> Characters = new();


        //private readonly string characterDir = "DynamicData/Character";
        //private readonly string playersFile = "DynamicData/players.json";

        [HttpGet]
        public ActionResult<IEnumerable<Character>> GetAll()
        {
            return Ok(Characters);
        }

        [HttpGet("{id}")]
        public ActionResult<Character> GetById(Guid id)
        {
            var character = Characters.Find(c => c.Id == id);
            if (character == null)
                return NotFound();
            return Ok(character);
        }

        //[HttpPost]
        //public ActionResult<Character> Create(Character character)
        //{
        //    character.Id = Guid.NewGuid();
        //    Characters.Add(character);
        //    return CreatedAtAction(nameof(GetById), new { id = character.Id }, character);
        //}

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateCharacterRequest request)
        {
            // 1. Load player data by username and hashed password
            var player = PlayerData.LoadPlayerDataByUserName(request.UserName);
            if (player == null)
                return NotFound("Player not found.");

            // Hash the provided password and compare
            //var hashedInput = HashPassword(request.Password);
            if (!string.Equals(player.Password, request.Password, StringComparison.Ordinal))
                return Unauthorized("Invalid password.");

            // 2. Create new character and save to file
            var character = request.Character;
            character.Id = Guid.NewGuid();

            // Create and initialize a new GameState for the character
            var gameState = new GameState(true);
           

            Character.SaveCharacterToFile(character,gameState);

            // 3. Add character id to player's PCIds[0] (replace or add)
            if (player.PCIds.Count == 0)
                player.PCIds.Add(character.Id);
            else
                player.PCIds[0] = character.Id;

            // 4. Save player data (encrypted, as before)
            string? error;
            //if (!PlayerData.SaveNewPlayer(player, out error))
            //    return StatusCode(500, error);

            if (!PlayerData.UpdatePlayerDataByUserName(player, out error))
                return StatusCode(500, error);

            return Ok(new { CharacterId = character.Id, GameState = gameState });
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, Character updated)
        {
            var character = Characters.Find(c => c.Id == id);
            if (character == null)
                return NotFound();

            // Update properties as needed
            character.Name = updated.Name;
            character.Type = updated.Type;
            character.Strength = updated.Strength;
            character.Dexterity = updated.Dexterity;
            character.Constitution = updated.Constitution;
            character.Intelligence = updated.Intelligence;
            character.Wisdom = updated.Wisdom;
            character.Charisma = updated.Charisma;
            character.ProficiencyLevels = updated.ProficiencyLevels;

            return NoContent();
        }

        [HttpGet("load/{id}")]
        public IActionResult LoadCharacterWithGameState(Guid id)
        {
            var (character, gameState) = Character.LoadCharacterWithGameState(id);
            if (character == null)
                return NotFound("Character not found.");

            return Ok(new { Character = character, GameState = gameState });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            var character = Characters.Find(c => c.Id == id);
            if (character == null)
                return NotFound();

            Characters.Remove(character);
            return NoContent();
        }

        // Simple hash for demo (replace with a secure hash in production)
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public class CreateCharacterRequest
        {
            public string UserName { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public Character Character { get; set; } = new Character(CharacterType.PC, "");
        }
    }
}