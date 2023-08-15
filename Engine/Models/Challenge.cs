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
        public bool Resets { get; set; }
        public int DaysUntilReset { get; set; }
        public int DaysSinceCompleted { get; set; }
        public bool ChallengeCompleted => Obstacles.Any(o => o.Passed == true);

        public Challenge(string name, string description, bool resets,
            int daysUntilReset = 0)
        {
            Name = name;
            Description = description;
            Obstacles = new List<Obstacle>();
            Resets = resets;
            DaysUntilReset = daysUntilReset;
        }

        public void ResetChallenge()
        {
            foreach (var obstacle in Obstacles)
            {
                obstacle.Passed = false;
            }
        }
    }
}
