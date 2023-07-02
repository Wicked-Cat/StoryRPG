using Engine.Actions;
using Engine.Models;
using Engine.Shared;
using static Engine.Models.Item;
using System.Xml;

namespace Engine.Factories
{
    public static class CharacteristicFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Characteristics.xml";
        public static readonly List<Characteristic> _characteristics = new List<Characteristic>();
        static CharacteristicFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                XmlDocument data = new XmlDocument();
                data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

                LoadItemsFromNodes(data.SelectNodes("/Characteristics/Characteristic"));
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
                
                Characteristic characteristic =
                    new Characteristic(node.AttributeAsString("Name"),
                    node.AttributeAsString($"Description"));

              

                _characteristics.Add(characteristic);
            }
        }


    }
}
