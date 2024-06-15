using System;
using Core;
using Unity.Mathematics;

namespace Match3
{
    public class Game
    {
        public Tile[,] tiles;

        public int width => config.width;
        public int height => config.height;

        public GameOptions options { get; private set; }
        public GameConfig config { get; private set; }
        public Engine engine { get; private set; }

        public System.Random random { get; private set; }

        public float time;

        public void Setup(Engine engine, GameConfig config, GameOptions options)
        {
            this.engine = engine;
            this.config = config;
            this.options = options;
            random = new(options.seed);
        }

        public void BuildGrid()
        {
            tiles = new Tile[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var cell = engine.CreateEntity(config.cell) as Cell;
                    cell.position = new Int2(i, j);
                    cell.Changed();
                }
            }
        }

        public bool Evaluate()
        {
            var changed = false;
            var loop = false;
            time = 0;
            do
            {
                loop = false;
                while (TryGetMatch(out var match))
                {
                    ApplyMatch(match);
                    loop = true;
                }
                while (CanApplyGravity())
                {
                    ApplyGravity();
                    FillTopRow();
                    time += 1;
                    loop = true;
                }
                if (loop) changed = true;
            } while (loop);

            return changed;
        }

        public bool IsEmptyAt(Int2 p) => tiles[p.x, p.y] == null;
        public bool IsNotEmptyAt(Int2 p) => tiles[p.x, p.y] != null;
        public Tile GetTileAt(Int2 p) => tiles[p.x, p.y];
        public void SetTileAt(Int2 p, Tile tile)
        {
            tiles[p.x, p.y] = tile;
            if (tile != null)
            {
                tile.position = p;
                tile.Changed();
            }
        }

        public void Swap(Int2 a, Int2 b)
        {
            var temp = GetTileAt(a);
            SetTileAt(a, GetTileAt(b));
            SetTileAt(b, temp);
        }

        private bool TryGetMatch(out Match match)
        {
            match = null;
            for (int p = 0; p < config.match.search.Length; p++)
            {
                var pattern = config.match.search[p];
                for (int i = 0; i <= width - pattern.width; i++)
                {
                    for (int j = 0; j <= height - pattern.height; j++)
                    {
                        var point = new Int2(i, j);
                        if (CheckMatch(point, pattern, out match))
                            return true;
                    }
                }
            }
            return false;
        }
        private bool CheckMatch(Int2 start, MatchPattern pattern, out Match match)
        {
            var color = TileColor.none;
            match = new Match(pattern);
            for (int i = 0; i < pattern.points.Length; i++)
            {
                var point = start + pattern.points[i];
                var tile = GetTileAt(point);
                if (tile.colorTrait != null)
                {
                    match.SetTileAt(i, tile);
                    if (i == 0) color = tile.colorTrait.color;
                    else
                    {
                        if (tile.colorTrait.color != color)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void ApplyMatch(Match match)
        {
            foreach (var tile in match.tiles)
            {
                tile.Hit(0);
            }
            if (match.pattern.hasReward)
            {

            }
        }

        private bool CanApplyGravity()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (IsEmptyAt(new Int2(i, j))) return true;
                }
            }
            return false;
        }

        private void ApplyGravity()
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height - 1; j++)
                {
                    var cell = new Int2(i, j);
                    var upCell = new Int2(i, j + 1);
                    if (IsEmptyAt(cell) && IsNotEmptyAt(upCell))
                    {
                        Swap(upCell, cell);
                        engine.ScheduleTask((v) =>
                        {

                        });
                    }
                }
        }

        private void FillTopRow()
        {
            for (int i = 0; i < width; i++)
            {
                var cell = new Int2(i, height - 1);
                if (IsEmptyAt(cell))
                {
                    var bead = engine.CreateEntity(config.beads.Random()) as BeadTile;
                    SetTileAt(cell, bead);
                    engine.ScheduleTask((v) =>
                    {
                        v.CreateView(bead);
                    });
                }
            }
        }
    }

    public class Match
    {
        public MatchPattern pattern;
        public Int2[] points;
        public Tile[] tiles;

        public Match(MatchPattern pattern)
        {
            this.pattern = pattern;
            points = new Int2[pattern.points.Length];
            tiles = new Tile[pattern.points.Length];
        }

        public void SetTileAt(int index, Tile tile)
        {
            points[index] = tile.position;
            tiles[index] = tile;
        }
    }

    public class GameOptions
    {
        public int seed;

        public GameOptions()
        {
            seed = UnityEngine.Random.Range(0, 1000000000);
        }

        public GameOptions(int seed)
        {
            this.seed = seed;
        }
    }
}