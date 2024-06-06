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

        public void Setup(EngineConfig config)
        {
            this.config = config;
        }

        public void Clear() { }

        public Tile CreateTile(TileView tile) => CreateTile(tile.key);
        public Tile CreateTile(string key) => null;
        public Tile CreateTile(string key, Id id) => null;
        public void RemoveTile(Tile tile) { }

        public void Evaluate() => evaluator.Evaluate(tiles);
    }
}
