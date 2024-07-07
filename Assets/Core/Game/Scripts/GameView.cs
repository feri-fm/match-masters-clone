using System;
using System.Collections;
using System.Threading.Tasks;
using MMC.EngineCore;
using MMC.Match3;
using UnityEngine;

namespace MMC.Game
{
    public class GameplayView : PoolObject
    {
        public EngineView engineView;

        public Gameplay gameplay;

        public void Setup(Gameplay gameplay)
        {
            this.gameplay = gameplay;

            gameplay.onNewEngine += (engine) =>
            {
                engine.waiter = Wait;
                engineView.Setup(engine);
            };
        }

        public Task Wait(float time)
        {
            var tcs = new TaskCompletionSource<byte>();
            StartCoroutine(IWait(time, () =>
            {
                tcs.SetResult(0);
            }));
            return tcs.Task;

            IEnumerator IWait(float time, Action callback)
            {
                yield return new WaitForSeconds(time);
                callback.Invoke();
            }
        }
    }

    public class Gameplay
    {
        public EngineConfig engineConfig;
        public Engine engine;
        public GameEntity gameEntity;
        public GameOptions gameOptions;

        public event Action<Engine> onNewEngine = delegate { };

        public void Setup()
        {
            engine = new Engine(engineConfig);
            gameEntity = engine.CreateEntity("game") as GameEntity;
            var options = gameOptions.JsonCopy();
            options.seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            gameEntity.Setup(options);

            onNewEngine.Invoke(engine);

            engine.Evaluate();
        }

        public void Load(GameplayData data)
        {
            if (engine != null)
                engine.Clear();

            engine = new Engine(engineConfig);
            engine.Load(data.engine);
            gameEntity = engine.GetEntity<GameEntity>();

            onNewEngine.Invoke(engine);

            engine.Evaluate();
        }
        public GameplayData Save()
        {
            return new GameplayData()
            {
                engine = engine.Save(),
            };
        }
    }

    public class GameplayData
    {
        public EngineData engine;
        public JsonData data;
    }
}