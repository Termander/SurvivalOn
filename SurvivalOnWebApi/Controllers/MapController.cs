using Microsoft.AspNetCore.Mvc;
using SurvivalCL;
using System.IO;
using System.Text.Json;
using static IslandMapGenerator;

namespace SurvivalOnWebApi.Controllers
{
    public class MapResponseDto
    {
        public List<MapTile> MapList { get; set; }
        public string TilePath { get; set; }
    }

    public class MapDataRoot
    {
        public string TilePath { get; set; }
        // Add other properties as needed
    }

    [ApiController]
    [Route("api/[controller]")]
    public class MapController : ControllerBase
    {
        private string GetTilePathFromJson(string jsonPath)
        {
            var json = System.IO.File.ReadAllText(jsonPath);
            var mapData = JsonSerializer.Deserialize<MapDataRoot>(json);
            return mapData?.TilePath ?? string.Empty;
        }

        // POST: api/Map/generate
        [HttpPost("generate")]
        public IActionResult GenerateIslandMap()
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "StaticData", "MapData", "GrassIsland.json");
            var mapTiles = IslandMapGenerator.GenerateIslandMap(jsonPath);

            var mapList = new List<MapTile>();
            int width = mapTiles.GetLength(0);
            int height = mapTiles.GetLength(1);
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (mapTiles[x, y] != null)
                        mapList.Add(mapTiles[x, y]);

            var response = new MapResponseDto
            {
                MapList = mapList,
                TilePath = jsonPath
            };

            return Ok(response);
        }

        // GET: api/Map
        [HttpGet]
        public IActionResult GetIslandMap()
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "StaticData", "MapData", "GrassIsland.json");
            var mapTiles = IslandMapGenerator.GenerateIslandMap(jsonPath);

            var mapList = new List<MapTile>();
            int width = mapTiles.GetLength(0);
            int height = mapTiles.GetLength(1);
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (mapTiles[x, y] != null)
                        mapList.Add(mapTiles[x, y]);

            var response = new MapResponseDto
            {
                MapList = mapList,
                TilePath = jsonPath
            };

            return Ok(response);
        }
    }
}