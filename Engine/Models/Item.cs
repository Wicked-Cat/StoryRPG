using Engine.Actions;
using Newtonsoft.Json;

namespace Engine.Models
{
    public class Item : BaseNotificationClass
    {
        private int _value;
        private int inventoryNumber;

        public List<Tag> Tags = new List<Tag>();
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
        public int InventoryNumber
        {
            get { return inventoryNumber; }
            set 
            { 
                inventoryNumber = value; 
                OnPropertyChanged();
            }
        }

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
        public void AddTags(Tag tag)
        {
                Tags.Add(tag);
        }
        public void PerformAction(LivingEntity actor, LivingEntity target)
        {
            Action?.Execute(actor, target);
        }

        public Item Clone()
        {
            Item item = new Item(ID, Name, Description, ActualValue, IsUnique, IsEquipped, Action);

            foreach (Tag tag in Tags)
            {
                item.Tags.Add(tag);
            }

            return item;
        }
    }
}
