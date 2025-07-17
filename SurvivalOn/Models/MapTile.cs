using System.Text.Json.Serialization;

namespace SurvivalOn.Models
{
    public class MapTile
    {
        public int TileId { get; set; }
        public int X { get; set; }          // Tile position on map
        public int Y { get; set; }          // Tile position on map
        public int ImageX { get; set; }     // X position in source image
        public int ImageY { get; set; }     // Y position in source image
        public int Width { get; set; }      // Tile width
        public int Height { get; set; }     // Tile height
        public string? TileType { get; set; }
    }

    public class MapResponseDto
    {
        public string TilePath { get; set; } = string.Empty;
        [JsonPropertyName("MapList")]
        public List<MapTile> MapList { get; set; } = new List<MapTile>();
    }
}