using Engine.ViewModels;
using Engine.Factories;

namespace Engine.Models
{
    public class World
    {
        public List<Location> _locations = new List<Location>();

        public void AddLocation(Location location)
        {
            _locations.Add(location);
        }

        public Location LocationAt(int x, int y, int z)
        {
            foreach(Location loc  in _locations)
            {
                if(loc.XCoordinate == x  && loc.YCoordinate == y && loc.ZCoordinate == z)
                    return loc;
            }
            return null;
        }

        public void RefreshLocations()
        {
            foreach (Location location in _locations)
            {
                RefreshEncounters(location);
                RefreshMerchants(location);
                RefreshItems(location);
            }
        }

        private void RefreshEncounters(Location location)
        {
            location.EncountersHere.Clear();
            foreach (EncounterPercent encounter in location.AllEncountersHere)
            {
                int rand = RandomNumberGenerator.NumberBetween(1, 100);
                if (rand <= encounter.ChanceOfEncounter)
                {
                    location.EncountersHere.Add(EncounterFactory.GetEncounter(encounter.EncounterID));
                }
            }
        }
        private void RefreshMerchants(Location location)
        {
            location.MerchantsHere.Clear();
            foreach (MerchantPercent merchant in location.AllMerchantsHere)
            {
                int rand = RandomNumberGenerator.NumberBetween(1, 100);
                if (rand <= merchant.ChanceOfEncounter)
                {
                    location.MerchantsHere.Add(MerchantFactory.GetMerchantByID(merchant.MerchantID));
                }
            }
        }
        private void RefreshItems(Location location)
        {
            location.ItemsHere.Clear();
            foreach (LocationItems item in location.AllItemsHere)
            {
                if ((!item.Respawns && !item.HasBeenCollected) || item.Respawns)
                {
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        int rand = RandomNumberGenerator.NumberBetween(1, 100);
                        if (rand <= item.Percentage)
                        {
                            location.AddItemToLocation(ItemFactory.GetItemByID(item.ID));
                           /* if (location.ItemsHere.Exists(i => i.BaseItem.ID == item.ID))
                            {
                                location.ItemsHere.First(i => i.BaseItem.ID == item.ID).Quantity++;
                            }
                            else
                            {
                                location.ItemsHere.Add(new ItemQuantity(ItemFactory.GetItemByID(item.ID), 1));
                            }*/
                        }
                    }
                }
            }
        }
        public void TickChallenges()
        {
            foreach(Location location in _locations)
            {
                if(location.ChallengeHere != null && location.ChallengeHere.ChallengeCompleted)
                {
                    location.ChallengeHere.DaysSinceCompleted += 1;
                }
            }
        }
        public void RefreshChallenges(Location CurrentLocation)
        {
            foreach(Location location in _locations)
            {
                if(location.ChallengeHere != null && location.ChallengeHere.DaysSinceCompleted >= location.ChallengeHere.DaysUntilReset 
                    && location != CurrentLocation && location.ChallengeHere.Resets == true)
                {
                    location.ChallengeHere.DaysSinceCompleted = 0;
                    location.ChallengeHere.ResetChallenge();
                }
            }
        }

    }
}
