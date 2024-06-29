namespace MMC.DebugRoom
{
    public class DebugPageListItem : ListItem<DebugPage>
    {
        public TextMember title;

        public override void Setup()
        {
            base.Setup();
            title.text = data.title;
        }

        [Member]
        public void Select()
        {
            data.Select();
        }
    }
}