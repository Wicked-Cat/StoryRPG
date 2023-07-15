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
        public int MaximumHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int Coverage { get; set; }
        public List<BodyPart> SubParts { get; set; }
        public List<Wound> Wounds { get; set; }

        public BodyPart(string name, int maximumHealth, int coverage)
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
