using System;
using System.Collections;
using System.Threading.Tasks;
using Core;
using ImUI;
using Match3;
using UnityEngine;

namespace DebugRoom
{
    public class GameSection : DebugSection
    {
        public Camera cam;
        public Transform cameraAnchor;
        public TextAdaptor gameText;

        public EngineView engineView;
        public EngineConfig engineConfig;
        public GameOptions gameOptions;

        public Engine engine;
        public GameEntity gameEntity;
        public Game game => gameEntity.game;

        public event Action<Engine> onEngineCreated = delegate { };

        private bool isEvaluating;

        private bool overlay;

        protected override void OnUI()
        {
            ui.Label(game.isEvaluating ? "Evaluating..." : "Stable");
            overlay = ui.Toggle("Overlay", overlay);

            ui.BeginHorizontal(50);
            ui.StashIndent(0);
            gameOptions.seed = ui.Number("Seed", gameOptions.seed, new VPLayoutFlexibleWidth(100));
            if (ui.Button("Rnd", new VPLayoutMinWidth(70)))
            {
                gameOptions.seed = UnityEngine.Random.Range(10000, 99999);
            }
            ui.LoadIndent();
            ui.EndLayout();
            gameOptions.width = ui.Slider("Width", gameOptions.width, 2, 16);
            gameOptions.height = ui.Slider("Height", gameOptions.height, 2, 16);
            gameOptions.beads = ui.Slider("Beads", gameOptions.beads, 2, 6);
            gameOptions.minBeads = ui.Slider("Min Bead", gameOptions.minBeads, 0, 20);
            gameOptions.maxBeads = ui.Slider("Max Bead", gameOptions.maxBeads, 0, 30);
            if (ui.Button("Reload"))
            {
                ReloadGame();
            }
        }

        private void Start()
        {
            ReloadGame();
        }

        private void Update()
        {
            cameraAnchor.position = engineView.GetPosition(new Vector2(game.width - 1, game.height - 1) / 2f);

            var normalizedPosition = Mathf.Clamp(manager.bottomDragHandle.normalizedPosition, 0, 0.9f);

            var ratio = Screen.height * (1 - normalizedPosition) / Screen.width;
            if (ratio < 1) ratio = 1;
            var widthSize = game.width / 2f * ratio * 1.1f;
            var heightSize = game.height / 2f * 1.1f;
            var size = Mathf.Max(widthSize, heightSize);

            size /= 1 - normalizedPosition;

            cameraAnchor.position += Vector3.down * normalizedPosition * size;
            cam.orthographicSize = size;

            if (isEvaluating != game.isEvaluating)
            {
                isEvaluating = game.isEvaluating;
                imUI.Changed();
            }

            gameText.gameObject.SetActive(overlay);
            gameText.text = game.isEvaluating ? "Evaluating..." : "Stable";
        }

        public void ReloadGame()
        {
            if (engine != null)
                engine.Clear();

            engine = new Engine(engineConfig);
            engine.waiter = manager.GetSection<WaiterSection>().Wait;
            engineView.Setup(engine);

            gameEntity = engine.CreateEntity("game") as GameEntity;
            gameEntity.Setup(gameOptions.JsonCopy());

            engine.Evaluate();

            manager.GetSection<StatsSection>().ResetData();
            onEngineCreated.Invoke(engine);
        }

        public EngineData Save()
        {
            return engine.Save();
        }

        public void Load(EngineData engineData)
        {
            if (engine != null)
                engine.Clear();

            engine = new Engine(engineConfig);
            engine.waiter = manager.GetSection<WaiterSection>().Wait;
            engineView.Setup(engine);

            engine.Load(engineData);
            gameEntity = engine.GetEntity<GameEntity>();
            gameOptions = game.options.JsonCopy();

            engine.Evaluate();

            onEngineCreated.Invoke(engine);
        }
    }
}