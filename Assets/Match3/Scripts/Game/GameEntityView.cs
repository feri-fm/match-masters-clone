
using System;
using UnityEngine;
using Core;

namespace Match3
{
    public class GameEntityView : EntityView<GameEntity>
    {
        public override Entity CreateEntity() => new GameEntity();
    }

    public class GameEntity : Entity<GameEntityView>
    {
        public Game game { get; private set; }

        protected override void OnSetup()
        {
            base.OnSetup();
            evaluable.RegisterCallback(0, Evaluate);
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            game.Clear();
        }

        public void Setup(GameConfig config, GameOptions options)
        {
            game = new Game();
            game.Setup(engine, config, options);
        }

        public async void Evaluate()
        {
            await game.Evaluate();
        }
    }
}
