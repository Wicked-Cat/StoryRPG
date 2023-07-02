using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class BodyPart
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaximumHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int Coverage { get; set; }
        public List<BodyPart> SubParts { get; set; }
        public List<Wound> Wounds { get; set; }

        public BodyPart(int id, string name, string description, int maximumHealth, int coverage)
        {
            Id = id;
            Name = name;
            Description = description;
            MaximumHealth = maximumHealth;
            CurrentHealth = maximumHealth;
            Coverage = coverage;
        }
    }
}
