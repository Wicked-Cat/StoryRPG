namespace Engine.Models
{
    public class MerchantPercent
    {
        public int MerchantID { get; set; }
        public int ChanceOfEncounter { get; set; }

        public MerchantPercent(int merchantID, int chanceOfEncounter)
        {
            MerchantID = merchantID;
            ChanceOfEncounter = chanceOfEncounter;
        }
    }
}
