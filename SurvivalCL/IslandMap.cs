using System;
using System.Collections.Generic;

namespace SurvivalCL
{
    public enum TileType
    {
        Water,
        Sand,
        Grass,
        Forest,
        River,
        Lake
    }

    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Type { get; set; }
        public string? TileSetImage { get; set; }
    }

    public class IslandMap
    {
        public int Width { get; }
        public int Height { get; }
        public Tile[,] Tiles { get; }
        public int LandCount { get; private set; }
        public int WaterCount { get; private set; }

        /// <summary>
        /// seaToLandRatio: 0.1 = 10% sea, 0.3 = 30% sea, etc. (land = 1 - seaToLandRatio)
        /// </summary>
        public IslandMap(int width = 10, int height = 10, double seaToLandRatio = 0.2)
        {
            Width = width;
            Height = height;
            Tiles = new Tile[width, height];
            GenerateIsland(seaToLandRatio);
        }

        private void GenerateIsland(double seaToLandRatio)
        {
            var rand = new Random();
            // 1. Fill with water
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                Tiles[x, y] = new Tile { X = x, Y = y, Type = TileType.Water };

            // 2. Random land mass using cellular automata
            double landRatio = 1.0 - seaToLandRatio;
            int targetLand = (int)(Width * Height * landRatio);

            // Seed random land
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                if (rand.NextDouble() < landRatio)
                    Tiles[x, y].Type = TileType.Grass;

            // Smooth with cellular automata
            for (int step = 0; step < 3; step++)
            {
                var copy = new TileType[Width, Height];
                for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    int landNeighbors = 0;
                    for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = x + dx, ny = y + dy;
                        if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
                            if (Tiles[nx, ny].Type == TileType.Grass)
                                landNeighbors++;
                    }
                    if (landNeighbors >= 5)
                        copy[x, y] = TileType.Grass;
                    else
                        copy[x, y] = TileType.Water;
                }
                for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    Tiles[x, y].Type = copy[x, y];
            }

            // 3. Add sand border
            for (int x = 1; x < Width - 1; x++)
            for (int y = 1; y < Height - 1; y++)
            {
                if (Tiles[x, y].Type == TileType.Grass)
                {
                    bool nearWater = false;
                    for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                        if (Tiles[x + dx, y + dy].Type == TileType.Water)
                            nearWater = true;
                    if (nearWater)
                        Tiles[x, y].Type = TileType.Sand;
                }
            }

            // 4. Add river (random start and end on land)
            var riverPath = new List<(int x, int y)>();
            int rx = rand.Next(1, Width - 2);
            int ry = 0;
            for (int i = 0; i < Height; i++)
            {
                if (Tiles[rx, ry].Type == TileType.Grass || Tiles[rx, ry].Type == TileType.Sand)
                {
                    Tiles[rx, ry].Type = TileType.River;
                    riverPath.Add((rx, ry));
                }
                ry++;
                if (rand.NextDouble() < 0.4 && rx > 1) rx--;
                else if (rand.NextDouble() < 0.4 && rx < Width - 2) rx++;
                if (ry >= Height) break;
            }

            // 5. Add small lake (3x3) on land, not on river
            int attempts = 0;
            while (attempts < 20)
            {
                int lx = rand.Next(1, Width - 4);
                int ly = rand.Next(1, Height - 4);
                bool canPlace = true;
                for (int x = lx; x < lx + 3; x++)
                for (int y = ly; y < ly + 3; y++)
                    if (Tiles[x, y].Type != TileType.Grass)
                        canPlace = false;
                if (canPlace)
                {
                    for (int x = lx; x < lx + 3; x++)
                    for (int y = ly; y < ly + 3; y++)
                        Tiles[x, y].Type = TileType.Lake;
                    break;
                }
                attempts++;
            }

            // Count land and water
            LandCount = 0;
            WaterCount = 0;
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                if (Tiles[x, y].Type == TileType.Grass || Tiles[x, y].Type == TileType.Sand || Tiles[x, y].Type == TileType.Lake || Tiles[x, y].Type == TileType.River)
                    LandCount++;
                else
                    WaterCount++;
            }
        }
    }
}