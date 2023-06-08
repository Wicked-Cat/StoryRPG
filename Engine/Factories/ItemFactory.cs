using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Engine.Actions;
using Engine.Models;
using Engine.Shared;
using Microsoft.VisualBasic;
using static Engine.Models.Item;

namespace Engine.Factories
{
    public static class ItemFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Items.xml";
        private static readonly List<Item> _gameItems = new List<Item>();
        public static List<ItemProperties> AllProperties = Enum.GetValues(typeof(ItemProperties)).Cast<ItemProperties>().ToList();
        static ItemFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                XmlDocument data = new XmlDocument();
                data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

                LoadItemsFromNodes(data.SelectNodes("/Items/Item"));
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }
        }
        public static Item CreateGameItem(int itemID)
        {
            return _gameItems.FirstOrDefault(item => item.ID == itemID)?.Clone();
        }

        private static void LoadItemsFromNodes(XmlNodeList nodes)
        {
            if (nodes == null)
                return;

            foreach(XmlNode node in nodes)
            {
                XmlNodeList properties = node.SelectNodes("Properties/Property");
                List<ItemProperties> toBeAdded = new List<ItemProperties>();
                foreach(XmlNode property in properties)
                {
                    toBeAdded.Add(GetProperty(property.AttributeAsString("Name")));
                }

                //fill in item constructor using node data
                Item gameItem =
                    new Item(node.AttributeAsInt("ID"),
                    node.AttributeAsString("Name"),
                    node.AttributeAsString($"Description"),
                    node.AttributeAsInt("Value"),
                    toBeAdded.Contains(ItemProperties.Weapon), //IsUnique
                    false); //IsEquipped

                //add properties
                foreach(ItemProperties property in toBeAdded)
                {
                    gameItem.Properties.Add(property);
                }

                //build action
                if (gameItem.Properties.Contains(ItemProperties.Weapon))
                {
                    gameItem.Action = new AttackWithWeapon(gameItem, node.AttributeAsInt("Damage"));
                }
                else if (gameItem.Properties.Contains(ItemProperties.Weapon))
                {
                    gameItem.Action = new Heal(gameItem, 0);
                }

                _gameItems.Add(gameItem);
            }
        }
        public static ItemProperties GetProperty(string aString)
        {
            bool match = Enum.IsDefined (typeof(ItemProperties), aString);
            if (match)
            {
                return AllProperties.FirstOrDefault(p => p.ToString().ToLower() == aString.ToLower());
            }
            else
            {
                throw new Exception($"No such property {aString} exists.");
            }
        }
        public static string ItemName(int itemID)
        {
            //? = if everything in front equals null, dont try to return
            //?? if result is null return ""
            return _gameItems.FirstOrDefault(i => i.ID == itemID)?.Name ?? "";
        }

        public static int ItemID(string itemName)
        {
            return _gameItems.FirstOrDefault(i => i.Name.ToLower() == itemName.ToLower())?.ID ?? default;
        }
        public static Item GetItem(int id)
        {
            return _gameItems.FirstOrDefault(m => m.ID == id).Clone();
        }
        public static bool IsItem(string aString)
        {
            if(_gameItems.Any(i => i.Name.ToLower() == aString.ToLower()))
                return true;

            return false;
        }
    }
}
