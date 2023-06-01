namespace Engine.Models
{
    public class EncounterPercent
    {
        public int EncounterID { get; set; }
        public int ChanceOfEncounter { get; set; }

        public EncounterPercent(int encounterID, int chanceOfEncounter) 
        {
            EncounterID = encounterID;
            ChanceOfEncounter = chanceOfEncounter;
        }
    }
}
