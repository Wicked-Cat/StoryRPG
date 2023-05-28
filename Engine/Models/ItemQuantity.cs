using Engine.Factories;
namespace Engine.Models
{
    public class ItemQuantity : BaseNotificationClass
    {
        private Item _baseItem;
        private int _quantity;
        public Item BaseItem
        {
            get { return _baseItem; }
            set
            {
                _baseItem = value;
                OnPropertyChanged(nameof(BaseItem));
            }
        }
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value; 
                OnPropertyChanged(nameof(Quantity));
            }
        }
        public string QuantityItemDescription => $"{Quantity} {BaseItem.Name}";

        public ItemQuantity(Item baseItem, int quantity) 
        {
            BaseItem = baseItem; 
            Quantity = quantity;
        }
    }
}
