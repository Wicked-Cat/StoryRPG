using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Engine.Factories;
using System.Xml.Linq;
using Engine.Models;
using Engine.Shared;
using System.Threading;

namespace Engine.Factories
{
    public static class MerchantFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Merchants.xml";
        private static readonly List<Merchant> _merchants = new List<Merchant>();
        private static List<Merchant> GeneratedMerchants = new List<Merchant>(); 

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
                    merchant.AddItemToPreferredItems(ItemFactory.GetTag(childNode.AttributeAsString("Name")));
                }
                foreach (XmlNode childNode in node.SelectNodes("./DislikedItems/Item"))
                {
                    merchant.AddItemToDislikedItems(ItemFactory.GetTag(childNode.AttributeAsString("Name")));
                }

                merchant.CurrentAncestry = AncestryFactory.GetAncestry(node.AttributeAsString("Ancestry"));

                _merchants.Add(merchant);
                GenerateMerchants();

            }
        }
        public static Merchant GetMerchantByID(int id)
        {
            return GeneratedMerchants.FirstOrDefault(m => m.ID == id).Clone();
        }
        public static Merchant GetMerchant(string aString)
        {
            return GeneratedMerchants.FirstOrDefault(m => m.Name.ToLower() == aString.ToLower()).Clone();
        }

        public static void GenerateMerchants()
        {
            GeneratedMerchants.Clear();
            foreach (Merchant baseMerchant in _merchants)
            {
                Merchant merchant = baseMerchant.Clone();
                merchant.RefreshStock(merchant);
                GeneratedMerchants.Add(merchant);
            }
        }

        public static void UpdateMerchant(Merchant merchant)
        {
            foreach(Merchant gMerchant in GeneratedMerchants)
            {
                if(gMerchant.ID == merchant.ID)
                {
                    gMerchant.Inventory = merchant.Inventory;
                }
            }
        }
    }
}