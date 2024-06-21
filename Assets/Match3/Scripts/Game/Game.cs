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

        public int width => options.width;
        public int height => options.height;

        public GameOptions options { get; private set; }
        public GameConfig config => entity.prefab.config;
        public GameEntity entity { get; private set; }
        public Engine engine => entity.engine;

        public bool isEvaluating { get; private set; }

        public RandomTable random { get; private set; }

        public Dictionary<TileColor, int> colorsCount = new();

        public Int2 swappedA;
        public Int2 swappedB;

        public int hittings;

        public void Setup(GameEntity entity, GameOptions options)
        {
            this.entity = entity;
            this.options = options;
            random = new();
            random.SetSeed(options.seed);
            random.SetMax(100);
            engine.onEntityRemoved += OnEntityRemoved;
            tiles = new Tile[width, height];
            ResetColors();
        }

        public void Remove()
        {
            engine.onEntityRemoved -= OnEntityRemoved;
        }

        public void BuildGrid()
        {
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
            bool loop;
            do
            {
                loop = false;
                hittings = 0;
                foreach (var tile in tiles)
                {
                    if (tile != null)
                        tile.canHit = true;
                }
                if (AnyMatch())
                {
                    while (TryGetMatch(out var match))
                    {
                        await ApplyMatch(match);
                        loop = true;
                    }
                }

                while (hittings > 0)
                {
                    await engine.Wait(0);
                }

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
                if (loop) await engine.Wait(0.3f);
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
                isEvaluating = true;

                var tileA = GetTileAt(a);
                var tileB = GetTileAt(b);

                if (tileA.HasTrait<PowerUpTrait>() && tileB.HasTrait<PowerUpTrait>())
                {
                    hittings = 0;
                    _ = tileA.Hit();
                    _ = tileB.Hit();
                    while (hittings > 0)
                        await engine.Wait(0);
                    await Evaluate();
                }
                else
                {
                    Swap(a, b);
                    await engine.Wait(0.3f);
                    isEvaluating = false;
                }
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

        public int RandInt(int max)
        {
            return random.Next().value % max;
        }
        public Int2 RandPoint()
        {
            return new Int2(RandInt(width), RandInt(height));
        }
        public T RandElement<T>(List<T> items) => RandElement(items.ToArray());
        public T RandElement<T>(T[] items)
        {
            var index = RandInt(items.Length);
            return items[index];
        }

        public void Swap(Int2 a, Int2 b)
        {
            var temp = GetTileAt(a);
            SetTileAt(a, GetTileAt(b));
            SetTileAt(b, temp);
        }

        private bool AnyMatch()
        {
            for (int p = 0; p < config.match.search.Length; p++)
            {
                var pattern = config.match.search[p];
                for (int i = 0; i <= width - pattern.width; i++)
                {
                    for (int j = 0; j <= height - pattern.height; j++)
                    {
                        var point = new Int2(i, j);
                        if (CheckMatch(point, pattern, out _))
                            return true;
                    }
                }
            }
            return false;
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
                var rewardPoint = match.pattern.rewardPoints[0];
                if (match.pattern.rewardPoints.Length > 1)
                {
                    foreach (var item in match.pattern.rewardPoints)
                    {
                        if (item + match.offset == swappedA || item + match.offset == swappedB)
                        {
                            rewardPoint = item;
                            hadReward = true;
                            swappedA = -Int2.one;
                            swappedB = -Int2.one;
                            break;
                        }
                    }
                    if (!hadReward)
                    {
                        rewardPoint = RandElement(match.pattern.rewardPoints);
                    }
                }

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
            CountColors();
            for (int i = 0; i < width; i++)
            {
                var point = new Int2(i, height - 1);
                if (IsEmptyAt(point))
                {
                    var bead = CreateColoredTile(GenerateBead(point));
                    SetTileAt(point, bead);
                    bead.WithTrait<AnimatorTrait>(t => t.SpawnAtTop());

                    colorsCount[bead.color] += 1;
                }
            }
        }
        private void ResetColors()
        {
            colorsCount[TileColor.blue] = 0;
            colorsCount[TileColor.red] = 0;
            colorsCount[TileColor.green] = 0;
            colorsCount[TileColor.yellow] = 0;
            colorsCount[TileColor.orange] = 0;
            colorsCount[TileColor.purple] = 0;
        }
        private void CountColors()
        {
            ResetColors();
            foreach (var tile in tiles)
            {
                if (tile != null && tile is ColoredTile coloredTile)
                {
                    colorsCount[coloredTile.color] += 1;
                }
            }
        }
        private BeadTileView GenerateBead(Int2 point)
        {
            var all = new List<BeadTileView>();
            var should = new List<BeadTileView>();
            var could = new List<BeadTileView>();
            var safeShould = new List<BeadTileView>();
            var safeCould = new List<BeadTileView>();

            var badColors = new List<TileColor>();
            var down = ValidatePoint(point + Int2.down) ? GetTileAt(point + Int2.down) as BeadTile : null;
            var left = ValidatePoint(point + Int2.left) ? GetTileAt(point + Int2.left) as BeadTile : null;
            var right = ValidatePoint(point + Int2.right) ? GetTileAt(point + Int2.right) as BeadTile : null;
            if (down != null) badColors.Add(down.prefab.color);
            if (left != null) badColors.Add(left.prefab.color);
            if (right != null) badColors.Add(right.prefab.color);

            for (int i = 0; i < options.beads; i++)
            {
                var bead = config.beads[i];
                all.Add(bead);
                var count = colorsCount[bead.color];
                if (count < options.minCount)
                {
                    should.Add(bead);
                    if (!badColors.Contains(bead.color))
                        safeShould.Add(bead);
                }
                else if (count < options.maxCount)
                {
                    could.Add(bead);
                    if (!badColors.Contains(bead.color))
                        safeCould.Add(bead);
                }
            }

            if (safeShould.Count > 0) return RandElement(safeShould);
            if (safeCould.Count > 0) return RandElement(safeCould);
            if (should.Count > 0) return RandElement(should);
            if (could.Count > 0) return RandElement(could);
            return RandElement(all);
        }

        public T CreateColoredTile<T>(ColoredTileView<T> view) where T : ColoredTile => CreateTileFromView(view) as T;
        public T CreateTile<T>(TileView<T> view) where T : Tile => CreateTileFromView(view) as T;
        public Tile CreateTileFromView(TileView view)
        {
            var tile = engine.CreateEntity(view) as Tile;
            tile.SetupGame(this);
            return tile;
        }

        public async Task RunProgram(Func<Game, Task> action)
        {
            isEvaluating = true;
            await action.Invoke(this);
            while (hittings > 0)
                await engine.Wait(0);
            await Evaluate();
        }

        public async Task Shuffle()
        {
            await RunProgram(async (g) =>
            {
                for (int i = 0; i < 20; i++)
                {
                    var a = RandPoint();
                    var b = RandPoint();
                    if (g.IsNotEmptyAt(a) && g.IsNotEmptyAt(b))
                    {
                        g.Swap(a, b);
                        if (g.AnyMatch())
                            g.Swap(a, b);
                    }
                }
                await g.engine.Wait(0.6f);
            });
        }
        public async Task TwoColors()
        {
            await RunProgram(async (g) =>
            {
                foreach (var tile in tiles)
                {
                    engine.RemoveEntity(tile);
                }
                var s1 = RandInt(options.beads);
                var s2 = (s1 + 1 + RandInt(options.beads - 2)) % options.beads;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        var black = (i % 2 == 0 && j % 2 == 1) || (i % 2 == 1 && j % 2 == 0);
                        var bead = black ? config.beads[s1] : config.beads[s2];
                        var tile = CreateColoredTile(bead);
                        var point = new Int2(i, j);
                        SetTileAt(point, tile);
                        tile.WithTrait<AnimatorTrait>(t => t.Jump());
                    }
                }
                await engine.Wait(0.1f);
            });
        }
        public async Task Duck()
        {
            await RunProgram(async (g) =>
            {
                for (int i = 0; i < g.width; i++)
                {
                    var tile = GetTileAt(new Int2(i, 3));
                    if (tile != null)
                    {
                        _ = tile.Hit();
                        await g.engine.Wait(0.05f);
                    }
                }
                await engine.Wait(0.2f);
            });
        }
        public async Task Rocket()
        {
            await RunProgram(async (g) =>
            {
                for (int i = 0; i < 5; i++)
                {
                    var tile = GetTileAt(g.RandPoint());
                    if (tile != null)
                    {
                        _ = tile.Hit();
                        await g.engine.Wait(0.2f);
                    }
                }
                await engine.Wait(0.3f);
            });
        }
        public async Task Bucket()
        {
            await RunProgram(async (g) =>
            {
                var color = config.beads[2 + g.RandInt(options.beads - 2)].color;
                var prefab = g.config.GetBeadTile(color);
                for (int i = 0; i < 6; i++)
                {
                    var point = g.RandPoint();
                    var tile = GetTileAt(point);
                    if (tile != null && tile is BeadTile beadTile && beadTile.color != color)
                    {
                        g.engine.RemoveEntity(beadTile);
                        var newTile = g.CreateColoredTile(prefab);
                        SetTileAt(point, newTile);
                        newTile.WithTrait<AnimatorTrait>(t => t.Spawn());
                        await g.engine.Wait(0.1f);
                    }
                }
                await engine.Wait(0.4f);
            });
        }
        public async Task Hat()
        {
            await RunProgram(async (g) =>
            {
                for (int i = 0; i < 6; i++)
                {
                    var point = g.RandPoint();
                    var tile = GetTileAt(point);
                    if (tile != null && tile is BeadTile beadTile)
                    {
                        var color = beadTile.color;
                        var prefab = g.RandElement(g.config.rewardTiles.Where(e => e.color == color).ToArray());
                        g.engine.RemoveEntity(beadTile);
                        var newTile = g.CreateTile(prefab);
                        SetTileAt(point, newTile);
                        newTile.WithTrait<AnimatorTrait>(t => t.Spawn());
                        await g.engine.Wait(0.1f);
                    }
                }
                await engine.Wait(0.4f);
            });
        }

        public GameData Save()
        {
            var ids = new Id[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    ids[i, j] = tiles[i, j]?.id ?? Id.empty;
                }
            }
            return new GameData()
            {
                options = options,
                random = random.Save(),
                tiles = ids,
            };
        }
        public void Load(GameData data)
        {
            options = data.options;
            random.Load(data.random);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tiles[i, j] = engine.entities.Find(e => e is Tile tile && tile.position == new Int2(i, j)) as Tile;
                    if (tiles[i, j] != null)
                    {
                        tiles[i, j].WithTrait<AnimatorTrait>(t => t.Jump());
                        tiles[i, j].SetupGame(this);
                    }
                }
            }
            _ = Evaluate();
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

    [Serializable]
    public class GameOptions
    {
        public int seed = 12345;
        public int width = 7;
        public int height = 7;
        public int minCount = 6;
        public int maxCount = 10;
        public int beads = 6;
    }

    public class GameData
    {
        public GameOptions options;
        public RandomTableData random;
        public Id[,] tiles;
    }
}