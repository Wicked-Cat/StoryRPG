using Engine.Factories;

namespace Engine.Models
{
    public class Merchant : LivingEntity
    {
        public int ID { get; }

        public readonly List<Tag> PreferredItems = new List<Tag>();
        public readonly List<Tag> DislikedItems = new List<Tag>();
        public int Markup { get; set; }

        public readonly List<MerchantStock> _sellList = new List<MerchantStock>();
        public Merchant(int id, string name,  string description, int markup) : 
            base(name, "Merchant", 10, 10, description, 10, 0)
        {
            ID = id;
            Markup = markup;

        }

        public void AddItemToSellList(int id, int percentage, int quantity)
        {
            _sellList.RemoveAll(ml => ml.ID == id);
            _sellList.Add(new MerchantStock(id, percentage, quantity));
        }
        public void AddItemToPreferredItems(Tag property)
        {
            PreferredItems.RemoveAll(p => p == property);
            PreferredItems.Add(property);
        }
        public void AddItemToDislikedItems(Tag property)
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
                new Merchant(ID, Name, Description, Markup);
            
            foreach (MerchantStock stock in _sellList)
            {
                merchant.AddItemToSellList(stock.ID, stock.Percentage, stock.Quantity); //clone the sell list
            }

            foreach (ItemQuantity item in Inventory)
            {
                for (int i = 0; i < item.Quantity; i++)
                         merchant.AddItemToInventory(item.BaseItem);
            }
            
            foreach (Tag properties in PreferredItems)
            {
                merchant.AddItemToPreferredItems(properties);
            }
            foreach (Tag properties in DislikedItems)
            {
                merchant.AddItemToDislikedItems(properties);
            }

            merchant.CurrentAncestry = CurrentAncestry;

            //add skills
            foreach(Skill skill in SkillFactory._skills)
            {
                merchant.Skills.Add(skill);
            }

            return merchant;
        }
    }
}
