using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using MMC.EngineCore;
using MMC.Match3;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace MMC.Game
{
    public class GameplayView : MonoBehaviour
    {
        public ShuffleCommand shuffle;
        public EngineConfig engineConfig;
        public EngineView engineView;
        public CameraRig cameraRig;

        public Gameplay gameplay { get; private set; }

        private bool dirty;

        protected virtual void Setup() { }
        protected virtual void Render() { }

        public void Setup(Gameplay gameplay)
        {
            this.gameplay = gameplay;

            gameplay.onNewEngine += (engine) =>
            {
                engine.waiter = Wait;
                engineView.Setup(engine);
            };

            gameplay.onChanged += () => dirty = true;

            GameManager.instance.cameraController.SetRig(cameraRig);

            Setup();
        }

        protected virtual void LateUpdate()
        {
            if (dirty)
            {
                dirty = false;
                Render();
            }
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
        public event Action<Tile> onTileHit = delegate { };
        public event Action onTryUseBooster = delegate { };
        public event Action<int> onTryUsePerk = delegate { };

        public event Action<Engine> onNewEngine = delegate { };
        public event Action onEvaluatingFinished = delegate { };

        public event Action onChanged = delegate { };

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
            gameEntity.game.onTrySwap += (a, b) => onTrySwap.Invoke(a.position, b.position);
            gameEntity.game.onSwapSucceed += (a, b) => onSwapSucceed.Invoke(a.position, b.position);
            gameEntity.game.onSwapFailed += (a, b) => onSwapFailed.Invoke(a.position, b.position);
            gameEntity.game.onTileHit += (e) => onTileHit.Invoke(e);
            gameEntity.game.onRewardMatch += (e) => onRewardMatch.Invoke(e);
            gameEntity.game.onEvaluatingFinished += () =>
            {
                if (!gameEntity.game.AnyMove())
                {
                    _ = gameEntity.game.RunCommand(prefab.shuffle);
                }
                else
                {
                    onEvaluatingFinished.Invoke();
                }
            };
        }

        public void Changed()
        {
            onChanged.Invoke();
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
            Changed();
        }

        public Task<bool> TrySwap(Int2 a, Int2 b)
        {
            return gameEntity.game.TrySwap(a, b, true);
        }

        public async Task UseBooster(Booster booster, bool withoutNotify = false)
        {
            if (!withoutNotify)
                onTryUseBooster.Invoke();
            await booster.Use(this);
        }

        public async Task UsePerk(int index, Perk perk, bool withoutNotify = false)
        {
            if (!withoutNotify)
                onTryUsePerk.Invoke(index);
            await perk.Use(this);
        }

        public string GetHash()
        {
            var data = Save();
            var hash = Hash128.Compute(data.ToJson());
            return hash.ToString();
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

            Changed();
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

    public abstract class GameplayPlayer
    {
        public int score;
        public int boosterScore;
        public bool[] usedPerks;

        public Gameplay gameplay;
        public Booster booster;
        public Perk[] perks;

        public abstract bool isMyPlayer { get; }
        public abstract bool isTurn { get; }
        public abstract int totalRounds { get; }
        public abstract int totalMoves { get; }
        public abstract int round { get; }
        public abstract int moves { get; }

        public void Setup(Gameplay gameplay, Booster booster, Perk[] perks)
        {
            this.gameplay = gameplay;
            this.booster = booster;
            this.perks = perks;
            usedPerks = new bool[perks.Length];
        }

        public async Task UseBooster(bool withoutNotify = false)
        {
            if (isTurn && !gameplay.gameEntity.isEvaluating && boosterScore >= booster.requiredScore)
            {
                await gameplay.UseBooster(booster, withoutNotify);
                boosterScore = 0;
                gameplay.Changed();
            }
        }

        public async Task UsePerk(int index, bool withoutNotify = false)
        {
            if (isTurn && !gameplay.gameEntity.isEvaluating && !usedPerks[index])
            {
                var task = gameplay.UsePerk(index, perks[index], withoutNotify);
                usedPerks[index] = true;
                gameplay.Changed();
                await task;
                gameplay.Changed();
            }
        }

        public virtual void Save(JsonData data) { }
        public virtual void Load(JsonData data) { }

        public JsonData _Save()
        {
            var data = new JsonData();
            data.W("s", score);
            data.W("b", boosterScore);
            data.W("p", usedPerks);
            Save(data);
            return data;
        }
        public void _Load(JsonData data)
        {
            score = data.R<int>("s");
            boosterScore = data.R<int>("b");
            usedPerks = data.R<bool[]>("p");
            Load(data);
        }
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