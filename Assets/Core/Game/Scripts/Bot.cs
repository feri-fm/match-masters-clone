using MMC.EngineCore;
using MMC.Match3;
using UnityEngine;

namespace MMC.Game
{
    public abstract class Bot : MonoBehaviour
    {
        public Gameplay gameplay { get; private set; }
        public bool isOpponent { get; private set; }
        public GameplayPlayer player { get; private set; }

        public Match3.Game game => gameplay.gameEntity.game;

        public BotAction Play(Gameplay gameplay, bool isOpponent, GameplayPlayer player)
        {
            this.gameplay = gameplay;
            this.isOpponent = isOpponent;
            this.player = player;
            var action = Think();
            return action;
        }

        protected abstract BotAction Think();
    }

    public abstract class BotAction { }

    public class SwapAction : BotAction
    {
        public Int2 a;
        public Int2 b;
        public Tile tileA;
        public Tile tileB;

        public SwapAction(Int2 a, Int2 b)
        {
            this.a = a;
            this.b = b;
        }
        public SwapAction(Tile a, Tile b)
        {
            tileA = a;
            tileB = b;
            this.a = a.position;
            this.b = b.position;
        }
    }

    public class UseBoosterAction : BotAction
    {
        public GameplayReader reader = new();
    }
    public class UsePerkAction : BotAction
    {
        public int index;
        public GameplayReader reader = new();

        public UsePerkAction(int index)
        {
            this.index = index;
        }
    }
}