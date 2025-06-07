using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using SurvivalOn.Models;

namespace SurvivalOn.Models
{
    public static class PlayerDataGenerator
    {

        public static async Task<(PlayerData? player, string? error)> LoadPlayerDataByUserNameAsync(string userName, string apiUrl)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(apiUrl))
                return (null, "Invalid username or API URL.");

            using HttpClient http = new HttpClient();
            try
            {
                var url = $"{apiUrl.TrimEnd('/')}/GetPlayerByUsername/{Uri.EscapeDataString(userName)}";
                var response = await http.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var player = await response.Content.ReadFromJsonAsync<PlayerData>();
                    return (player, null);
                }
                else
                {
                    return (null, $"API error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                return (null, $"HTTP error: {ex.Message}");
            }
        }


        public static async Task<(bool success, string? error)> SaveNewPlayerAsync(PlayerData player, string apiUrl)
        {
            using HttpClient http = new HttpClient();
            try
            {
                var response = await http.PostAsJsonAsync(apiUrl, player);

                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }
                else
                {
                    return (false, $"API error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"HTTP error: {ex.Message}");
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