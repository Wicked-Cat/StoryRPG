using Engine.Actions;

namespace Engine.Models
{
    public class Item : BaseNotificationClass
    {
        private int _value {  get; set; }
        public enum ItemProperties
        {
            Weapon, NaturalWeapon, Consumable, Miscellaneous, Resource, Food,
            Wood, Bone, Meat, Hide, Wheat,
            Rodent, Feline, Insect
        }

        public List<ItemProperties> Properties = new List<ItemProperties>();
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ActualValue { get; set; }
        public int Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged(); }
        }
        public bool IsUnique { get; set; }
        public bool IsEquipped { get; set; }
        public IAction Action { get; set; }

        public Item(
            int id, string name, string description, int actualValue, bool isUnique, bool isEquipped = false, IAction action = null)
        {
            ID = id;
            Name = name;
            Description = description;
            ActualValue = actualValue;
            IsUnique = isUnique;
            IsEquipped = isEquipped;
            Action = action;
        }
        public void AddProperty(ItemProperties property)
        {
                Properties.Add(property);
        }
        public void PerformAction(LivingEntity actor, LivingEntity target)
        {
            Action?.Execute(actor, target);
        }

        public Item Clone()
        {
            Item item = new Item(ID, Name, Description, ActualValue, IsUnique, IsEquipped, Action);

            foreach (ItemProperties properties in Properties)
            {
                item.Properties.Add(properties);
            }

            return item;
        }
    }
}
