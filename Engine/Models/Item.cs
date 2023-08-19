using Engine.Actions;
using Newtonsoft.Json;

namespace Engine.Models
{
    public class Item : BaseNotificationClass
    {
        private int _value;
        private int inventoryNumber;
        [JsonIgnore]
        public List<Tag> Tags = new List<Tag>();
        public int ID { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public string Description { get; set; }
        [JsonIgnore]
        public int ActualValue { get; set; }
        [JsonIgnore]
        public int Value
        {
            get { return _value; }
            set { _value = value; OnPropertyChanged(); }
        }
        [JsonIgnore]
        public bool IsUnique { get; set; }
        [JsonIgnore]
        public bool IsEquipped { get; set; }
        [JsonIgnore]
        public IAction Action { get; set; }
        [JsonIgnore]
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
