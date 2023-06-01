using Engine.Models;

namespace Engine.EventArgs
{
    public class OnEncounterEventArgs : System.EventArgs
    {
        public Encounter Encounter { get; set; }

        public OnEncounterEventArgs(Encounter encounter)
        {
            Encounter = encounter;


        }
    }
}