using Engine.Factories;
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
        public double GlobalExperienceModifier { get; set; }
        public Player(string name, string description) 
            : base(name, description)
        {
            
        }

        public Item FindNumberedItem(string aString)
        {
            bool parse = int.TryParse(aString, out int num);
            if (parse)
            {
                if ((num <= Inventory.Count()) && (num > 0))
                    return Inventory[num - 1].BaseItem;
                else
                    return default;
            }
            else
            {
                return GetItemFromInventory(aString);
            }
        }

    }
}
