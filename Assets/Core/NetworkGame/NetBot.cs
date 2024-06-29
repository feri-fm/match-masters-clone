using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkGame
{
    [RequireComponent(typeof(NetPlayer))]
    public class NetBot : MonoBehaviour
    {
        public NetPlayer player { get; set; }
        public NetGame game { get; set; }
    }

    public class NetBot<TGame, TPlayer> : NetBot where TGame : NetGame where TPlayer : NetPlayer
    {
        public new TGame game => base.game as TGame;
        public new TPlayer player => base.player as TPlayer;
    }
}
