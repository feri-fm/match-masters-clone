using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using ImUI;
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

        public Member<Blocker> blocker;

        public Gameplay gameplay { get; private set; }

        private bool dirty;

        protected virtual void Setup() { }
        protected virtual void Render() { }

        protected virtual void Start()
        {
            blocker.value.gameObject.SetActive(false);
        }

        public void Setup(Gameplay gameplay)
        {
            this.gameplay = gameplay;

            gameplay.view = this;

            gameplay.onNewEngine += (engine) =>
            {
                engine.waiter = Wait;
                engineView.Setup(engine);
            };

            gameplay.onChanged += () => dirty = true;

            GameManager.instance.cameraController.SetRig(cameraRig);

            Setup();
        }

        public Task<Tile> PromptTile()
        {
            var tcs = new TaskCompletionSource<Tile>();

            blocker.value.gameObject.SetActive(true);
            blocker.value.onClick.AddListener(Use);

            void Use()
            {
                blocker.value.onClick.RemoveListener(Use);
                blocker.value.gameObject.SetActive(false);

                var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var point = engineView.GetPoint(position);
                var tile = gameplay.game.ValidatePoint(point) ? gameplay.game.GetTileAt(point) : null;

                tcs.SetResult(tile);
            }

            return tcs.Task;
        }

        protected virtual void LateUpdate()
        {
            if (dirty)
            {
                dirty = false;
                Render();
            }
        }

        public void ForceRender()
        {
            Render();
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

        public GameplayView view;

        public Match3.Game game => gameEntity.game;

        public event Action<Int2, Int2> onTrySwap = delegate { };
        public event Action<Int2, Int2> onSwapSucceed = delegate { };
        public event Action<Int2, Int2> onSwapFailed = delegate { };
        public event Action<Tile> onRewardMatch = delegate { };
        public event Action<Tile> onTileHit = delegate { };
        public event Action<GameplayReader> onTryUseBooster = delegate { };
        public event Action<int, GameplayReader> onTryUsePerk = delegate { };

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
            gameEntity.game.onTrySwap += (a, b) => { onTrySwap.Invoke(a.position, b.position); Changed(); };
            gameEntity.game.onSwapSucceed += (a, b) => { onSwapSucceed.Invoke(a.position, b.position); Changed(); };
            gameEntity.game.onSwapFailed += (a, b) => { onSwapFailed.Invoke(a.position, b.position); Changed(); };
            gameEntity.game.onTileHit += (e) => { onTileHit.Invoke(e); Changed(); };
            gameEntity.game.onRewardMatch += (e) => { onRewardMatch.Invoke(e); Changed(); };
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
                Changed();
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

        public Task<Tile> PromptTile()
        {
            if (view != null) return view.PromptTile();
            return Task.FromResult<Tile>(null);
        }

        public Task<bool> TrySwap(Int2 a, Int2 b)
        {
            return gameEntity.game.TrySwap(a, b, true);
        }

        public async Task UseBooster(Booster booster, GameplayReader reader, bool withoutNotify = false)
        {
            if (!withoutNotify)
                onTryUseBooster.Invoke(reader);
            await booster.Apply(this, reader);
        }

        public async Task UsePerk(int index, Perk perk, GameplayReader reader, bool withoutNotify = false)
        {
            if (!withoutNotify)
                onTryUsePerk.Invoke(index, reader);
            await perk.Apply(this, reader);
        }

        public string GetChecksum()
        {
            var data = Save();
            var checksum = "";
            checksum += $"[{data.engine.identifier.lastId}]";
            checksum += $"[{data.data.ToJson()}]";
            var gameData = data.engine.entities.FirstOrDefault(e => e.key == "game");
            if (gameData != null)
            {
                checksum += $"[{gameData.data.ToJson()}]";
            }
            // foreach (var entity in data.engine.entities)
            // {
            //     // if (tile is ColoredTile coloredTile)
            //     //     checksum += $"[{tile.id},{tile.key},{coloredTile.color.value}]";
            //     // else
            //     //     checksum += $"[{tile.id},{tile.key}]";
            //     checksum += $"[{entity.id},{entity.key}]";
            // }
            return checksum;
        }
        public string GetHash()
        {
            var checksum = GetChecksum();
            var hash = Hash128.Compute(checksum);
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

    public class GameplayReader : JsonData
    {
        public string Save() { return json.ToJson(); }

        public GameplayReader() : base() { }
        public GameplayReader(JObject json) : base(json) { }

        public static GameplayReader From(string json)
        {
            return new GameplayReader(JObject.Parse(json));
        }
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

        public bool CanUserBooster()
        {
            return boosterScore >= booster.requiredScore;
        }

        public bool CanUsePerk(int index)
        {
            return !usedPerks[index];
        }

        public async Task UseBooster(GameplayReader reader, bool withoutNotify = false)
        {
            if (isTurn && !gameplay.gameEntity.isEvaluating && CanUserBooster())
            {
                if (reader == null)
                {
                    reader = new GameplayReader();
                    var res = await booster.WriteReader(gameplay, reader);
                    if (!res) return;
                }

                await gameplay.UseBooster(booster, reader, withoutNotify);
                boosterScore = 0;
                gameplay.Changed();
            }
        }

        public async Task UsePerk(int index, GameplayReader reader, bool withoutNotify = false)
        {
            if (isTurn && !gameplay.gameEntity.isEvaluating && CanUsePerk(index))
            {
                if (reader == null)
                {
                    reader = new GameplayReader();
                    var res = await perks[index].WriteReader(gameplay, reader);
                    if (!res) return;
                }

                var task = gameplay.UsePerk(index, perks[index], reader, withoutNotify);
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