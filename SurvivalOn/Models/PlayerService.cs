public class PlayerService
{
    private readonly HttpClient _http;
    private readonly string _apiUrl;

    public PlayerService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiUrl = config["Api:BaseUrlPlayers"] ?? throw new ArgumentNullException(nameof(config), "Api:BaseUrlPlayers configuration is missing.");
    }

    // Use _apiUrl in your methods
}