using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Encounters
    {
        public int MonsterID { get; set; }
        public int ChanceOfEncounter { get; set; }

        public Encounters(int monsterID, int chanceOfEncounter) 
        {
            MonsterID = monsterID;
            ChanceOfEncounter = chanceOfEncounter;
        }
    }
}
