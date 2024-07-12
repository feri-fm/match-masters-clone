using System;
using System.Collections;
using System.Threading.Tasks;
using MMC.EngineCore;
using MMC.Match3;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace MMC.Game
{
    public class GameplayView : MonoBehaviour
    {
        public EngineConfig engineConfig;
        public EngineView engineView;

        public Gameplay gameplay { get; private set; }

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
        public Engine engine;
        public GameEntity gameEntity;
        public GameOptions gameOptions;

        public GameplayView prefab { get; private set; }

        public EngineConfig engineConfig => prefab.engineConfig;

        public event Action<Int2, Int2> onTrySwap = delegate { };
        public event Action<Int2, Int2> onSwapSucceed = delegate { };
        public event Action<Int2, Int2> onSwapFailed = delegate { };
        public event Action<Tile> onRewardMatch = delegate { };
        public event Action<Engine> onNewEngine = delegate { };

        public virtual void Setup() { }

        public void Setup(GameplayView prefab, GameOptions gameOptions)
        {
            this.prefab = prefab;
            this.gameOptions = gameOptions;

            engine = new Engine(engineConfig);
            gameEntity = engine.CreateEntity("game") as GameEntity;
            gameEntity.Setup(gameOptions);
            SetupEngine();
            onNewEngine.Invoke(engine);

            Setup();
        }

        protected virtual void SetupEngine()
        {
            gameEntity.game.onTrySwap += (a, b) =>
            {
                onTrySwap.Invoke(a.position, b.position);
            };
            gameEntity.game.onSwapSucceed += (a, b) => onSwapSucceed.Invoke(a.position, b.position);
            gameEntity.game.onSwapFailed += (a, b) => onSwapFailed.Invoke(a.position, b.position);
            gameEntity.game.onRewardMatch += (a) => onRewardMatch.Invoke(a);
        }

        public Gameplay GetFastGameplay()
        {
            var gameplay = Activator.CreateInstance(GetType()) as Gameplay;
            gameplay.Setup(prefab, gameOptions);
            var data = Save();
            gameplay.Load(data);
            return gameplay;
        }

        public void Evaluate()
        {
            engine.Evaluate();
        }
        public Task<bool> TrySwap(Int2 a, Int2 b)
        {
            return gameEntity.game.TrySwap(a, b, true);
        }

        public Hash128 GetHash()
        {
            var data = Save();
            var hash = Hash128.Compute(data.ToJson());
            return hash;
        }

        public void Load(GameplayData data)
        {
            if (engine != null)
                engine.Clear();

            engine = new Engine(engineConfig);
            onNewEngine.Invoke(engine);
            engine.Load(data.engine);
            gameEntity = engine.GetEntity<GameEntity>();
            gameOptions = gameEntity.game.options;
            SetupEngine();

            var json = new JsonData(data.data);
            json.Load(this);
            Load(json);
        }
        public GameplayData Save()
        {
            var data = new JsonData();
            data.Save(this);
            Save(data);
            return new GameplayData()
            {
                engine = engine.Save(),
                data = data.json,
            };
        }

        public virtual void Save(JsonData data) { }
        public virtual void Load(JsonData data) { }
    }

    public class GameplayData
    {
        public EngineData engine;
        public JObject data;
    }

    public class Gameplay<T> : Gameplay where T : GameplayView
    {
        public new T prefab => base.prefab as T;
    }
    public class GameplayView<T> : GameplayView where T : Gameplay
    {
        public new T gameplay => base.gameplay as T;
    }
}