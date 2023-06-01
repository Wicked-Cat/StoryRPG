using System.Collections.Generic;
using Engine.Factories;

namespace Engine.Models
{
    public class Monster : LivingEntity
    {
        public readonly List<MonsterLoot> _lootTable = new List<MonsterLoot>();
        public int ID { get; }
        public Monster(int id, string name, string ancestry, string charClass, int maxHealth, int currentHealth, string description, int experience, int cats,
            int strength, int dexterity, int endurance, int perception, int sensitivity, int willpower, int appearance, 
            int presence, int empathy, Item equippedWeapon)
            : base(name, ancestry, charClass, maxHealth, currentHealth, description, experience, cats,
            strength, dexterity, endurance, perception,
            sensitivity, willpower, appearance, presence,
            empathy)
        {
            ID = id;
            EquippedWeapon = equippedWeapon;

        }

        public void AddItemToLootTable(int id, int percentage, int ammount)
        {
            _lootTable.RemoveAll(ml => ml.ID == id); //remove entry from loot table if it already exists
            _lootTable.Add(new MonsterLoot(id, percentage, ammount));
        }

        public Monster Clone()
        {
            Monster monster = new Monster(ID, Name, Ancestry, CharClass, MaximumHealth, CurrentHealth, Description, Experience, Cats,
                Strength, Dexterity, Endurance, Perception, Sensitivity, Willpower, Appearance, Presence, Empathy, 
                EquippedWeapon);

            foreach (ItemQuantity item in Inventory)
            {
                for (int i = 0; i < item.Quantity; i++)
                {
                    monster.AddItemToInventory(item.BaseItem);
                }
            }

            foreach (MonsterLoot item in _lootTable)
            {
                //for every item, determine how many will be added to inventory
                for (int i = 0; i < item.Quantity; i++)
                {
                    if(RandomNumberGenerator.NumberBetween(1, 100) <= item.Percentage)
                    {
                        monster.AddItemToInventory(ItemFactory.CreateGameItem(item.ID));
                    }
                }
            }

            return monster;
        }
    }
}
