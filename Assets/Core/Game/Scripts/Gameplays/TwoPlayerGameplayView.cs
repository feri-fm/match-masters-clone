using System;
using System.Collections.Generic;
using MMC.EngineCore;
using MMC.Match3;
using UnityEngine;
using UnityEngine.UI;

namespace MMC.Game
{
    public class TwoPlayerGameplayView : GameplayView<TwoPlayerGameplay>
    {
        public int maxRounds = 5;
        public int maxMoves = 2;
        public float messageTime = 1;
        public float handTime = 0.4f;
        public Color myColor;
        public Color opponentColor;
        public Color naturalColor;
        public AnimationCurve messageScale;

        public Member<Transform> messageBody;
        public TextMember messageText;
        public Member<Image> messageColor;
        public Member<Transform> hand;
        public GameObjectMember myTurn;
        public GameObjectMember opponentTurn;
        public GameObjectMember finished;
        public TextMember status;

        private List<GameplayMessage> messages = new();

        private float showMessageTime;
        private float setHandTime;
        private Id handTileId;
        private float messageAlpha;

        private void Start()
        {
            messageAlpha = messageColor.value.color.a;
        }

        public override void Setup()
        {
            base.Setup();
            finished.SetActive(false);
            gameplay.onMessage += QueueMessage;

            gameplay.onFinish += () =>
            {
                finished.SetActive(true);
                GameManager.instance.finishGamePanel.OpenPanel();
            };
        }

        private void LateUpdate()
        {
            status.text = $"Index: {(gameplay.isOpponent ? 1 : 0)}";
            status.text += $"\tTurn: {gameplay.turn}";
            status.text += $"\nMoves: {gameplay.moves}";
            status.text += $"\tRound: {gameplay.round}";
            status.text += $"\n<b><color=blue>Score:</color> {gameplay.myScore}</b>";
            status.text += $"\t<b><color=red>Score:</color> {gameplay.opponentScore}</b>";

            myTurn.SetActive(gameplay.IsMyTurn());
            opponentTurn.SetActive(!gameplay.IsMyTurn());

            var t = Time.time - showMessageTime;
            messageBody.value.transform.localScale = new Vector3(1, messageScale.Evaluate(t), 1);

            if (messages.Count > 0 && t >= messageTime)
            {
                var message = messages[0];
                messages.RemoveAt(0);
                ShowMessage(message);
            }

            if (Time.time < setHandTime + handTime)
            {
                hand.value.gameObject.SetActive(true);
                var tile = engineView.GetViewById(handTileId);
                if (tile != null)
                {
                    hand.value.position = tile.GetTrait<AnimatorTraitView>().body.position;
                }
            }
            else
            {
                hand.value.gameObject.SetActive(false);
            }
        }

        public void QueueMessage(GameplayMessage message)
        {
            messages.Add(message);
        }

        public void ShowMessage(GameplayMessage message)
        {
            showMessageTime = Time.time;
            messageText.text = message.text;
            var realColor = naturalColor;
            if (message.color == GameplayColor.My) realColor = myColor;
            if (message.color == GameplayColor.Opponent) realColor = opponentColor;
            var color = new Color(realColor.r, realColor.g, realColor.b, messageAlpha);
            messageColor.value.color = color;
        }

        public void SetHandAt(Id tileId)
        {
            handTileId = tileId;
            setHandTime = Time.time;
        }
    }

    public class GameplayMessage
    {
        public string text;
        public GameplayColor color;
    }

    public enum GameplayColor
    {
        My, Opponent, Natural
    }

    public class TwoPlayerGameplay : Gameplay<TwoPlayerGameplayView>
    {
        public int round;
        public int moves;
        public int turn;
        public bool earnedExtraMove;

        public int myScore;
        public int opponentScore;

        public event Action<GameplayMessage> onMessage = delegate { };
        public event Action onFinish = delegate { };

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
            onTileHit += (tile) =>
            {
                if (IsMyTurn()) myScore += 1;
                else opponentScore += 1;
            };
        }

        public void ShowStartMessage()
        {
            ShowMessage("Round 1", GameplayColor.Natural);
            if (isOpponent)
                ShowMessage("Opponent turn", GameplayColor.Opponent);
            else
                ShowMessage("Your turn", GameplayColor.My);
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
                ShowMessage("Extra Move!", IsMyTurn() ? GameplayColor.My : GameplayColor.Opponent);
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
                        onFinish.Invoke();
                        // ShowMessage("Game finished", GameplayColor.Natural);
                        return;
                    }
                    else
                    {
                        ShowMessage("Round " + (round + 1), GameplayColor.Natural);
                    }
                }

                if (IsMyTurn())
                    ShowMessage("Your turn", GameplayColor.My);
                else
                    ShowMessage("Opponent turn", GameplayColor.Opponent);
            }
        }

        public void ShowMessage(string text, GameplayColor color)
        {
            onMessage.Invoke(new GameplayMessage() { text = text, color = color });
        }

        public override void Save(JsonData data)
        {
            base.Save(data);
            data.W("round", round);
            data.W("moves", moves);
            data.W("turn", turn);
            data.W("earnedExtraMove", earnedExtraMove);
        }
        public override void Load(JsonData data)
        {
            base.Load(data);
            round = data.R<int>("round");
            moves = data.R<int>("moves");
            turn = data.R<int>("turn");
            earnedExtraMove = data.R<bool>("earnedExtraMove");
        }
    }
}