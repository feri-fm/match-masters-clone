using System;
using System.Collections.Generic;
using MMC.EngineCore;
using MMC.Match3;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MMC.Game
{
    public class TwoPlayerGameplayView : GameplayView<TwoPlayerGameplay>
    {
        public int totalRounds = 5;
        public int totalMoves = 2;
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
        public GameplayPlayerView myPlayer;
        public GameplayPlayerView opponentPlayer;

        private List<GameplayMessage> messages = new();

        private float showMessageTime;
        private float setHandTime;
        private Id handTileId;
        private float messageAlpha;

        private void Start()
        {
            messageAlpha = messageColor.value.color.a;
        }

        protected override void Setup()
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

        protected override void Render()
        {
            base.Render();
            if (gameplay.myPlayer != null) myPlayer.Render(gameplay.myPlayer);
            if (gameplay.opponentPlayer != null) opponentPlayer.Render(gameplay.opponentPlayer);
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
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

        public TwoPlayerGameplayPlayer myPlayer;
        public TwoPlayerGameplayPlayer opponentPlayer;

        public event Action<GameplayMessage> onMessage = delegate { };
        public event Action onFinish = delegate { };

        public bool isFinished { get; private set; }

        public bool isOpponent { get; private set; }

        public override void Setup()
        {
            base.Setup();
            round = 0;
            moves = prefab.totalMoves;
            turn = 0;

            myPlayer = new TwoPlayerGameplayPlayer();
            opponentPlayer = new TwoPlayerGameplayPlayer();

            myPlayer.index = 0;
            opponentPlayer.index = 1;

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
                if (IsMyTurn())
                {
                    myPlayer.score += 1;
                    if (tile is ColoredTile colored && colored.color == TileColor.blue)
                        myPlayer.boosterScore = Mathf.Min(myPlayer.boosterScore + 1, myPlayer.booster.requiredScore);
                }
                else
                {
                    opponentPlayer.score += 1;
                    if (tile is ColoredTile colored && colored.color == TileColor.red)
                        opponentPlayer.boosterScore = Mathf.Min(opponentPlayer.boosterScore + 1, opponentPlayer.booster.requiredScore);
                }
                Changed();
            };

            Changed();
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
            if (isOpponent)
                gameEntity.game.ApplyColorMap();
        }

        public void SetIsOpponent(bool value)
        {
            isOpponent = value;
            if (isOpponent)
                gameEntity.game.colorMap = e => e == TileColor.blue ? TileColor.red : (e == TileColor.red ? TileColor.blue : e);
            else
                gameEntity.game.colorMap = e => e;

            if (myPlayer != null) myPlayer.index = MyIndex();
            if (opponentPlayer != null) opponentPlayer.index = OpponentIndex();
        }

        public TwoPlayerGameplayPlayer GetCurrentPlayer()
        {
            if (turn == MyIndex()) return myPlayer;
            else return opponentPlayer;
        }
        public int MyIndex()
        {
            return isOpponent ? 1 : 0;
        }
        public int OpponentIndex()
        {
            return isOpponent ? 0 : 1;
        }
        public bool IsMyTurn()
        {
            return IsTurn(MyIndex());
        }
        public bool IsTurn(int index)
        {
            return index == turn;
        }
        public void StartMove()
        {
            moves -= 1;
            earnedExtraMove = false;
            Changed();
        }
        public void ExtraMove()
        {
            if (!earnedExtraMove)
            {
                earnedExtraMove = true;
                moves += 1;
                ShowMessage("Extra Move!", IsMyTurn() ? GameplayColor.My : GameplayColor.Opponent);
                Changed();
            }
        }
        public void FailedMove()
        {
            moves += 1;
            Changed();
        }
        public void EndMove()
        {
            if (moves <= 0)
            {
                turn += 1;
                moves = prefab.totalMoves;
                if (turn >= 2)
                {
                    turn = 0;
                    round += 1;

                    if (round >= prefab.totalRounds)
                    {
                        //TODO: finish game here
                        isFinished = true;
                        onFinish.Invoke();
                        // ShowMessage("Game finished", GameplayColor.Natural);
                        Changed();
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
            Changed();
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
            data.W("isFinished", isFinished);

            var p0 = isOpponent ? opponentPlayer : myPlayer;
            var p1 = isOpponent ? myPlayer : opponentPlayer;
            data.W("p0", p0._Save().json);
            data.W("p1", p1._Save().json);
        }
        public override void Load(JsonData data)
        {
            base.Load(data);
            round = data.R<int>("round");
            moves = data.R<int>("moves");
            turn = data.R<int>("turn");
            earnedExtraMove = data.R<bool>("earnedExtraMove");
            isFinished = data.R<bool>("isFinished");

            var p0 = isOpponent ? opponentPlayer : myPlayer;
            var p1 = isOpponent ? myPlayer : opponentPlayer;
            p0._Load(new JsonData(data.R<JObject>("p0")));
            p1._Load(new JsonData(data.R<JObject>("p1")));
        }
    }

    public class TwoPlayerGameplayPlayer : GameplayPlayer
    {
        public int index;

        public new TwoPlayerGameplay gameplay => base.gameplay as TwoPlayerGameplay;

        public override bool isMyPlayer => gameplay.IsMyTurn();
        public override bool isTurn => gameplay.IsTurn(index);
        public override int moves => gameplay.moves;
        public override int round => gameplay.round;
        public override int totalMoves => gameplay.prefab.totalMoves;
        public override int totalRounds => gameplay.prefab.totalRounds;
    }
}