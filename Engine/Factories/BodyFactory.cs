using Engine.Models;
using Engine.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

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

                //XmlNodeList list = node.SelectNodes("/Parts/BodyPart");
                if(node.SelectNodes("Parts/BodyPart") is XmlNodeList bodyParts)
                {
                    var xmlNodesBodyParts = bodyParts.Cast<XmlNode>().ToList();
                    foreach (XmlNode childNode in xmlNodesBodyParts)
                    {
                        BodyPart newPart = new BodyPart(childNode.AttributeAsString("Name"),
                        childNode.AttributeAsDouble("Health"),
                        childNode.AttributeAsDouble("Coverage"));

                        if(node.SelectNodes("SubParts/SubPart") is XmlNodeList subParts)
                        {
                            var xmlNodesSubParts = subParts.Cast<XmlNode>().ToList();
                            foreach(XmlNode subNode in xmlNodesSubParts)
                            {
                                BodyPart subPart = new BodyPart(subNode.AttributeAsString("Name"),
                                    subNode.AttributeAsDouble("Health"),
                                    subNode.AttributeAsDouble("Coverage"));
                                newPart.SubParts.Add(subPart);
                            }
                        }

                        body.Parts.Add(newPart);

                    }
                }
                /*
                if(node.SelectNodes("Parts/BodyPart/SubParts/SubPart") is XmlNodeList subParts)
                {
                    var xmlNodesSubParts = subParts.Cast<XmlNode>().ToList();
                    foreach (XmlNode childNode in xmlNodesSubParts)
                    {
                            body.Parts.First(p => p.Name.ToLower() == childNode.AttributeAsString("Parent").ToLower()).SubParts.
                            Add(new BodyPart(childNode.AttributeAsString("Name"),
                            childNode.AttributeAsDouble("Health"),
                            childNode.AttributeAsDouble("Coverage")));

                    }
                }
                */
                _bodies.Add(body);
            }
        }

        public static Body GetBody(string aString)
        {
            return _bodies.FirstOrDefault(b => b.Name.ToLower() == aString.ToLower())?.Clone();
        }

        public static Body GetBodyById(int id)
        {
            return _bodies.FirstOrDefault(b => b.Id == id)?.Clone();
        }
    }
    
}