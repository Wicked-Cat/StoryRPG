using Engine.Models;

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