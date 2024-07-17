using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MMC.EngineCore;
using UnityEngine;

namespace MMC.Match3
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

        public Func<TileColor, TileColor> colorMap = (c) => c;

        public Dictionary<TileColor, int> colorsCount = new();

        public event Action<Tile> onTileHit = delegate { };
        public event Action<Tile, Tile> onTrySwap = delegate { };
        public event Action<Tile, Tile> onSwapFailed = delegate { };
        public event Action<Tile, Tile> onSwapSucceed = delegate { };
        public event Action<Tile> onRewardMatch = delegate { };
        public event Action onEvaluatingFinished = delegate { };

        public Int2 swappedA;
        public Int2 swappedB;

        public int hittings;

        private bool removed;
        private bool isCancelSwap;

        public Guid id { get; }

        public Game()
        {
            id = Guid.NewGuid();
        }

        public override string ToString()
        {
            return base.ToString() + " " + id;
        }

        public void Setup(GameEntity entity, GameOptions options)
        {
            this.entity = entity;
            this.options = options;
            random = new();
            random.SetSeed(options.seed);
            random.SetMax(100);
            engine.onEntityRemoved += OnEntityRemoved;
            tiles = new Tile[width, height];
            removed = false;
            ResetColors();
        }

        public void Remove()
        {
            engine.onEntityRemoved -= OnEntityRemoved;
            removed = true;
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
                if (GetTileAt(tile.position) == tile)
                    SetTileAt(tile.position, null);
            }
        }

        private bool _isInEvaluate;
        public async Task<bool> Evaluate()
        {
            if (_isInEvaluate)
            {
                throw new Exception("Game is already evaluating!");
            }
            _isInEvaluate = true;
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
                        await Wait(0);
                        loop = true;
                    }
                }

                while (hittings > 0)
                {
                    await Wait(0);
                }

                if (loop) await Wait(0.2f);

                while (CanApplyGravity())
                {
                    ApplyGravity();
                    await Wait(0.02f);
                    FillTopRow();
                    await Wait(0.02f);
                    loop = true;
                }
                ApplyGravity();
                if (loop) changed = true;
                if (loop) await Wait(0.3f);
            } while (loop);
            isEvaluating = false;
            _isInEvaluate = false;
            onEvaluatingFinished.Invoke();
            return changed;
        }

        public void CancelSwap()
        {
            isCancelSwap = true;
        }

        public Task<bool> TrySwap(Tile a, Tile b) => TrySwap(a.position, b.position);
        public async Task<bool> TrySwap(Int2 a, Int2 b, bool withoutNotify = false)
        {
            if (isEvaluating) return false;

            if (!ValidatePoint(a) || !ValidatePoint(b)) return false;

            if (IsEmptyAt(a) || IsEmptyAt(b)) return false;

            var delta = a - b;
            if (delta.x < 0) delta.x = -delta.x;
            if (delta.y < 0) delta.y = -delta.y;
            if ((delta.x != 0 || delta.y != 1) && (delta.x != 1 || delta.y != 0))
                return false;

            isEvaluating = true;
            swappedA = a;
            swappedB = b;
            var tileA = GetTileAt(a);
            var tileB = GetTileAt(b);

            isCancelSwap = false;
            if (!withoutNotify)
                onTrySwap.Invoke(tileA, tileB);

            if (isCancelSwap) return false;

            if (tileA.HasTrait<PowerUpTrait>() && tileB.HasTrait<PowerUpTrait>())
            {
                hittings = 0;

                if (tileA is ClearLineTile lineA && tileB is ClearLineTile lineB
                    && lineA.prefab.direction == lineB.prefab.direction)
                {
                    var prefab = config.GetRewardTile<ClearLineTileView>(lineA.color, e => e.direction != lineA.prefab.direction);
                    engine.RemoveEntity(tileA);
                    tileA = CreateTileFromView(prefab);
                    SetTileAt(a, tileA);
                    tileA.WithTrait<AnimatorTrait>(t => t.Jump());
                    tileA.canHit = true;
                }

                SetTileAt(a, null);
                SetTileAt(b, tileA);

                tileA.WithTrait<AnimatorTrait>(t => t.MoveTo());
                await Wait(0.3f);
                _ = tileA.Hit();
                _ = tileB.Hit();
                while (hittings > 0)
                    await Wait(0);
                await Evaluate();
            }
            else
            {
                Swap(a, b);
                tileA.WithTrait<AnimatorTrait>(t => t.MoveTo());
                tileB.WithTrait<AnimatorTrait>(t => t.MoveTo());
                await Wait(0.2f);
                if (!await Evaluate())
                {
                    Swap(a, b);
                    tileA.WithTrait<AnimatorTrait>(t => t.MoveTo());
                    tileB.WithTrait<AnimatorTrait>(t => t.MoveTo());
                    await Wait(0.15f);
                    isEvaluating = false;
                    onSwapFailed.Invoke(tileA, tileB);
                    return false;
                }
            }

            onSwapSucceed.Invoke(tileA, tileB);

            return true;
        }

        public async Task Wait(float time)
        {
            await engine.Wait(time);
            if (removed) throw new Exception("Game removed!");
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

        public bool AnyMatch()
        {
            return ScanMatch(config.match.search, out _);
        }
        public bool AnyMove()
        {
            return ScanMatch(config.match.moves, out _);
        }
        private bool TryGetMatch(out Match match)
        {
            return ScanMatch(config.match.patterns, out match);
        }
        public bool ScanMatch(MatchPattern[] patterns, out Match match)
        {
            match = null;
            for (int p = 0; p < patterns.Length; p++)
            {
                var pattern = patterns[p];
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
                if (IsNotEmptyAt(point))
                {
                    // throw new Exception("Reward point is not available");
                    // engine.RemoveEntity(GetTileAt(point));
                }
                var tile = CreateTileFromView(prefab);
                SetTileAt(point, tile);
                tile.WithTrait<AnimatorTrait>(t => t.Spawn());
                onRewardMatch.Invoke(tile);
                await Wait(0.3f);
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
                    var prefab = GenerateBead(point);
                    var bead = CreateColoredTile(prefab);
                    SetTileAt(point, bead);
                    bead.WithTrait<AnimatorTrait>(t => t.SpawnAtTop());

                    colorsCount[MapColor(bead.color)] += 1;
                }
            }
        }

        public void OnTileHit(Tile tile)
        {
            onTileHit.Invoke(tile);
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
                    colorsCount[MapColor(coloredTile.color)] += 1;
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
            if (down != null) badColors.Add(MapColor(down.prefab.color));
            if (left != null) badColors.Add(MapColor(left.prefab.color));
            if (right != null) badColors.Add(MapColor(right.prefab.color));

            for (int i = 0; i < options.beads; i++)
            {
                var bead = config.beads[i];
                all.Add(bead);
                var count = colorsCount[bead.color];
                if (count < options.minBeads)
                {
                    should.Add(bead);
                    if (!badColors.Contains(bead.color))
                        safeShould.Add(bead);
                }
                else if (count < options.maxBeads)
                {
                    could.Add(bead);
                    if (!badColors.Contains(bead.color))
                        safeCould.Add(bead);
                }
            }

            if (safeShould.Count > 0) return MapColor(RandElement(safeShould));
            if (safeCould.Count > 0) return MapColor(RandElement(safeCould));
            if (should.Count > 0) return MapColor(RandElement(should));
            if (could.Count > 0) return MapColor(RandElement(could));
            return MapColor(RandElement(all));
        }

        public T CreateColoredTile<T>(ColoredTileView<T> view) where T : ColoredTile => CreateTileFromView(view) as T;
        public T CreateTile<T>(TileView<T> view) where T : Tile => CreateTileFromView(view) as T;
        public Tile CreateTileFromView(TileView view)
        {
            var tile = engine.CreateEntity(view) as Tile;
            tile.SetupGame(this);
            return tile;
        }

        public T MapColor<T>(T prefab) where T : TileView
        {
            return MapColor(prefab as TileView) as T;
        }
        public TileView MapColor(TileView prefab)
        {
            if (prefab is ColoredTileView coloredTile) return MapColor(coloredTile);
            return prefab;
        }
        public ColoredTileView MapColor(ColoredTileView prefab)
        {
            var newColor = MapColor(prefab.color);
            if (newColor == prefab.color) return prefab;
            return config.coloredTiles.FirstOrDefault(e => e.color == newColor && e.GetType() == prefab.GetType()) ?? prefab;
        }
        public TileColor MapColor(TileColor color)
        {
            return colorMap.Invoke(color);
        }

        public async Task RunCommand(GameCommand command)
        {
            isEvaluating = true;
            await command.Instantiate().Run(this);
            while (hittings > 0)
                await Wait(0);
            await Evaluate();
        }

        public GameData Save()
        {
            var ids = new int[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    ids[i, j] = tiles[i, j]?.id.value ?? -1;
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
                    var tile = engine.entities.Find(e => e is Tile tile && tile.position == new Int2(i, j)) as Tile;
                    if (tile != null)
                    {
                        tile.WithTrait<AnimatorTrait>(t => t.Jump());
                        tile.SetupGame(this);
                        tiles[i, j] = tile;
                    }
                }
            }
        }

        public void ApplyColorMap()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var tile = engine.entities.Find(e => e is Tile tile && tile.position == new Int2(i, j)) as Tile;
                    if (tile != null)
                    {
                        var newPrefab = MapColor(tile.prefab as TileView);
                        if (newPrefab != tile.prefab)
                        {
                            var tileData = tile.Save();
                            engine.RemoveEntity(tile);
                            var newTile = engine.CreateEntity(newPrefab, tile.id) as Tile;
                            newTile.Load(tileData);
                            newTile.PostLoad(tileData);
                            tile = newTile;
                        }
                        tile.WithTrait<AnimatorTrait>(t => t.Jump());
                        tile.SetupGame(this);
                        tiles[i, j] = tile;
                    }
                }
            }
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
        public int minBeads = 6;
        public int maxBeads = 10;
        public int beads = 6;
    }

    public class GameData
    {
        public GameOptions options;
        public RandomTableData random;
        public int[,] tiles;
    }
}