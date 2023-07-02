using Engine.Models;
using Engine.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Engine.Factories
{
    /*
    public static class BodyPartFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\BodyParts.xml";
        public static readonly List<BodyPart> _bodyparts = new List<BodyPart>();

        static BodyPartFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                XmlDocument data = new XmlDocument();
                data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

                LoadBodyPartsFromNodes(data.SelectNodes("/BodyParts/BodyPart"));
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}.");
            }
        }

        private static void LoadBodyPartsFromNodes(XmlNodeList nodes)
        {
            if (nodes == null)
            {
                return;
            }
            foreach (XmlNode node in nodes)
            {
                BodyPart bodypart =
                    new BodyPart(node.AttributeAsString("Name"),
                    node.AttributeAsString("Description"));



                _bodyparts.Add(bodypart);
            }
        }
    }
    */
}
