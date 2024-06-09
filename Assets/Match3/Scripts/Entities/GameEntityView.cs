
using UnityEngine;

namespace Match3
{
    public class GameEntityView : EntityView<GameEntity>
    {
        public int width = 7;
        public int height = 7;
        public CellEntityView cell;
        public BeadTileView[] beads;

        public override Entity CreateEntity() => new GameEntity();
    }

    public class GameEntity : Entity<GameEntityView>
    {
        public Tile[,] tiles;

        public int width => prefab.width;
        public int height => prefab.height;

        protected override void OnSetup()
        {
            base.OnSetup();
            BuildGrid();
            evaluable.RegisterCallback(0, Evaluate);
        }

        public void BuildGrid()
        {
            tiles = new Tile[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var cell = engine.CreateEntity(prefab.cell) as CellEntity;
                    cell.position = new Int2(i, j);
                    cell.Changed();
                }
            }
        }

        public void Evaluate()
        {
            DoEvaluate();
        }
        public bool DoEvaluate()
        {
            var changed = false;
            var loop = false;
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
            return false;
        }

        private void ApplyMatch(Match match) { }

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
                    var bead = engine.CreateEntity(prefab.beads.Random()) as BeadTile;
                    SetTileAt(cell, bead);
                }
            }
        }
    }

    public class Match { }
}
