using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class IslandMapGenerator
{
    public class TileDef
    {
        public int Id { get; set; }
        public string TileType { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Oriantation { get; set; } = "";
        public List<int> N { get; set; } = new();
        public List<int> S { get; set; } = new();
        public List<int> E { get; set; } = new();
        public List<int> W { get; set; } = new();
    }

    public class GrassIslandTemplate
    {
        public string TilePath { get; set; } = "";
        public List<TileDef> Tiles { get; set; } = new();
    }

    public class MapTile
    {
        public int TileId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    public static MapTile[,] GenerateIslandMap(string jsonPath)
    {
        var json = File.ReadAllText(jsonPath);
        var template = JsonSerializer.Deserialize<GrassIslandTemplate>(json);

        // Find water tiles
        var waterTiles = template.Tiles.FindAll(t => t.TileType.Equals("Water"));

        // Find land tiles
        var landTiles = template.Tiles.FindAll(t => !t.TileType.Equals("Water"));

        int sectors = 12;
        int sectorSize = 2; // Each sector is 2x2 tiles
        int mapSize = sectors * sectorSize; // 24x24 tiles

        var map = new MapTile[mapSize, mapSize];
        var rand = new Random();
        try
        {


            for (int sx = 0; sx < sectors; sx++)
            {
                for (int sy = 0; sy < sectors; sy++)
                {
                    bool isEdge = sx == 0 || sy == 0 || sx == sectors - 1 || sy == sectors - 1;
                    for (int tx = 0; tx < sectorSize; tx++)
                    {
                        for (int ty = 0; ty < sectorSize; ty++)
                        {
                            int mx = sx * sectorSize + tx;
                            int my = sy * sectorSize + ty;
                            TileDef chosenTile;

                            if (isEdge)
                            {
                                // Only water tiles on the edge
                                chosenTile = waterTiles[rand.Next(waterTiles.Count)];
                            }
                            else
                            {
                                // Pick a land tile that matches neighbors
                                var candidates = new List<TileDef>(landTiles);
                                // Check N neighbor
                                if (my > 0)
                                {
                                    var nTileId = map[mx, my - 1]?.TileId ?? -1;
                                    candidates.RemoveAll(t => !t.N.Contains(nTileId));
                                }
                                // Check W neighbor
                                if (mx > 0)
                                {
                                    var wTileId = map[mx - 1, my]?.TileId ?? -1;
                                    candidates.RemoveAll(t => !t.W.Contains(wTileId));
                                }
                                if (candidates.Count == 0)
                                    chosenTile = landTiles[rand.Next(landTiles.Count)];
                                else
                                    chosenTile = candidates[rand.Next(candidates.Count)];
                            }

                            map[mx, my] = new MapTile
                            {
                                TileId = chosenTile.Id,
                                X = mx,
                                Y = my
                            };
                        }
                    }
                }
            }
        }
        catch (Exception er)
        {

            string a=  er.Message;
        }
        return map;
    }
}