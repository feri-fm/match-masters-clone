using MMC.Network;

namespace MMC.Game
{
    public class BasePanel : Panel
    {
        public GameManager game => GameManager.instance;
        public NetNetworkManager networkManager => NetNetworkManager.instance;

        private bool dirty;

        public override void Setup()
        {
            base.Setup();
            game.onStateChanged += MarkDirty;
        }

        public override void OnOpen()
        {
            base.OnOpen();
            OnRender();
        }

        public virtual void OnRender() { }

        public void MarkDirty()
        {
            dirty = true;
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            if (dirty)
            {
                dirty = false;
                OnRender();
            }
        }
    }
}