using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SurvivalCL;

namespace SurvivalOnWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private static readonly string FilePath = Path.Combine("DynamicData", "players.json");

        [HttpPost]
        public IActionResult RegisterPlayer([FromBody] PlayerData player)
        {
            try
            {
                string error = string.Empty;
                if (PlayerData.SaveNewPlayer(player, out error!))
                {
                    return Ok();
                }
                else
                {
                    if (error == "Username already exists.")
                        return Conflict("Username already exists.");
                    else
                        throw new Exception("Failed to save player data: " + error);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error saving player: {ex.Message}");
            }
        }

        [HttpGet("{username}")]
        public IActionResult GetPlayerByUsername(string username)
        {
            try
            {
                var player = PlayerData.LoadPlayerDataByUserName(username);
                if (player == null)
                    return NotFound("Player not found.");

                // For security, you may want to exclude the password in the response:
                player.Password = string.Empty;

                return Ok(player);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading player: {ex.Message}");
            }
        }

        [HttpGet("authenticate")]
        public IActionResult Authenticate([FromQuery] string username, [FromQuery] string password)
        {
            try
            {
                var player = PlayerData.LoadPlayerDataByUserName(username);
                if (player == null)
                    return NotFound("Player not found.");

                // For demo only! In production, use hashed passwords.
                if (player.Password != password)
                    return Unauthorized("Invalid password.");

                // Hide password in response
                player.Password = string.Empty;
                return Ok(player);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error authenticating player: {ex.Message}");
            }
        }

        // Helper class for deserialization
        private class EncryptedPlayerFile
        {
            public Guid Id { get; set; }
            public string EncryptedData { get; set; } = string.Empty;
        }
    }
}