using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class BodyPart
    {
        public string Name { get; set; }
        public double MaximumHealth { get; set; }
        public double CurrentHealth { get; set; }
        public double Coverage { get; set; }
        public List<BodyPart> SubParts { get; set; }
        public List<Wound> Wounds { get; set; }

        public BodyPart(string name, double maximumHealth, double coverage)
        {
            Name = name;
            MaximumHealth = maximumHealth;
            CurrentHealth = maximumHealth;
            Coverage = coverage;
            SubParts = new List<BodyPart>();
            Wounds = new List<Wound>();
        }
    }
}
