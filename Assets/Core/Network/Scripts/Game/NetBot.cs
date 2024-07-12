using System;
using MMC.EngineCore;
using MMC.Game;
using MMC.Match3;
using UnityEngine;

namespace MMC.Network.GameMiddleware
{
    public class NetBot : MonoBehaviour
    {
        public ShuffleCommand shuffle;
        public MatchPattern[] patterns;

        public NetPlayer player;
        public NetGame netGame => player.game;
        public TwoPlayerGameplay gameplay => netGame.gameplay;
        public Match3.Game game => netGame.gameplay.gameEntity.game;

        public void Setup(NetPlayer player)
        {
            this.player = player;
            transform.parent = player.transform;
            lastSwap = Time.time + 1;
        }

        private float lastSwap;
        private void Update()
        {
            if (gameplay.IsTurn(player.index))
            {
                if (Time.time > lastSwap + 3)
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

            if (game.ScanMatch(patterns, out var match))
            {
                var offset = match.offset;
                var a = offset + match.pattern.rewardPoints[0];
                var b = offset + match.pattern.rewardPoints[1];
                player.TrySwap(a, b);
            }
            else
            {
                throw new NotImplementedException("shuffle in when no move over network not supported yet");
                // _ = game.RunCommand(shuffle);
            }
        }
    }
}