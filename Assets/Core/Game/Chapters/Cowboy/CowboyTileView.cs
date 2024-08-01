using System.Collections.Generic;
using System.Threading.Tasks;
using MMC.EngineCore;
using MMC.Match3;
using UnityEngine;

namespace MMC.Game.Chapters.Cowboy
{
    public class CowboyTileView : ColoredTileView<CowboyTile>
    {
        public int count = 4;
        public LineRenderer lineRenderer;

        protected override void OnRender()
        {
            base.OnRender();
            lineRenderer.positionCount = entity.targets.Count * 2;
            for (int i = 0; i < entity.targets.Count; i++)
            {
                lineRenderer.SetPosition(i * 2, engine.GetPosition(entity.position) + Vector3.back);
                lineRenderer.SetPosition(i * 2 + 1, engine.GetPosition(entity.targets[i]) + Vector3.back);
            }
        }

        public override Entity CreateEntity() => new CowboyTile();
    }

    public class CowboyTile : ColoredTile<CowboyTileView>
    {
        public List<Int2> targets = new();

        protected override void OnSetup()
        {
            base.OnSetup();
            AddTrait<GravityTraitView>();
            AddTrait<SwappableTileTraitView>();
            AddTrait<AnimatorTraitView>();
            AddTrait<ScoreTraitView>();
            AddTrait<MechanicTraitView>();
        }

        protected override async Task OnHit()
        {
            await base.OnHit();
            targets.Clear();
            var targetTiles = new List<Tile>();
            game.ScanRandom(prefab.count, t => t != this && !t.willHit, tile =>
            {
                tile.willHit = true;
                targets.Add(tile.position);
                targetTiles.Add(tile);
            });
            Changed();
            await game.Wait(0.2f);
            var tasks = new List<Task>();
            foreach (var tile in targetTiles)
            {
                tasks.Add(tile.Hit());
            }
            await Task.WhenAll(tasks);
            await game.Wait(0.2f);
            engine.RemoveEntity(this);
        }
    }
}