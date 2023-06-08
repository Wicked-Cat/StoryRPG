using System.IO;
using System.Xml;
using Engine.Models;
using Engine.Shared;

namespace Engine.Factories
{
    internal static class WorldFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Locations.xml";
        internal static World CreateWorld()
        {
            World world = new World();
            if(File.Exists(GAME_DATA_FILENAME))
            {
                XmlDocument data = new XmlDocument();
                data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));
                LoadLocationsFromNodes(world,
                    data.SelectNodes("/Locations/Location"));
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }
            return world;
        }

        private static void LoadLocationsFromNodes(World world, XmlNodeList nodes)
        {
            if(nodes == null)
            {
                return;
            }
            foreach(XmlNode node in nodes)
            {
                Location location =
                    new Location(node.AttributeAsInt("X"),
                    node.AttributeAsInt("Y"),
                    node.AttributeAsInt("Z"),
                    node.AttributeAsString("Name"),
                    node.SelectSingleNode("./Description")?.InnerText ?? "",
                    node.AttributeAsString("Region"),
                    node.AttributeAsString("Province"),
                    node.AttributeAsString("Country"));
                AddEncounters(location, node.SelectNodes("./Encounters/Encounter"));
                AddMerchants(location, node.SelectNodes("./Merchants/Merchant"));
                world.AddLocation(location);
            }
        }

        private static void AddEncounters(Location location, XmlNodeList encounter)
        {
            if (encounter == null)
            {
                return;
            }
            foreach (XmlNode encounterNode in encounter)
            {
                location.AddEncounter(encounterNode.AttributeAsInt("ID"),
                    encounterNode.AttributeAsInt("Percent"));
            }
        }
        private static void AddMerchants(Location location, XmlNodeList merchantsHere)
        {
            if (merchantsHere == null)
            {
                return;
            }
            foreach (XmlNode node in merchantsHere)
            {
                location.MerchantsHere.Add(MerchantFactory.GetMerchantByID(node.AttributeAsInt("ID")));
            }
        }
    }
}