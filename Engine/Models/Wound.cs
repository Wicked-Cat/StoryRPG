using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Wound
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Damage { get; set; }
        public enum WoundType { Laceration, Bruise, Puncture, Burn, Frostbite }
        public WoundType Type { get; set;}

        public Wound(string name, string description, int damage, WoundType type)
        {
            Name = name;
            Description = description;
            Damage = damage;
            Type = type;
        }
    }
}
