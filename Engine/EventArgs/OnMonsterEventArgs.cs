using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.EventArgs
{
    public class OnMonsterEventArgs : System.EventArgs
    {
        public Monster Monster { get; set; }

        public OnMonsterEventArgs(Monster monster)
        {
            Monster = monster;


        }
    }
}