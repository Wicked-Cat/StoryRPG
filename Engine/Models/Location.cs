using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Engine.Factories;
using Engine.Service;

namespace Engine.Models
{
    public class Location : BaseNotificationClass
    {
        #region Private Variables
        private string _encounterText;
        private string _merchantText;
        private string _itemText;
        #endregion

        #region Public Variables
        public int XCoordinate { get; set; }
        public int YCoordinate { get; set; }
        public int ZCoordinate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Encounter> EncountersHere { get; set; } = new List<Encounter>();
        public List<EncounterPercent> AllEncountersHere { get; set; } = new List<EncounterPercent>();
        public List<Merchant> MerchantsHere { get; set; } = new List<Merchant>();
        public List<MerchantPercent> AllMerchantsHere { get; set; } = new List<MerchantPercent>();
        public List<ItemQuantity> ItemsHere { get; set; } = new List<ItemQuantity>();
        public List<LocationItems> AllItemsHere { get; set; } = new List<LocationItems>();
        public Challenge ChallengeHere { get; set; }
        public string EncountersText
        {
            get { return WriteEncounterText(); }
            set { _encounterText = WriteEncounterText(); OnPropertyChanged();  }
        }
        public string MerchantsText
        {
            get { return WriteMerchantText(); }
            set { _merchantText = WriteMerchantText(); OnPropertyChanged(); }
        }

        public string ItemsText 
        {
            get { return WriteItemText(); }
            set { _itemText = WriteItemText(); OnPropertyChanged(); }
        }

        public enum LandSeaSky { Land, Sea, Sky }
        public enum Depth { Underground, Aboveground }
        public enum Shelter { Interior, Exterior }
        #endregion

        public Location(
            int xCoordinate,
            int yCoordinate,
            int zCoordinate,
            string name,
            string description) 
        {
            XCoordinate = xCoordinate;
            YCoordinate = yCoordinate;
            ZCoordinate = zCoordinate;
            Name = name;
            Description = description;
        }

        #region Creation Functions
        public void AddEncounter(int encounterID, int chanceOfEncounter)
        {
            if(AllEncountersHere.Exists(e => e.EncounterID == encounterID))
            {
                //if the monster has already been added to this locationn, overwrite chanceOfEncounter
                AllEncountersHere.First(m => m.EncounterID == encounterID).ChanceOfEncounter = chanceOfEncounter;
            }
            else
            {
                //if the monster is not already at this location, add it
                AllEncountersHere.Add(new EncounterPercent(encounterID, chanceOfEncounter));
            }
        }
        public void AddMerchant(int merchantID, int chanceOfEncounter)
        {
            if(AllMerchantsHere.Exists(m =>m.MerchantID == merchantID))
            {
                AllMerchantsHere.First(m => m.MerchantID == merchantID).ChanceOfEncounter = chanceOfEncounter;
            }
            else
            {
                AllMerchantsHere.Add(new MerchantPercent(merchantID, chanceOfEncounter));
            }
        }
        public void AddItems(int itemID, int percent, int quantity, bool respawns)
        {

            if(AllItemsHere.Exists(i => i.ID == itemID))
            {
                AllItemsHere.First(i => i.ID == itemID).Quantity++;
            }
            else
            {
                    AllItemsHere.Add(new LocationItems(itemID, percent, quantity, respawns));

            }
        }
        #endregion 

        public void AddItemToLocation(Item item)
        {
            if (!ItemsHere.Any(i => i.BaseItem.ID == item.ID))
            {
                  ItemsHere.Add(new ItemQuantity(item, 0));
            }
            ItemsHere.First(i => i.BaseItem.ID == item.ID).Quantity++;
            ItemsText = WriteItemText();
        }
        public void RemoveItemFromLocation(Item item)
        {
            ItemQuantity itemToRemove = ItemsHere.FirstOrDefault(i => i.BaseItem.Name == item.Name);
            if (itemToRemove != null)
            {
                AllItemsHere.First(i => i.ID == itemToRemove.BaseItem.ID).HasBeenCollected = true;

                if (itemToRemove.Quantity == 1)
                {
                    ItemsHere.Remove(itemToRemove);
                }
                else
                {
                    itemToRemove.Quantity--;
                }
            }
            ItemsText = WriteItemText();
        }
        public Encounter GetEncounter(string aString)
        {
            if (EncountersHere.Any(e => e.Name.ToLower() == aString.ToLower()))
                return EncountersHere.First(e => e.Name.ToLower() == aString.ToLower());
            else
            {
                return null;
            }

            return null;
            /*
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
            return EncounterFactory.GetEncounter(EncountersHere.Last().EncounterID);*/
        }
       

        #region Text Functions
        public string WriteEncounterText()
        {
            string text = "";
            foreach(Encounter encounter in EncountersHere)
            {
                text = $"{text} \n {encounter.Name}";
            }
            return text;
        }

        public string WriteMerchantText()
        {
            string text = "";
            foreach(Merchant merchant in MerchantsHere)
            {
                text = $"{text} \n {merchant.Name}";
            }
            return text;
        }

        public string WriteItemText()
        {
            string text = "";
            foreach(ItemQuantity item in ItemsHere)
            {
                text = $"{text} \n {item.Quantity} {item.BaseItem.Name}";
            }
            return text;
        }
        #endregion
    }
}
