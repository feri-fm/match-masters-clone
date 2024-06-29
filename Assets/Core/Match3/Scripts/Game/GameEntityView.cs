
using System;
using UnityEngine;
using MMC.EngineCore;

namespace MMC.Match3
{
    public class GameEntityView : EntityView<GameEntity>
    {
        public GameConfig config;

        public override Entity CreateEntity() => new GameEntity();
    }

    public class GameEntity : Entity<GameEntityView>
    {
        public Game game { get; private set; }

        public bool isEvaluating => game.isEvaluating;

        protected override void OnSetup()
        {
            base.OnSetup();
            game = new Game();
            evaluable.RegisterCallback(0, Evaluate);
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();
            game.Remove();
        }

        public void Setup(GameOptions options)
        {
            game.Setup(this, options);
            game.BuildGrid();
        }

        public async void Evaluate()
        {
            await game.Evaluate();
        }

        public override void Save(JsonData data)
        {
            base.Save(data);
            data.W("game", game.Save());
        }
        public override void PostLoad(JsonData data)
        {
            base.PostLoad(data);
            var gameData = data.R<GameData>("game");
            game.Setup(this, gameData.options);
            game.Load(gameData);
        }
    }
}
