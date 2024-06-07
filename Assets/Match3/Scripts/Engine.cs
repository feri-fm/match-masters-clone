using System;
using System.Collections.Generic;

namespace Match3
{
    public class Engine
    {
        public readonly List<Tile> tiles = new();
        public readonly Dictionary<Id, Tile> tileById = new();

        public EngineConfig config { get; private set; }

        public readonly IdentifierGenerator identifierGenerator = new();
        public readonly Evaluator evaluator = new();

        public Engine(EngineConfig config)
        {
            this.config = config;
        }

        public void Clear()
        {
            foreach (var tile in tiles)
            {
                tile._Remove();
            }
            tiles.Clear();
            identifierGenerator.Clear();
            evaluator.Clear();
        }

        public Tile CreateTile<TView>() where TView : TileView => CreateTile(config.GetTile<TView>());
        public Tile CreateTile(TileView tile) => CreateTile(tile.key);
        public Tile CreateTile(string key)
        {
            var id = identifierGenerator.Generate();
            return CreateTile(key, id);
        }
        public Tile CreateTile(string key, Id id)
        {
            var prefab = config.GetTile(key);
            var tile = prefab.CreateTile();
            tiles.Add(tile);
            tileById.Add(id, tile);
            tile._Setup(this, prefab, id);
            return tile;
        }
        public void RemoveTile(Tile tile)
        {
            tile._Remove();
            tiles.Remove(tile);
            tileById.Remove(tile.id);
        }

        public void Evaluate()
        {
            evaluator.Evaluate(tiles);
        }

        public List<Tile> GetTilesWithTrait<T>() where T : Trait
        {
            var res = new List<Tile>();
            foreach (var tile in tiles)
                if (tile.HasTrait<T>())
                    res.Add(tile);
            return res;
        }

        public void ForTileWithTrait<T>(Action<Tile> action) where T : Trait
        {
            foreach (var tile in tiles)
            {
                if (tile.HasTrait<T>())
                    action.Invoke(tile);
            }
        }
    }
}
