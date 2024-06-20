using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Unity.Mathematics;
using UnityEngine;

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

        public bool isEvaluating { get; private set; }

        public System.Random random { get; private set; }

        public Int2 swappedA;
        public Int2 swappedB;

        public int hittings;

        public void Setup(Engine engine, GameConfig config, GameOptions options)
        {
            this.engine = engine;
            this.config = config;
            this.options = options;
            random = new(options.seed);
            engine.onEntityRemoved += OnEntityRemoved;
            BuildGrid();
        }

        public void Clear()
        {
            engine.onEntityRemoved -= OnEntityRemoved;
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

        public void OnEntityRemoved(Entity entity)
        {
            if (entity is Tile tile)
            {
                SetTileAt(tile.position, null);
            }
        }

        public async Task<bool> Evaluate()
        {
            isEvaluating = true;
            var changed = false;
            var loop = false;
            do
            {
                loop = false;
                hittings = 0;
                foreach (var tile in tiles)
                {
                    if (tile != null)
                        tile.canHit = true;
                }
                while (TryGetMatch(out var match))
                {
                    await ApplyMatch(match);
                    loop = true;
                }
                while (hittings > 0)
                    await engine.Wait(0);

                if (loop) await engine.Wait(0.4f);
                while (CanApplyGravity())
                {
                    ApplyGravity();
                    FillTopRow();
                    await engine.Wait(0.1f);
                    loop = true;
                }
                ApplyGravity();
                if (loop) changed = true;
                if (loop) await engine.Wait(0.2f);
            } while (loop);
            isEvaluating = false;
            return changed;
        }

        public void TrySwap(Tile a, Tile b) => TrySwap(a.position, b.position);
        public async void TrySwap(Int2 a, Int2 b)
        {
            isEvaluating = true;
            swappedA = a;
            swappedB = b;
            Swap(a, b);
            await engine.Wait(0.4f);
            if (!await Evaluate())
            {
                Swap(a, b);
            }
        }

        public bool ValidatePoint(Int2 p) => p.x >= 0 && p.x < width && p.y >= 0 && p.y < height;
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
            for (int p = 0; p < config.match.patterns.Length; p++)
            {
                var pattern = config.match.patterns[p];
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
        private bool CheckMatch(Int2 offset, MatchPattern pattern, out Match match)
        {
            var color = TileColor.none;
            match = new Match(pattern, offset);
            for (int i = 0; i < pattern.points.Length; i++)
            {
                var point = offset + pattern.points[i];
                var tile = GetTileAt(point);
                if (tile != null && tile.canHit && tile is ColoredTile coloredTile)
                {
                    match.SetTileAt(i, coloredTile);
                    if (i == 0) color = coloredTile.color;
                    else if (coloredTile.color != color)
                        return false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private async Task ApplyMatch(Match match)
        {
            foreach (var tile in match.tiles.Where(e => e is BeadTile))
                await tile.Hit();
            foreach (var tile in match.tiles.Where(e => e is not BeadTile))
                await tile.Hit();

            if (match.pattern.hasReward)
            {
                var hadReward = false;
                Int2 rewardPoint = Int2.zero;
                foreach (var item in match.pattern.rewardPoints)
                {
                    if (item + match.offset == swappedA || item + match.offset == swappedB)
                    {
                        rewardPoint = item;
                        hadReward = true;
                        break;
                    }
                }
                if (!hadReward)
                    rewardPoint = match.pattern.rewardPoints.Random();

                var point = match.offset + rewardPoint;
                TileView prefab = null;
                switch (match.pattern.reward)
                {
                    case MatchPattern.Reward.Horizontal: prefab = config.GetRewardTile<ClearLineTileView>(match.color, e => e.direction == ClearLineDirection.Horizontal); break;
                    case MatchPattern.Reward.Vertical: prefab = config.GetRewardTile<ClearLineTileView>(match.color, e => e.direction == ClearLineDirection.Vertical); break;
                    case MatchPattern.Reward.Lightning: prefab = config.GetRewardTile<LightningTileView>(match.color); break;
                    case MatchPattern.Reward.Bomb: prefab = config.GetRewardTile<BombTileView>(match.color); break;
                }
                var tile = CreateTileFromView(prefab);
                SetTileAt(point, tile);
                tile.WithTrait<AnimatorTrait>(t => t.Spawn());
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
            {
                // var isHole = false;
                var last = false;
                for (int j = 0; j < height; j++)
                {
                    if (last)
                    {
                        last = false;
                        continue;
                    }
                    var cell = new Int2(i, j);
                    var downCell = new Int2(i, j - 1);
                    var tile = GetTileAt(cell);
                    var moved = false;
                    if (j > 0 && IsNotEmptyAt(cell) && IsEmptyAt(downCell))
                    {
                        last = true;
                        moved = true;
                        Swap(cell, downCell);
                    }
                    if (tile != null)
                    {
                        if (moved)
                        {
                            tile.WithTrait<AnimatorTrait>(t => t.Stretch());
                        }
                        else
                        {
                            tile.WithTrait<AnimatorTrait>(t => t.Squash());
                        }
                    }
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
                    var bead = CreateColoredTile(config.beads.Random());
                    SetTileAt(cell, bead);
                    bead.WithTrait<AnimatorTrait>(t => t.SpawnAtTop());
                }
            }
        }

        public T CreateColoredTile<T>(ColoredTileView<T> view) where T : ColoredTile => CreateTileFromView(view) as T;
        public T CreateTile<T>(TileView<T> view) where T : Tile => CreateTileFromView(view) as T;
        public Tile CreateTileFromView(TileView view)
        {
            var tile = engine.CreateEntity(view) as Tile;
            tile.SetupGame(this);
            return tile;
        }

        public async Task Shuffle()
        {
            for (int i = 0; i < 20; i++)
            {
                var a = new Int2(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
                var b = new Int2(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, height));
                Swap(a, b);
            }
            await engine.Wait(0.6f);
        }
    }

    public class Match
    {
        public MatchPattern pattern;
        public Int2 offset;
        public Int2[] points;
        public ColoredTile[] tiles;

        public TileColor color => tiles[0].color;

        public Match(MatchPattern pattern, Int2 offset)
        {
            this.pattern = pattern;
            this.offset = offset;
            points = new Int2[pattern.points.Length];
            tiles = new ColoredTile[pattern.points.Length];
        }

        public void SetTileAt(int index, ColoredTile tile)
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