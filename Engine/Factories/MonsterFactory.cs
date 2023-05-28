using System.Collections.Generic;
using System.Linq;
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
                                node.AttributeAsString("Ancestry"),
                                node.AttributeAsString("Class"),
                                node.AttributeAsInt("MaximumHealth"),
                                node.AttributeAsInt("MaximumHealth"), //current health
                                node.AttributeAsString("Description"),
                                node.AttributeAsInt("Experience"),
                                node.AttributeAsInt("Cats"),
                                node.AttributeAsInt("STR"),
                                node.AttributeAsInt("DEX"),
                                node.AttributeAsInt("END"),
                                node.AttributeAsInt("PER"),
                                node.AttributeAsInt("SEN"),
                                node.AttributeAsInt("WIL"),
                                node.AttributeAsInt("APP"),
                                node.AttributeAsInt("PRE"),
                                node.AttributeAsInt("EMP"),
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
                _monsters.Add(monster);
            }
        }

        public static Monster GetMonster(int id)
        {
            return _monsters.FirstOrDefault(m => m.ID == id).Clone();
        }
    }
}