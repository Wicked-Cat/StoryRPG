namespace Engine.Models
{
    public class ItemPercent
    {
        public Item BaseItem { get; set; }
        public int Percent { get; set; }

        public ItemPercent(Item baseItem, int percent) 
        {
            BaseItem = baseItem;
            Percent = percent;
        }
    }
}
