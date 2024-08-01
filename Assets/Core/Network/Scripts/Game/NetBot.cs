using System;
using MMC.EngineCore;
using MMC.Game;
using MMC.Match3;
using UnityEngine;

namespace MMC.Network.GameMiddleware
{
    public class NetBot : MonoBehaviour
    {
        public float waitTime = 1.5f;
        public Bot bot;

        public NetPlayer player { get; private set; }
        public NetGame netGame => player.game;
        public TwoPlayerGameplay gameplay => netGame.gameplay;
        public Match3.Game game => netGame.gameplay.gameEntity.game;

        public void Setup(NetPlayer player)
        {
            this.player = player;
            transform.parent = player.transform;
            lastSwap = Time.time + waitTime;
        }

        private float lastSwap;
        private void Update()
        {
            if (!gameplay.isFinished && gameplay.IsTurn(player.index))
            {
                if (Time.time > lastSwap + waitTime && Time.time > netGame.lastEvaluateTime + waitTime)
                {
                    lastSwap = Time.time;
                    Think();
                }
            }
            else
            {
                lastSwap = Time.time;
            }
        }

        private void Think()
        {
            // var w = player.netGame.gameplay.gameOptions.width;
            // var h = player.netGame.gameplay.gameOptions.height;
            // var start = new Int2(Random.Range(0, w - 1), Random.Range(0, h - 1));
            // var dir = Random.value > 0.5f ? Int2.right : Int2.up;
            // var a = start;
            // var b = start + dir;
            // player.TrySwap(a, b);

            // if (game.ScanMatch(goodMoves.GetPatterns(), out var match)
            //     || game.ScanMatch(game.config.match.moves.GetPatterns(), out match))
            // {
            //     var offset = match.offset;
            //     var a = offset + match.pattern.rewardPoints[0];
            //     var b = offset + match.pattern.rewardPoints[1];
            //     player.TrySwap(a, b);
            // }

            var action = bot.Play(gameplay, gameplay.isOpponent, gameplay.GetPlayer(player.index));
            if (action is SwapAction swap)
            {
                player.TrySwap(swap.a, swap.b);
            }
            else if (action is UseBoosterAction useBooster)
            {
                player.UseBooster(useBooster.reader.Save());
            }
            else if (action is UsePerkAction usePerk)
            {
                player.UsePerk(usePerk.index, usePerk.reader.Save());
            }
            else
            {
                if (game.ScanMatch(game.config.match.moves.GetPatterns(), ScanDirection.Default, TileColor.none, _ => true, out var match))
                {
                    var offset = match.offset;
                    var a = offset + match.pattern.rewardPoints[0];
                    var b = offset + match.pattern.rewardPoints[1];
                    player.TrySwap(a, b);
                }
            }
        }
    }
}