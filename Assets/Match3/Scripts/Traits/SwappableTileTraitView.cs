using Core;
using UnityEngine;

namespace Match3
{
    public class SwappableTileTraitView : TraitView<SwappableTileTrait>
    {
        public float minDelta = 20;
        public override Trait CreateTrait() => new SwappableTileTrait();

        private Vector3 startPos;
        private void OnMouseDown()
        {
            startPos = Input.mousePosition;
        }
        private void OnMouseUp()
        {
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

    public class SwappableTileTrait : Trait
    {

    }
}
