using Engine.Factories;

namespace Engine.Models
{
    public class Merchant : LivingEntity
    {
        public int ID { get; }

        public readonly List<Item.ItemProperties> PreferredItems = new List<Item.ItemProperties>();
        public readonly List<Item.ItemProperties> DislikedItems = new List<Item.ItemProperties>();
        public int Markup { get; set; }

        public readonly List<MerchantStock> _sellList = new List<MerchantStock>();
        public Merchant(int id, string name, string ancestry, string description, int markup) : base(name, ancestry, "Merchant", 10, 10, description, 10, 0,
            1, 1, 1, 1, 1, 1, 1, 1, 1)
        {
            ID = id;
            Markup = markup;

        }

        public void AddItemToSellList(int id, int percentage, int quantity)
        {
            _sellList.RemoveAll(ml => ml.ID == id);
            _sellList.Add(new MerchantStock(id, percentage, quantity));
        }
        public void AddItemToPreferredItems(Item.ItemProperties property)
        {
            PreferredItems.RemoveAll(p => p == property);
            PreferredItems.Add(property);
        }
        public void AddItemToDislikedItems(Item.ItemProperties property)
        {
            DislikedItems.RemoveAll(p => p == property);
            DislikedItems.Add(property);
        }

        public void RefreshStock(Merchant merchant)
        {
            merchant.Inventory.Clear();

            foreach (MerchantStock stock in merchant._sellList.ToList())
            {
                merchant.AddItemToSellList(stock.ID, stock.Percentage, stock.Quantity); //clone the sell list

                for (int i = 0; i <= stock.Quantity; i++)
                {
                    if (RandomNumberGenerator.NumberBetween(1, 100) <= stock.Percentage)
                    {
                        merchant.AddItemToInventory(ItemFactory.CreateGameItem(stock.ID));
                    }
                }

            }
        }
      
        public Merchant Clone()
        {
            Merchant merchant =
                new Merchant(ID, Name, Ancestry, Description, Markup);
            
            foreach (MerchantStock stock in _sellList)
            {
                merchant.AddItemToSellList(stock.ID, stock.Percentage, stock.Quantity); //clone the sell list
            }

            foreach (ItemQuantity item in Inventory)
            {
                for (int i = 0; i < item.Quantity; i++)
                         merchant.AddItemToInventory(item.BaseItem);
            }
            
            foreach (Item.ItemProperties properties in PreferredItems)
            {
                merchant.AddItemToPreferredItems(properties);
            }
            foreach (Item.ItemProperties properties in DislikedItems)
            {
                merchant.AddItemToDislikedItems(properties);
            }

            return merchant;
        }
    }
}
