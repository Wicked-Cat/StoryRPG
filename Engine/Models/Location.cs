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
        public string Name { get; set; }
        public string Description { get; set; }
        public string Region { get; set; }
        public string Province { get; set; }
        public string Country { get; set; }
        public List<Encounters> EncountersHere { get; set; } = new List<Encounters>();

        public Location(
            int xCoordinate,
            int yCoordinate,
            string name,
            string description,
            string region,
            string province,
            string country) 
        {
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;
            Name = name;
            Description = description;
            Region = region;
            Province = province;
            Country = country;
        }

        public void AddMonster(int monsterID, int chanceOfEncounter)
        {
            if(EncountersHere.Exists(e => e.MonsterID == monsterID))
            {
                //if the monster has already been added to this locationn, overwrite chanceOfEncounter
                EncountersHere.First(m => m.MonsterID == monsterID).ChanceOfEncounter = chanceOfEncounter;
            }
            else
            {
                //if the monster is not already at this location, add it
                EncountersHere.Add(new Encounters(monsterID, chanceOfEncounter));
            }
        }

        public Monster GetMonster()
        {
            if (!EncountersHere.Any())
                return null;

            int rand = RandomNumberGenerator.NumberBetween(1, 100);
            foreach (Encounters encounter in EncountersHere)
            {
                if(rand <= encounter.ChanceOfEncounter)
                {
                    return MonsterFactory.GetMonster(encounter.MonsterID);
                }
            }
            //if there was a problem, return the last monster in the list
            return MonsterFactory.GetMonster(EncountersHere.Last().MonsterID);

            /*
            int totalChances = EncountersHere.Sum(m => m.ChanceOfEncounter);
            int randomNum = RandomNumberGenerator.NumberBetween(1, totalChances);
            int runningTotal = 0;

            foreach(Encounters encounters in EncountersHere)
            {
                runningTotal += encounters.ChanceOfEncounter;

                if(randomNum <= runningTotal)
                {
                    return MonsterFactory.GetMonster(encounters.MonsterID);
                }
            }

            //if there was a problem, return the last monster in the list
            return MonsterFactory.GetMonster(EncountersHere.Last().MonsterID);
            */
        }
    }
}
