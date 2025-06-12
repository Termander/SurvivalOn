using Microsoft.JSInterop;
using SurvivalCL;
using System.Text.Json;
using System.Threading.Tasks;

namespace SurvivalOn.Services
{
    public class UserSession
    {
        private readonly IJSRuntime _jsRuntime;

        public string? Username { get; set; }
        public string? HashedPassword { get; set; }
        public Guid? CharGuid { get; set; }
        public Character? activeChar { get; set; }
        public GameState? stateOfGame { get; set; }

        public UserSession(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task SaveSessionAsync()
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorageHelper.set", "Username", Username ?? "");
            await _jsRuntime.InvokeVoidAsync("sessionStorageHelper.set", "HashedPassword", HashedPassword ?? "");
            await _jsRuntime.InvokeVoidAsync("sessionStorageHelper.set", "CharGuid", CharGuid.ToString());
            // Optionally serialize and save other properties as needed
            if (activeChar != null)
            {
                var charJson = JsonSerializer.Serialize(activeChar);
                await _jsRuntime.InvokeVoidAsync("sessionStorageHelper.set", "activeChar", charJson);
            }
            if (stateOfGame != null)
            {
                var stateJson = JsonSerializer.Serialize(stateOfGame);
                await _jsRuntime.InvokeVoidAsync("sessionStorageHelper.set", "stateOfGame", stateJson);
            }
        }

        public async Task LoadSessionAsync()
        {
            Username = await _jsRuntime.InvokeAsync<string>("sessionStorageHelper.get", "Username");
            HashedPassword = await _jsRuntime.InvokeAsync<string>("sessionStorageHelper.get", "HashedPassword");

            var guidString = await _jsRuntime.InvokeAsync<string>("sessionStorageHelper.get", "CharGuid");
            if (Guid.TryParse(guidString, out var guid))
            {
                CharGuid = guid;
            }
            var charJson = await _jsRuntime.InvokeAsync<string>("sessionStorageHelper.get", "activeChar");
            if (!string.IsNullOrEmpty(charJson))
            {
                activeChar = JsonSerializer.Deserialize<Character>(charJson);
            }
            var stateJson = await _jsRuntime.InvokeAsync<string>("sessionStorageHelper.get", "stateOfGame");
            if (!string.IsNullOrEmpty(stateJson))
            {
                stateOfGame = JsonSerializer.Deserialize<GameState>(stateJson);
            }
        }
    }
}
