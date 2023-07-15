using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Obstacle
    {
        public int Difficulty { get; set; }
        public enum Types { Skill, Characteristic, Item}
        public Types Type { get; set; }
        public Skill RequiredSkill { get; set; }
        public Characteristic RequiredCharacteristic { get; set; }
        public Item RequiredItem { get; set; }

        public Obstacle(int difficulty, Types type, Skill requiredSkill,
            Characteristic requiredCharacteristic, Item requiredItem)
        {
            Difficulty = difficulty;
            Type = type;
            RequiredSkill = requiredSkill;
            RequiredCharacteristic = requiredCharacteristic;
            RequiredItem = requiredItem;
        }
    }
}
