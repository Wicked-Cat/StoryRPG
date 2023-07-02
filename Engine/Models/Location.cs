using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Engine.Factories;

namespace Engine.Models
{
    public class Location
    {
        public int XCoordinate { get; set; }
        public int YCoordinate { get; set; }
        public int ZCoordinate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Region { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public List<EncounterPercent> EncountersHere { get; set; } = new List<EncounterPercent>();
        public List<Merchant> MerchantsHere { get; set; } = new List<Merchant>();
        public enum LandSeaSky { Land, Sea, Sky }
        public enum Depth { Underground, Aboveground }
        public enum Shelter { Interior, Exterior }
        public enum Exits { ImpassableWall, Climbable, Pickable }
        public Location(
            int xCoordinate,
            int yCoordinate,
            int zCoordinate,
            string name,
            string description,
            string region,
            string province,
            string country) 
        {
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;
            ZCoordinate = zCoordinate;
            Name = name;
            Description = description;
            Region = region;
            Province = province;
            Country = country;
        }

        public void AddEncounter(int encounterID, int chanceOfEncounter)
        {
            if(EncountersHere.Exists(e => e.EncounterID == encounterID))
            {
                //if the monster has already been added to this locationn, overwrite chanceOfEncounter
                EncountersHere.First(m => m.EncounterID == encounterID).ChanceOfEncounter = chanceOfEncounter;
            }
            else
            {
                //if the monster is not already at this location, add it
                EncountersHere.Add(new EncounterPercent(encounterID, chanceOfEncounter));
            }
        }

        public void AddWall()
        {

        }

        public Encounter GetEncounter()
        {
            if (!EncountersHere.Any())
                return null;

            foreach (EncounterPercent encounter in EncountersHere)
            {
                int rand = RandomNumberGenerator.NumberBetween(1, 100);
                if (rand <= encounter.ChanceOfEncounter)
                {
                    return EncounterFactory.GetEncounter(encounter.EncounterID);
                }
                else
                {
                    return null;
                }
            }
            //if there was a problem, return the last encounter in the list
            return EncounterFactory.GetEncounter(EncountersHere.Last().EncounterID);
        }
    }
}
