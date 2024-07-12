using MMC.Match3;
using UnityEngine;
using UnityEngine.UI;

namespace MMC.Game
{
    public class TwoPlayerGameplayView : GameplayView<TwoPlayerGameplay>
    {
        public int maxRounds = 5;
        public int maxMoves = 2;

        public GameObjectMember myTurn;
        public GameObjectMember opponentTurn;
        public TextMember status;

        private void Update()
        {
            status.text = $"Index: {(gameplay.isOpponent ? 1 : 0)}";
            status.text += $"\nTurn: {gameplay.turn}";
            status.text += $"\nMoves: {gameplay.moves}";
            status.text += $"\nRound: {gameplay.round}";

            myTurn.SetActive(gameplay.IsMyTurn());
            opponentTurn.SetActive(!gameplay.IsMyTurn());
        }
    }

    public class TwoPlayerGameplay : Gameplay<TwoPlayerGameplayView>
    {
        public int round;
        public int moves;
        public int turn;

        private bool earnedExtraMove;

        public bool isOpponent { get; private set; }

        public override void Setup()
        {
            base.Setup();
            round = 0;
            moves = prefab.maxMoves;
            turn = 0;

            onSwapSucceed += (a, b) =>
            {
                EndMove();
            };
            onSwapFailed += (a, b) =>
            {
                FailedMove();
            };
            onRewardMatch += (tile) =>
            {
                ExtraMove();
            };
        }

        protected override void SetupEngine()
        {
            base.SetupEngine();
            SetIsOpponent(isOpponent);
        }

        public void SetIsOpponent(bool value)
        {
            isOpponent = value;
            if (isOpponent)
                gameEntity.game.colorMap = e => e == TileColor.blue ? TileColor.red : (e == TileColor.red ? TileColor.blue : e);
            else
                gameEntity.game.colorMap = e => e;
        }

        public bool IsMyTurn()
        {
            return isOpponent ? IsTurn(1) : IsTurn(0);
        }
        public bool IsTurn(int index)
        {
            return index == turn;
        }
        public void StartMove()
        {
            moves -= 1;
            earnedExtraMove = false;
        }
        public void ExtraMove()
        {
            if (!earnedExtraMove)
            {
                earnedExtraMove = true;
                moves += 1;
            }
        }
        public void FailedMove()
        {
            moves += 1;
        }
        public void EndMove()
        {
            if (moves <= 0)
            {
                turn += 1;
                moves = prefab.maxMoves;
                if (turn >= 2)
                {
                    turn = 0;
                    round += 1;

                    if (round >= prefab.maxRounds)
                    {
                        //TODO: finish game here
                    }
                }
            }
        }
    }
}