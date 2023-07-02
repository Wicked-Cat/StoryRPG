using Engine.Models;
using Engine.Shared;
using System.Xml;
using System.Xml.Linq;
using static Engine.Models.Ancestry;

namespace Engine.Factories
{
    public static class AncestryFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Ancestries.xml";
        public static readonly List<Ancestry> _ancestries = new List<Ancestry>();
        static AncestryFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                XmlDocument data = new XmlDocument();
                data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

                LoadItemsFromNodes(data.SelectNodes("/Ancestries/Ancestry"));
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }
        }

        private static void LoadItemsFromNodes(XmlNodeList nodes)
        {
            if (nodes == null)
                return;

            foreach (XmlNode node in nodes)
            {
                /*
                XmlNodeList tags = node.SelectNodes("Tags/Tag");
                List<AncestryTag> toBeAdded = new List<AncestryTag>();
                foreach (XmlNode property in tags)
                {
                    toBeAdded.Add(GetTag(property.AttributeAsString("Name")));
                }
                */
                Ancestry ancestry =
                    new Ancestry(node.AttributeAsInt("Id"),
                        node.AttributeAsString("Name"),
                    node.AttributeAsString($"Description"));
                /*
                foreach (AncestryTag property in toBeAdded)
                {
                    ancestry.Tags.Add(property);
                } */

                foreach (XmlNode childNode in node.SelectNodes("./Multipliers/SkillMultiplier"))
                {
                    ancestry.AddMultiplierToList(childNode.AttributeAsString("Name"),
                        childNode.AttributeAsDouble("Value"),
                        Multiplier.Type.Skill);
                }
                foreach (XmlNode childNode in node.SelectNodes("./Multipliers/CharacteristicMultiplier"))
                {
                    ancestry.AddMultiplierToList(childNode.AttributeAsString("Name"),
                        childNode.AttributeAsDouble("Value"),
                        Multiplier.Type.Characteristic);
                }

                foreach (XmlNode childNode in node.SelectNodes("./Tags/Tag"))
                {
                    ancestry.AddTagToList(GetTag(childNode.AttributeAsString("Name")));
                }

                _ancestries.Add(ancestry);
            }
        }

        public static Tag GetTag(string aString)
        {
            return TagFactory._tags.FirstOrDefault(t => t.Name.ToLower() == aString.ToLower());
        }
        public static Ancestry GetAncestry(string aString)
        {
                return _ancestries.FirstOrDefault(a => a.Name.ToLower() == aString.ToLower()).Clone();
        }
    }
}