using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Xml;
using Engine.Models;
using Engine.Shared;

namespace Engine.Factories
{
    public static class MonsterFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Monsters.xml";
        public static readonly List<Monster> _monsters = new List<Monster>();

        static MonsterFactory()
        {
            if(File.Exists(GAME_DATA_FILENAME))
            {
                XmlDocument data = new XmlDocument();
                data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

                LoadMonstersFromNodes(data.SelectNodes("/Monsters/Monster"));
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}.");
            }
        }

        private static void LoadMonstersFromNodes(XmlNodeList nodes)
        {
            if (nodes == null)
            {
                return;
            }
            foreach (XmlNode node in nodes) 
            {
                Monster monster =
                    new Monster(node.AttributeAsInt("ID"),
                                node.AttributeAsString("Name"),
                                node.AttributeAsString("Class"),
                                node.AttributeAsInt("MaximumHealth"),
                                node.AttributeAsInt("MaximumHealth"), //current health
                                node.AttributeAsString("Description"),
                                node.AttributeAsInt("Experience"),
                                node.AttributeAsInt("Cats"),
                                ItemFactory.CreateGameItem(node.AttributeAsInt("WeaponID")));

                XmlNodeList lootItems = node.SelectNodes("LootItems/LootItem");
                if (lootItems != null)
                {
                    foreach (XmlNode item in lootItems)
                    {
                        monster.AddItemToLootTable(item.AttributeAsInt("ID"),
                                                   item.AttributeAsInt("Percentage"),
                                                   item.AttributeAsInt("Quantity"));
                    }
                }

                monster.CurrentAncestry =  AncestryFactory.GetAncestry(node.AttributeAsString("Ancestry"));

                foreach (Skill skill in SkillFactory._skills)
                {
                    monster.Skills.Add(skill.Clone());
                }
                foreach(Characteristic characteristic in CharacteristicFactory._characteristics)
                {
                    monster.Characteristics.Add(characteristic.Clone());
                }

                foreach (Multiplier multiplier in monster.CurrentAncestry.Multipliers)
                {
                    switch (multiplier.MultiplierType)
                    {
                        case Multiplier.Type.Skill:
                            monster.Skills.FirstOrDefault(s => s.Name.ToLower() == multiplier.Name.ToLower()).LevelMultiplier = multiplier.MultiplierValue;
                            break;
                        case Multiplier.Type.Characteristic:
                            monster.Characteristics.FirstOrDefault(c => c.Name.ToLower() == multiplier.Name.ToLower()).LevelMultiplier = multiplier.MultiplierValue;
                            break;
                    }
                }

                XmlNodeList characteristics = node.SelectNodes("Characteristics/Characteristic");
                if (characteristics != null)
                {
                    foreach (XmlNode childNode in characteristics)
                    { 
                        foreach (Characteristic characteristic in monster.Characteristics)
                        {
                            if (characteristic.Name.ToLower() == childNode.AttributeAsString("Name").ToLower())
                            {
                                characteristic.BaseLevel = childNode.AttributeAsDouble("Level");
                            }
                        } 
                    }
                }
               

                _monsters.Add(monster);
            }
        }

        public static Monster GetMonster(int id)
        {
            return _monsters.FirstOrDefault(m => m.ID == id)?.Clone();
        }

        public static bool IsMonster(string aString)
        {
            if (_monsters.Any(i => i.Name.ToLower() == aString.ToLower()))
                return true;

            return false;
        }
    }
}