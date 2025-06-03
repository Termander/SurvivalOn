using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SurvivalOn.Models;

namespace SurvivalOn.Models
{
    public static class PlayerDataGenerator
    {
        // 1. Create and save encrypted player data
        public static void CreatePlayerDataFiles()
        {
            var player = new PlayerData
            {
                Name = "Cüneyt",
                Surname = "Özem",
                UserName = "Zadeh",
                Password = "termander",
                Email = "cuneytozem@hotmail.com",
                PCIds = new List<Guid>(),
                RegisterDate = DateTime.UtcNow
            };

            var secondJsonObj = new Dictionary<string, string>
            {
                ["Name"] = CryptoHelper.Encrypt(player.Name),
                ["Surname"] = CryptoHelper.Encrypt(player.Surname),
                ["UserName"] = CryptoHelper.Encrypt(player.UserName),
                ["Password"] = CryptoHelper.Encrypt(player.Password),
                ["Email"] = CryptoHelper.Encrypt(player.Email),
                ["PCIds"] = CryptoHelper.Encrypt(JsonSerializer.Serialize(player.PCIds)),
                ["RegisterDate"] = CryptoHelper.Encrypt(player.RegisterDate.ToString("o"))
            };
            string secondJson = JsonSerializer.Serialize(secondJsonObj);

            var firstJsonObj = new
            {
                Id = player.Id,
                EncryptedData = CryptoHelper.Encrypt(secondJson)
            };
            string firstJson = JsonSerializer.Serialize(firstJsonObj, new JsonSerializerOptions { WriteIndented = true });

            Directory.CreateDirectory("DynamicData");
            File.WriteAllText("DynamicData/players.json", firstJson);
        }

        // 2. Load and decrypt player data from file
        public static PlayerData? LoadPlayerDataFromFile()
        {
            if (!File.Exists("DynamicData/players.json"))
                return null;

            var firstJson = File.ReadAllText("DynamicData/players.json");
            var firstJsonObj = JsonSerializer.Deserialize<EncryptedPlayerFile>(firstJson);

            if (firstJsonObj == null)
                return null;

            // Decrypt the second JSON
            var decryptedSecondJson = CryptoHelper.Decrypt(firstJsonObj.EncryptedData);
            var secondJsonObj = JsonSerializer.Deserialize<Dictionary<string, string>>(decryptedSecondJson);

            if (secondJsonObj == null)
                return null;

            // Decrypt each field
            var player = new PlayerData
            {
                Id = firstJsonObj.Id,
                Name = CryptoHelper.Decrypt(secondJsonObj["Name"]),
                Surname = CryptoHelper.Decrypt(secondJsonObj["Surname"]),
                UserName = CryptoHelper.Decrypt(secondJsonObj["UserName"]),
                Password = CryptoHelper.Decrypt(secondJsonObj["Password"]),
                Email = CryptoHelper.Decrypt(secondJsonObj["Email"]),
                PCIds = JsonSerializer.Deserialize<List<Guid>>(CryptoHelper.Decrypt(secondJsonObj["PCIds"])) ?? new List<Guid>(),
                RegisterDate = DateTime.Parse(CryptoHelper.Decrypt(secondJsonObj["RegisterDate"]))
            };

            return player;
        }

        // 3. Load and decrypt player data by username
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

        // 4. Save a new player with encryption to the array in players.json
        public static void SaveNewPlayer(PlayerData player)
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

            // Prepare the encrypted player data
            var secondJsonObj = new Dictionary<string, string>
            {
                ["Name"] = CryptoHelper.Encrypt(player.Name),
                ["Surname"] = CryptoHelper.Encrypt(player.Surname),
                ["UserName"] = CryptoHelper.Encrypt(player.UserName),
                ["Password"] = CryptoHelper.Encrypt(player.Password),
                ["Email"] = CryptoHelper.Encrypt(player.Email),
                ["PCIds"] = CryptoHelper.Encrypt(JsonSerializer.Serialize(player.PCIds)),
                ["RegisterDate"] = CryptoHelper.Encrypt(player.RegisterDate.ToString("o"))
            };
            string secondJson = JsonSerializer.Serialize(secondJsonObj);

            var encryptedPlayer = new EncryptedPlayerFile
            {
                Id = player.Id,
                EncryptedData = CryptoHelper.Encrypt(secondJson)
            };

            // Add the new encrypted player to the list
            encryptedPlayers.Add(encryptedPlayer);

            // Save the updated list back to the file
            Directory.CreateDirectory("DynamicData");
            var updatedJson = JsonSerializer.Serialize(encryptedPlayers, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, updatedJson);
        }

        // 5. Save a new player with encryption to the array in players.json with error handling
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

        // Helper class for deserialization
        private class EncryptedPlayerFile
        {
            public Guid Id { get; set; }
            public string EncryptedData { get; set; } = string.Empty;
        }
    }
}