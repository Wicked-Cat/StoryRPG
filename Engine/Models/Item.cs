using Engine.Actions;

namespace Engine.Models
{
    public class Item
    {
        public enum ItemCategory
        {
            Weapon,
            Consumable,
            Miscellaneous
        }
        public ItemCategory Category { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public bool IsUnique { get; set; }
        public bool IsEquipped { get; set; }
        public IAction Action { get; set; }

        public Item(
            ItemCategory category, int id, string name, string description, int price, bool isUnique = false, bool isEquipped = false, IAction action = null)
        {
            Category = category;
            ID = id;
            Name = name;
            Description = description;
            Price = price;
            IsUnique = isUnique;
            IsEquipped = isEquipped;
            Action = action;
        }

        public void PerformAction(LivingEntity actor, LivingEntity target)
        {
            Action?.Execute(actor, target);
        }
        public Item Clone()
        {
            return new Item(Category, ID, Name, Description, Price, IsUnique, IsEquipped, Action);
        }
    }
}
