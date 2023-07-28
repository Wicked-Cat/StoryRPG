using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Factories;

namespace Engine.Models
{
    public class World
    {
        private List<Location> _locations = new List<Location>();

        internal void AddLocation(Location location)
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

                /*#
                location.EncountersHere.Clear();
                location.MerchantsHere.Clear();
                location.ItemsHere.Clear();
                foreach (EncounterPercent encounter in location.AllEncountersHere)
                {
                    int rand = RandomNumberGenerator.NumberBetween(1, 100);
                    if (rand <= encounter.ChanceOfEncounter)
                    {
                        location.EncountersHere.Add(EncounterFactory.GetEncounter(encounter.EncounterID));
                    }
                }
                foreach (MerchantPercent merchant in location.AllMerchantsHere)
                {
                    int rand = RandomNumberGenerator.NumberBetween(1, 100);
                    if (rand <= merchant.ChanceOfEncounter)
                    {
                        location.MerchantsHere.Add(MerchantFactory.GetMerchantByID(merchant.MerchantID));
                    }
                }
                foreach (LocationItems item in location.AllItemsHere)
                {
                    if ((!item.HasBeenCollected && item.IsUnique) || !item.IsUnique)
                    {
                        for (int i = 0; i < item.Quantity; i++)
                        {
                            int rand = RandomNumberGenerator.NumberBetween(1, 100);
                            if (rand <= item.Percentage)
                            {
                                if(location.ItemsHere.Exists(i => i.BaseItem.ID == item.ID))
                                {
                                    location.ItemsHere.First(i => i.BaseItem.ID == item.ID).Quantity++;
                                }
                                else
                                {
                                    location.ItemsHere.Add(new ItemQuantity(ItemFactory.GetItemByID(item.ID), 1));
                                }
                            }
                        }
                    }
                }*/


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

    }
}
