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
            if (File.Exists(GAME_DATA_FILENAME))
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
                    node.SelectSingleNode("./Description")?.InnerText ?? "");
                AddEncounters(location, node.SelectNodes("./Encounters/Encounter"));
                AddMerchants(location, node.SelectNodes("./Merchants/Merchant"));
                AddItems(location, node.SelectNodes("./Items/Item"));
                AddChallenge(location, node.SelectNodes("./Challenge"));
                AddObstacles(location.ChallengeHere, node.SelectNodes("./Challenge/Obstacles/Obstacle"));
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
                location.AddMerchant(node.AttributeAsInt("ID"),
                    node.AttributeAsInt("Percent"));
            }
        }
        private static void AddItems(Location location, XmlNodeList itemsHere)
        {
            if (itemsHere == null)
            {
                return;
            }
            foreach(XmlNode node in itemsHere)
            {
                location.AddItems(node.AttributeAsInt("ID"),
                    node.AttributeAsInt("Percent"),
                    node.AttributeAsInt("Quantity"),
                    node.AttributeAsBool("Respawns"));

            }
        }
        private static void AddChallenge(Location location, XmlNodeList challengeHere)
        {
            if (challengeHere == null)
                return;

            foreach (XmlNode node in challengeHere)
            {
                location.ChallengeHere = new Challenge(node.AttributeAsString("Name"),
                    node.AttributeAsString("Description"),
                    node.AttributeAsBool("Resets"),
                    (node.AttributeAsBool("Resets") == true ? node.AttributeAsInt("DaysUntilReset") : 0));

                /*
                XmlNodeList obstacleNodes = node.SelectNodes("./Obstacles/Obstacle");
                foreach(XmlNode oNode in obstacleNodes)
                {
                    location.ChallengeHere.Obstacles.Add(new Obstacle(DetermineObstacleType(node.AttributeAsString("Check"))));
                } */
            }

        }

        private static void AddObstacles(Challenge challenge, XmlNodeList obstacles)
        {
            foreach(XmlNode node in obstacles)
            {
                challenge.Obstacles.Add(new Obstacle(DetermineObstacleType(node.AttributeAsString("Check")), 
                    node.AttributeAsString("Description"),
                    node.AttributeAsInt("CheckValue"),
                    node.AttributeAsString("PassText"),
                    node.AttributeAsString("FailText")));
            }
        }

        private static object DetermineObstacleType(string aString)
        {
            bool parse = int.TryParse(aString, out int num);

            if (parse)
            {
                Item item = ItemFactory.CreateGameItem(num);
                if (item != null)
                    return item;
            }
            else
            {
                if(CharacteristicFactory._characteristics.Any(c => c.Name.ToLower() == aString.ToLower()))
                {
                    return CharacteristicFactory._characteristics.First(c => c.Name.ToLower() == aString.ToLower());
                }
                else if(SkillFactory._skills.Any(s => s.Name.ToLower() == aString.ToLower()))
                {
                    return SkillFactory._skills.First(s => s.Name.ToLower() == aString.ToLower());
                }
                else
                {
                    throw new Exception($"No check with the name {aString}");
                }
            }
            return null;
        }
    }
}