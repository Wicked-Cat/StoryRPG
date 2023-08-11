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
        public string Description { get; set; }
        public int CheckValue { get; set; }
        public bool Passed { get; set; }
        public int SelectionNumber { get; set; }

        public Obstacle(object check, string description, int checkValue)
        {
            Check = check;
            CheckValue = checkValue;
            Description = description;
        }
    }
}
