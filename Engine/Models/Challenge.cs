using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Challenge
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Obstacle> Obstacles { get; set; }
        public int DaysUntilReset { get; set; }
        public bool Passed { get; set; }

        public Challenge(string name, string description,  List<Obstacle> obstacles, 
            int daysUntilReset, bool passed = false)
        {
            Name = name;
            Description = description;
            Obstacles = new List<Obstacle>();
            DaysUntilReset = daysUntilReset;
        }
    }
}
