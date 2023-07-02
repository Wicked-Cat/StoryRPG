using Engine.Models;
using Engine.Shared;
using System.Xml;
using System.Xml.Linq;
using static Engine.Models.Ancestry;
using static Engine.Models.Item;
using static Engine.Models.Tag;
using static Engine.Models.TagProperties;

namespace Engine.Factories
{
    public static class TagFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Tags.xml";
        public static readonly List<Tag> _tags = new List<Tag>();
        private static List<Tag.TagType> _tagTypes = Enum.GetValues(typeof(Tag.TagType)).Cast<Tag.TagType>().ToList();
        private static List<TagProperties.TagType> _tagPropertyTypes = Enum.GetValues(typeof(TagProperties.TagType)).Cast<TagProperties.TagType>().ToList();

        static TagFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                XmlDocument data = new XmlDocument();
                data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

                LoadTagsFromNodes(data.SelectNodes("/Tags/Tag"));
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}");
            }
        }

        private static void LoadTagsFromNodes(XmlNodeList nodes)
        {
            if (nodes == null)
                return;

            foreach (XmlNode node in nodes)
            {
                Tag tag = new Tag(node.AttributeAsString("Name"),
                    DetermineTagType(node.AttributeAsString("Type")));

                foreach (XmlNode childNode in node.SelectNodes("/TagProperties/TagProperty"))
                {
                    tag.Properties.Add(new TagProperties(childNode.AttributeAsString("Name"),
                        DetermineTagPropertyType(childNode.AttributeAsString("Type")),
                        childNode.AttributeAsInt("Flat"),
                        childNode.AttributeAsDouble("Multiplier")));
                }

                _tags.Add(tag);
            }
        }

        private static Tag.TagType DetermineTagType(string aString)
        {
            bool match = Enum.IsDefined(typeof(Tag.TagType), aString);
            if (match)
            {
                return _tagTypes.FirstOrDefault(p => p.ToString().ToLower() == aString.ToLower());
            }
            else
            {
                throw new Exception($"No such property {aString} exists.");
            }
        }

        private static TagProperties.TagType DetermineTagPropertyType(string aString)
        {
            bool match = Enum.IsDefined(typeof(TagProperties.TagType), aString);
            if (match)
            {
                return _tagPropertyTypes.FirstOrDefault(p => p.ToString().ToLower() == aString.ToLower());
            }
            else
            {
                throw new Exception($"No such property {aString} exists.");
            }
        }
    }
}
