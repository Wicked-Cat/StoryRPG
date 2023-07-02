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
    public static class SkillFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Skills.xml";
        public static readonly List<Skill> _skills = new List<Skill>();

        static SkillFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                XmlDocument data = new XmlDocument();
                data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

                LoadSkillsFromNodes(data.SelectNodes("/Skills/Skill"));
            }
            else
            {
                throw new FileNotFoundException($"Missing data file: {GAME_DATA_FILENAME}.");
            }
        }

        private static void LoadSkillsFromNodes(XmlNodeList nodes)
        {
            if (nodes == null)
            {
                return;
            }
            foreach (XmlNode node in nodes)
            {
                Skill skill =
                    new Skill(node.AttributeAsString("Name"),
                    node.AttributeAsString("Description"));

                skill.Category = DetermineCategory(node.AttributeAsString("Category"));


                _skills.Add(skill);
            }
        }

        private static Skill.Categories DetermineCategory(string aString)
        {
            switch(aString.ToLower())
            {
                case "survival":
                    return Skill.Categories.Survival;
                    break;
                case "combat":
                    return Skill.Categories.Combat;
                    break;
                case "mystic":
                    return Skill.Categories.Mystic;
                    break;
                case "social":
                    return Skill.Categories.Social;
                    break;
                case "crafting":
                    return Skill.Categories.Crafting;
                    break;
                case "curse":
                    return Skill.Categories.Curse;
                    break;
            }

            return Skill.Categories.Survival;
        }
       
    }
}
