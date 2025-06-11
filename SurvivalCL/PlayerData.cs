using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SurvivalCL
{
    public class PlayerData
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // For demo only! Hash in production.
        public string Email { get; set; } = string.Empty;
        public List<Guid> PCIds { get; set; } = new();
        public DateTime RegisterDate { get; set; } = DateTime.UtcNow;


        public static PlayerData? LoadPlayerDataByUserName(string userName)
        {
            var filePath = "DynamicData/players.json";
            if (!File.Exists(filePath))
                return null;

            // Read the file as an array of encrypted player objects
            var fileContent = File.ReadAllText(filePath);
            var encryptedPlayers = JsonSerializer.Deserialize<List<EncryptedPlayerFile>>(fileContent);

            if (encryptedPlayers == null)
                return null;

            foreach (var encryptedPlayer in encryptedPlayers)
            {
                var decryptedSecondJson = CryptoHelper.Decrypt(encryptedPlayer.EncryptedData);
                var secondJsonObj = JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedSecondJson);

                if (secondJsonObj == null)
                    continue;

                var decryptedUserName = CryptoHelper.Decrypt(secondJsonObj["UserName"]);
                if (string.Equals(decryptedUserName, userName, StringComparison.OrdinalIgnoreCase))
                {
                    // Found the user, build and return PlayerData
                    return new PlayerData
                    {
                        Id = encryptedPlayer.Id,
                        Name = CryptoHelper.Decrypt(secondJsonObj["Name"]),
                        Surname = CryptoHelper.Decrypt(secondJsonObj["Surname"]),
                        UserName = decryptedUserName,
                        Password = CryptoHelper.Decrypt(secondJsonObj["Password"]),
                        Email = CryptoHelper.Decrypt(secondJsonObj["Email"]),
                        PCIds = JsonSerializer.Deserialize<List<Guid>>(CryptoHelper.Decrypt(secondJsonObj["PCIds"])) ?? new List<Guid>(),
                        RegisterDate = DateTime.Parse(CryptoHelper.Decrypt(secondJsonObj["RegisterDate"]))
                    };
                }
            }

            // Not found
            return null;
        }
        public static bool SaveNewPlayer(PlayerData player, out string? error)
        {
            error = null;
            try
            {
                var filePath = "DynamicData/players.json";
                List<EncryptedPlayerFile> encryptedPlayers;

                // Load existing encrypted players if file exists, otherwise create new list
                if (File.Exists(filePath))
                {
                    var fileContent = File.ReadAllText(filePath);
                    encryptedPlayers = JsonSerializer.Deserialize<List<EncryptedPlayerFile>>(fileContent) ?? new List<EncryptedPlayerFile>();
                }
                else
                {
                    encryptedPlayers = new List<EncryptedPlayerFile>();
                }

                // Check for duplicate username
                foreach (var encryptedPlayer in encryptedPlayers)
                {
                    var decryptedSecondJson = CryptoHelper.Decrypt(encryptedPlayer.EncryptedData);
                    var secondJsonObj = JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedSecondJson);
                    if (secondJsonObj != null)
                    {
                        var decryptedUserName = CryptoHelper.Decrypt(secondJsonObj["UserName"]);
                        if (string.Equals(decryptedUserName, player.UserName, StringComparison.OrdinalIgnoreCase))
                        {
                            error = "Username already exists.";
                            return false;
                        }
                    }
                }

                // Prepare the encrypted player data
                var secondJsonObjNew = new Dictionary<string, string>
                {
                    ["Name"] = CryptoHelper.Encrypt(player.Name),
                    ["Surname"] = CryptoHelper.Encrypt(player.Surname),
                    ["UserName"] = CryptoHelper.Encrypt(player.UserName),
                    ["Password"] = CryptoHelper.Encrypt(player.Password),
                    ["Email"] = CryptoHelper.Encrypt(player.Email),
                    ["PCIds"] = CryptoHelper.Encrypt(JsonSerializer.Serialize(player.PCIds)),
                    ["RegisterDate"] = CryptoHelper.Encrypt(player.RegisterDate.ToString("o"))
                };
                string secondJson = JsonSerializer.Serialize(secondJsonObjNew);

                var encryptedPlayerNew = new EncryptedPlayerFile
                {
                    Id = player.Id,
                    EncryptedData = CryptoHelper.Encrypt(secondJson)
                };

                // Add the new encrypted player to the list
                encryptedPlayers.Add(encryptedPlayerNew);

                // Save the updated list back to the file
                Directory.CreateDirectory("DynamicData");
                var updatedJson = JsonSerializer.Serialize(encryptedPlayers, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, updatedJson);

                return true;
            }
            catch (Exception ex)
            {
                error = $"Error saving player: {ex.Message}";
                return false;
            }
        }

        public static bool UpdatePlayerDataByUserName(PlayerData updatedPlayer, out string? error)
        {
            error = null;
            try
            {
                var filePath = "DynamicData/players.json";
                if (!File.Exists(filePath))
                {
                    error = "Player data file not found.";
                    return false;
                }

                var fileContent = File.ReadAllText(filePath);
                var encryptedPlayers = JsonSerializer.Deserialize<List<EncryptedPlayerFile>>(fileContent) ?? new List<EncryptedPlayerFile>();
                bool found = false;

                for (int i = 0; i < encryptedPlayers.Count; i++)
                {
                    var decryptedSecondJson = CryptoHelper.Decrypt(encryptedPlayers[i].EncryptedData);
                    var secondJsonObj = JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedSecondJson);

                    if (secondJsonObj == null)
                        continue;

                    var decryptedUserName = CryptoHelper.Decrypt(secondJsonObj["UserName"]);
                    if (string.Equals(decryptedUserName, updatedPlayer.UserName, StringComparison.OrdinalIgnoreCase))
                    {
                        // Update the player data
                        var updatedSecondJsonObj = new Dictionary<string, string>
                        {
                            ["Name"] = CryptoHelper.Encrypt(updatedPlayer.Name),
                            ["Surname"] = CryptoHelper.Encrypt(updatedPlayer.Surname),
                            ["UserName"] = CryptoHelper.Encrypt(updatedPlayer.UserName),
                            ["Password"] = CryptoHelper.Encrypt(updatedPlayer.Password),
                            ["Email"] = CryptoHelper.Encrypt(updatedPlayer.Email),
                            ["PCIds"] = CryptoHelper.Encrypt(JsonSerializer.Serialize(updatedPlayer.PCIds)),
                            ["RegisterDate"] = CryptoHelper.Encrypt(updatedPlayer.RegisterDate.ToString("o"))
                        };
                        string updatedSecondJson = JsonSerializer.Serialize(updatedSecondJsonObj);

                        encryptedPlayers[i] = new EncryptedPlayerFile
                        {
                            Id = updatedPlayer.Id,
                            EncryptedData = CryptoHelper.Encrypt(updatedSecondJson)
                        };
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    error = "Player not found.";
                    return false;
                }

                Directory.CreateDirectory("DynamicData");
                var updatedJson = JsonSerializer.Serialize(encryptedPlayers, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, updatedJson);

                return true;
            }
            catch (Exception ex)
            {
                error = $"Error updating player: {ex.Message}";
                return false;
            }
        }

        private class EncryptedPlayerFile
        {
            public Guid Id { get; set; }
            public string EncryptedData { get; set; } = string.Empty;
        }
    }
}
