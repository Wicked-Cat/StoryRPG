using Engine.Models;
using System.Xml;
using Engine.Shared;

namespace Engine.Factories
{
        public static class EncounterFactory
        {
            private const string GAME_DATA_FILENAME = ".\\GameData\\Encounters.xml";
            public static readonly List<Encounter> _encounters = new List<Encounter>();

            static EncounterFactory()
            {
                if (File.Exists(GAME_DATA_FILENAME))
                {
                    XmlDocument data = new XmlDocument();
                    data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

                    LoadEncountersFromNodes(data.SelectNodes("/Encounters/Encounter"));
                }
                else
                {
                    throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}.");
                }
            }

            private static void LoadEncountersFromNodes(XmlNodeList nodes)
            {
                if (nodes == null)
                {
                    return;
                }
                foreach (XmlNode node in nodes)
                {
                    Encounter encounter =
                        new Encounter(node.AttributeAsInt("ID"),
                                         node.AttributeAsString("Name"));

                    XmlNodeList monsters = node.SelectNodes("Monsters/Monster");
                    if (monsters != null)
                    {
                        foreach (XmlNode monster in monsters)
                        {
                            encounter.AddMonsterToList(monster.AttributeAsInt("ID"));
                        }
                    }
                _encounters.Add(encounter);
            }
            }

            public static Encounter GetEncounter(int id)
            {
                return _encounters.FirstOrDefault(e => e.ID == id)?.Clone();
            }
        }
    }
