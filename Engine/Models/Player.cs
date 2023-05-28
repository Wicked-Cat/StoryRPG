using StoryRPG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Engine.Models
{

    public class Player : LivingEntity
    {
        public Player(string name, string ancestry, string charClass, int maxHealth, int currentHealth, string description, int experience, int cats,
            int strength, int dexterity, int endurance, int perception,
            int sensitivity, int willpower, int appearance, int presence,
            int empathy) 
            : base(name, ancestry, charClass, maxHealth, currentHealth, description, experience, cats,
            strength, dexterity, endurance, perception,
            sensitivity, willpower, appearance, presence,
            empathy)
        {

        }
        public void AddExperience(int experience)
        {
            Experience += experience;
        }

    }
}
