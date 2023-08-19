using System.Collections.Generic;
using Engine.Factories;

namespace Engine.Models
{
    public class Monster : LivingEntity
    {
        public readonly List<MonsterLoot> _lootTable = new List<MonsterLoot>();
        public int ID { get; }
        public Monster(int id, string name, string description,
            Item equippedWeapon)
            : base(name, description)
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
            Monster monster = new Monster(ID, Name, Description,
                EquippedWeapon);

            foreach (MonsterLoot item in _lootTable)
            {
                monster.AddItemToLootTable(item.ID, item.Percentage, item.Quantity); //clone loot table

                //for every item, determine how many will be added to inventory
                for (int i = 0; i < item.Quantity; i++)
                {
                    if(RandomNumberGenerator.NumberBetween(1, 100) <= item.Percentage)
                    {
                        monster.AddItemToInventory(ItemFactory.CreateGameItem(item.ID));
                    }
                }
            }

            monster.Ancestry = Ancestry;

            foreach(Skill skill in Skills)
            {
                monster.Skills.Add(skill);
            }
            foreach(Characteristic characteristic in Characteristics)
            {
                monster.Characteristics.Add(characteristic);

            }

            foreach (Multiplier multiplier in Ancestry.Multipliers)
            {
                switch (multiplier.MultiplierType)
                {
                    case Multiplier.Type.Skill:
                        Skills.FirstOrDefault(s => s.Name.ToLower() == multiplier.Name.ToLower()).LevelMultiplier = multiplier.MultiplierValue;
                        break;
                    case Multiplier.Type.Characteristic:
                        Characteristics.FirstOrDefault(c => c.Name.ToLower() == multiplier.Name.ToLower()).LevelMultiplier = multiplier.MultiplierValue;
                        break;
                }
            }
            foreach(Characteristic characteristic in Characteristics)
            {
                monster.Characteristics.FirstOrDefault(c => c.Name.ToLower() == characteristic.Name.ToLower()).BaseLevel = characteristic.BaseLevel;
            }

            monster.CurrentBody = CurrentBody.Clone();

            return monster;
        }
    }
}
