using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Engine.Actions;
using Engine.Models;
using Engine.Shared;

namespace Engine.Factories
{
    public static class ItemFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Items.xml";
        private static readonly List<Item> _gameItems = new List<Item>();

        static ItemFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                XmlDocument data = new XmlDocument();
                data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

                LoadItemsFromNodes(data.SelectNodes("/Items/Weapons/Weapon"));
                LoadItemsFromNodes(data.SelectNodes("/Items/Consumables/Consumable"));
                LoadItemsFromNodes(data.SelectNodes("/Items/MiscellaneousItems/Miscellaneous"));
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

        private static void BuildMiscItem(int id, string name, string description, int price)
        {
            _gameItems.Add(new Item(Item.ItemCategory.Miscellaneous, id, name, description, price));
        }

        private static void BuildWeapon(int id, string name, string description, int price, int damage)
        {
            Item weapon = new Item(Item.ItemCategory.Weapon, id, name, description, price, true);
            weapon.Action = new AttackWithWeapon(weapon, damage);
            _gameItems.Add(weapon);
        }

        private static void BuildConsumable(int id, string name, string description, int price, int toHeal)
        {

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
        private static void LoadItemsFromNodes(XmlNodeList nodes)
        {
            if (nodes == null)
                return;

            foreach(XmlNode node in nodes)
            {
                //node names must match item categories 
                Item.ItemCategory itemCategory = DetermineItemCategory(node.Name);

                //fill in item constructor using node data
                Item gameItem =
                    new Item(itemCategory,
                    node.AttributeAsInt("ID"),
                    node.AttributeAsString("Name"),
                    node.AttributeAsString("Description"),
                    node.AttributeAsInt("Price"),
                    itemCategory == Item.ItemCategory.Weapon, //if category is weapon, IsUnique == true
                    false);

                if(itemCategory == Item.ItemCategory.Weapon)
                {
                    gameItem.Action =
                        new AttackWithWeapon(gameItem,
                        node.AttributeAsInt("Damage"));
                }
                else if(itemCategory == Item.ItemCategory.Consumable)
                {

                }
                _gameItems.Add(gameItem);
            }
        }

        private static Item.ItemCategory DetermineItemCategory(string itemType)
        {
            switch (itemType)
            {
                case "Weapon":
                    return Item.ItemCategory.Weapon;
                case "Consumable":
                    return Item.ItemCategory.Consumable;
                default:
                    return Item.ItemCategory.Miscellaneous;
            }
        }
    }
}
