using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Engine.Models;
using Engine.Shared;

namespace Engine.Factories
{
    public static class MerchantFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Merchants.xml";
        private static readonly List<Merchant> _merchants = new List<Merchant>();

        static MerchantFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                XmlDocument data = new XmlDocument();
                data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));
                LoadMerchantsFromNodes(data.SelectNodes("Merchants/Merchant"));
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }
        }

        private static void LoadMerchantsFromNodes(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                Merchant merchant =
                    new Merchant(node.AttributeAsInt("ID"),
                    node.AttributeAsString("Name"),
                    node.AttributeAsString("Ancestry"),
                    node.SelectSingleNode("./Description")?.InnerText ?? "",
                    node.AttributeAsInt("Markup"));

                foreach (XmlNode childNode in node.SelectNodes("./SellList/Item"))
                {
                    merchant.AddItemToSellList(childNode.AttributeAsInt("ID"),
                        childNode.AttributeAsInt("Percentage"),
                        childNode.AttributeAsInt("Quantity"));
                }
                foreach (XmlNode childNode in node.SelectNodes("./PreferredItems/Item"))
                {
                    merchant.AddItemToPreferredItems(ItemFactory.GetProperty(childNode.AttributeAsString("Name")));
                }
                foreach (XmlNode childNode in node.SelectNodes("./DislikedItems/Item"))
                {
                    merchant.AddItemToDislikedItems(ItemFactory.GetProperty(childNode.AttributeAsString("Name")));
                }

                _merchants.Add(merchant);
            }
        }
        public static Merchant GetMerchantByID(int id)
        {
            return _merchants.FirstOrDefault(m => m.ID == id).Clone();
        }
        public static Merchant GetMerchant(string aString)
        {
            return _merchants.FirstOrDefault(m => m.Name.ToLower() == aString.ToLower()).Clone();
        }
    }
}