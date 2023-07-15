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
                    new Body(node.AttributeAsInt("ID"),
                        node.AttributeAsString("Name"),
                        node.AttributeAsString("Description"));
                XmlNodeList bodyParts = node.SelectNodes("/Parts/BodyPart").ToList();

                if(bodyParts != null)
                {
                    foreach (XmlNode childNode in bodyParts)
                    {
                        BodyPart newPart = new BodyPart(childNode.AttributeAsString("Name"),
                        childNode.AttributeAsInt("Health"),
                        childNode.AttributeAsInt("Coverage"));

                        XmlNodeList subParts = node.SelectNodes("./SubParts/SubPart"); //idk how to do this bit
                        if(subParts != null)
                        {
                            foreach(XmlNode childNode2 in subParts)
                            {
                                newPart.SubParts.Add(new BodyPart(childNode2.AttributeAsString("Name"),
                            childNode2.AttributeAsInt("Health"),
                            childNode2.AttributeAsInt("Coverage")));
                            }
                        }
                        body.Parts.Add(newPart);

                    }
                }

                _bodies.Add(body);
            }
        }

        public static Body GetBody(string aString)
        {
            return _bodies.FirstOrDefault(b => b.Name.ToLower() == aString.ToLower()).Clone();
        }

        public static Body GetBodyById(int id)
        {
            return _bodies.FirstOrDefault(b => b.Id == id).Clone();
        }
    }
    
}