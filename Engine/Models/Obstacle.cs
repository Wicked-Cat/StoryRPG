using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Obstacle
    {
        public object Check { get; set; }

        public int Difficulty { get; set; }
        public enum Types { Skill, Characteristic, Item}
        public Types Type { get; set; }
        public Skill RequiredSkill { get; set; }
        public Characteristic RequiredCharacteristic { get; set; }
        public Item RequiredItem { get; set; }

        public Obstacle(object check)
        {
            Check = check;
        }
    }
}
