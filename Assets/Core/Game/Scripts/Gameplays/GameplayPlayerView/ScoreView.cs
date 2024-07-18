using Unity.Burst.Intrinsics;
using UnityEngine;

namespace MMC.Game.GameplayPlayerViews
{
    public class ScoreView : GameplayPlayerViewPart
    {
        public TextMember score;
        public TextMember counterScore;
        public GameObjectMember counter;
        public float addTime = 0.1f;

        private int _counterScore;
        private int _lastScore;
        private int _score;

        private float _lastAddTime;

        private void Update()
        {
            var delta = player.score - _lastScore;

            if (delta != 0 && Time.time > _lastAddTime + addTime)
            {
                var dir = (int)Mathf.Sign(delta);
                _counterScore += dir;
                _lastAddTime = Time.time;
            }

            if (!player.gameplay.gameEntity.isEvaluating && _counterScore != 0
                && Time.time > _lastAddTime + addTime)
            {
                var dir = (int)Mathf.Sign(_counterScore);
                _score += dir;
                _counterScore -= dir;
                _lastAddTime = Time.time;
            }

            _lastScore = _counterScore + _score;

            counter.SetActive(_counterScore != 0);

            counterScore.text = _counterScore.ToString();
            score.text = _score.ToString();
        }
    }
}