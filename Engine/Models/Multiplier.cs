using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Multiplier
    {
        public string Name { get; set; }    
        public double MultiplierValue { get; set; }
        public enum Type { Skill, Characteristic }
        public Type MultiplierType { get; set; }

        public Multiplier(string name, double multiplierValue, Type multiplierType)
        {
            Name = name;
            MultiplierValue = multiplierValue;
            MultiplierType = multiplierType;
        }
    }
}
