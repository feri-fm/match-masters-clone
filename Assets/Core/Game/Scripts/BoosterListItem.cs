using MMC.Server.Models;

namespace MMC.Game
{
    public class BoosterListItem : ListItem<Booster>
    {
        public ImageMember icon;
        public TextMember count;
        public TextMember key;
        public GameObjectMember locked;
        public GameObjectMember unlocked;
        public GameObjectMember selected;
        public GameObjectMember notSelected;

        private GameManager game => GameManager.instance;

        protected override void Setup()
        {
            base.Setup();
            icon.sprite = data.icon;
            var itemCount = game.user.inventory.GetCount(data.key);
            count.text = itemCount.ToString();
            locked.SetActive(!game.user.inventory.HasItem(data.key));
            unlocked.SetActive(!locked.activeSelf);
            selected.SetActive(game.user.IsSelected(data.key));
            notSelected.SetActive(!selected.activeSelf);
            key.text = data.key;
        }

        [Member]
        public void Select()
        {
            game.ChangeState(() =>
            {
                game.user.SelectBooster(game.config, data.key);
                game.networkManager.menu.UpdateSelectedItems(game.user.selectedItems.ToArray());
            });
        }

        [Member]
        public void Unlock()
        {
            game.ChangeState(() =>
            {
                game.user.inventory.AddItem(data.key);
                game.networkManager.menu.UnlockBooster(data);
            });
        }

        [Member]
        public void Add()
        {
            game.ChangeState(() =>
            {
                game.user.inventory.ChangeCount(data.key, 1);
                var count = game.user.inventory.GetCount(data.key);
                game.networkManager.menu.SetItemCount(data.key, count);
            });
        }

        [Member]
        public void Play()
        {
            game.Join();
        }
    }
}