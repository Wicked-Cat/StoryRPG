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
    public static class BodyFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Bodies.xml";
        public static readonly List<Body> _bodies = new List<Body>();

        static BodyFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                XmlDocument data = new XmlDocument();
                data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

                LoadBodiesFromNodes(data.SelectNodes("/Bodies/Body"));
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}.");
            }
        }

        private static void LoadBodiesFromNodes(XmlNodeList nodes)
        {
            if (nodes == null)
            {
                return;
            }
            foreach (XmlNode node in nodes)
            {
                Body body =
                    new Body(node.AttributeAsString("Name"),
                    node.AttributeAsString("Description"));



                _bodies.Add(body);
            }
        }
    }
    */
}
