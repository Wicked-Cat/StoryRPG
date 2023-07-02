using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class TagProperties
    {
        public string Name { get; set; }
        public enum TagType { Skill, Characteristic, Item}
        public  TagType Type { get; set; }
        public int Flat { get; set; }
        public double Modifier { get; set; }

        public TagProperties(string name, TagType type, int flat, double modifier)
        {
            Name = name;
            Type = type;
            Flat = flat;
            Modifier = modifier;
        }
    }
}
