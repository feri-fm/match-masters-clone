using System.Collections.Generic;
using Match3;
using UnityEngine;

namespace DebugRoom
{
    public class StatsSection : DebugSection
    {
        public TextAdaptor scoreText;

        private Dictionary<TileColor, int> colorsCount = new();
        private int score;
        private bool overlay;

        private GameSection gameSection;

        protected override void Setup()
        {
            base.Setup();
            ResetData();
            gameSection = manager.GetSection<GameSection>();

            gameSection.onEngineCreated += (engine) =>
            {
                var gameEntity = engine.GetEntity<GameEntity>();
                gameEntity.game.onTileHit += (tile) =>
                {
                    if (tile is ColoredTile coloredTile)
                    {
                        if (!colorsCount.ContainsKey(coloredTile.color))
                            colorsCount[coloredTile.color] = 1;
                        else
                            colorsCount[coloredTile.color] += 1;
                        score++;
                        imUI.Changed();
                    }
                };
            };
        }

        protected override void OnUI()
        {
            if (ui.Button("Reset"))
            {
                ResetData();
            }

            overlay = ui.Toggle("Overlay", overlay);

            ui.disabled = true;
            ui.Text("Score", score.ToString());
            foreach (var row in colorsCount)
            {
                ui.Text(row.Key.ToString(), row.Value.ToString());
            }
            ui.disabled = false;
        }

        private void LateUpdate()
        {
            scoreText.gameObject.SetActive(overlay);
            scoreText.text = $"Score: {score}";
        }

        public void ResetData()
        {
            score = 0;
            colorsCount[TileColor.blue] = 0;
            colorsCount[TileColor.red] = 0;
            colorsCount[TileColor.green] = 0;
            colorsCount[TileColor.yellow] = 0;
            colorsCount[TileColor.orange] = 0;
            colorsCount[TileColor.purple] = 0;
        }

        public StatsData Save()
        {
            var data = new StatsData();
            data.score = score;
            data.colorsCount = new();
            foreach (var row in colorsCount)
            {
                data.colorsCount[row.Key.value] = row.Value;
            }
            return data;
        }
        public void Load(StatsData data)
        {
            ResetData();
            score = data.score;
            foreach (var row in data.colorsCount)
            {
                colorsCount[new TileColor(row.Key)] = row.Value;
            }
        }
    }

    public class StatsData
    {
        public int score;
        public Dictionary<int, int> colorsCount;
    }
}