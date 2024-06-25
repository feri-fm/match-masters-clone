using MMC.Core;
using UnityEngine;

namespace MMC.Match3
{
    public class SwappableTileTraitView : TraitView<SwappableTileTrait>
    {
        public float minDelta = 20;
        public override Trait CreateTrait() => new SwappableTileTrait();

        private Vector3 startPos;
        private bool isDown;

        private void OnMouseDown()
        {
            if (!UIFilter.IsPointerClear())
            {
                startPos = Input.mousePosition;
                isDown = true;
            }
        }
        private void OnMouseUp()
        {
            if (isDown)
            {
                isDown = false;
                var game = engine.engine.GetEntity<GameEntity>().game;
                var tileView = entity as TileView;
                var tile = tileView.tile;

                if (game.isEvaluating) return;

                var delta = Input.mousePosition - startPos;
                if (delta.magnitude > 0)
                {
                    if (Mathf.Abs(Mathf.Abs(delta.x) - Mathf.Abs(delta.y)) > minDelta)
                    {
                        var dir = Vector2Int.zero;
                        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                            dir.x = (int)Mathf.Sign(delta.x);
                        else
                            dir.y = (int)Mathf.Sign(delta.y);

                        var otherPoint = tile.position + (Int2)dir;
                        if (game.ValidatePoint(otherPoint))
                        {
                            var otherTile = game.GetTileAt(otherPoint);
                            if (otherTile != null)
                            {
                                game.TrySwap(tile, otherTile);
                            }
                        }
                    }
                }
            }
        }
    }

    public class SwappableTileTrait : Trait
    {

    }
}
