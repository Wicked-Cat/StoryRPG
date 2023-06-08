using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Trade : BaseNotificationClass
    {
        #region Private Variables
        private ObservableCollection<ItemQuantity> _toSellInventory;
        private ObservableCollection<ItemQuantity> _toBuyInventory;

        #endregion

        #region Public Variables
        public ObservableCollection<ItemQuantity> ToSellInventory
        {
            get { return _toSellInventory; }
            set
            {
                _toSellInventory = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ItemQuantity> ToBuyInventory
        {
            get { return _toBuyInventory; }
            set
            {
                _toBuyInventory = value;
                OnPropertyChanged();
            }
        }
        public int TotalSellValue =>
            ToSellInventory.Sum(x => x.BaseItem.Value * x.Quantity);
        public int TotalBuyValue =>
            ToBuyInventory.Sum(x => x.BaseItem.Value * x.Quantity);

        #endregion

        public Trade(ObservableCollection<ItemQuantity> toSellInventory, ObservableCollection<ItemQuantity> toBuyInventory)
        {
            ToSellInventory = toSellInventory;
            ToBuyInventory = toBuyInventory;
        }


        #region Inventory Functions

        public void AddItemToBuyList(Item item)
        {
            if (item.IsUnique)
            {
                ToBuyInventory.Add(new ItemQuantity(item, 1));
            }
            else
            {
                if (!ToBuyInventory.Any(i => i.BaseItem.ID == item.ID))
                {
                    ToBuyInventory.Add(new ItemQuantity(item, 0));
                }
                ToBuyInventory.First(i => i.BaseItem.ID == item.ID).Quantity++;
            }
            OnPropertyChanged(nameof(TotalBuyValue));
        }
        public void RemoveItemFromBuyList(Item item)
        {
            ItemQuantity itemToRemove = ToBuyInventory.FirstOrDefault(i => i.BaseItem == item);
            if (itemToRemove != null)
            {
                if (itemToRemove.Quantity == 1)
                {
                    ToBuyInventory.Remove(itemToRemove);
                }
                else
                {
                    itemToRemove.Quantity--;
                }
            }
            OnPropertyChanged(nameof(TotalBuyValue));
        }
        public void AddItemToSellList(Item item)
        {
            if (item.IsUnique)
            {
                ToSellInventory.Add(new ItemQuantity(item, 1));
            }
            else
            {
                if (!ToSellInventory.Any(i => i.BaseItem.ID == item.ID))
                {
                    ToSellInventory.Add(new ItemQuantity(item, 0));
                }
                ToSellInventory.First(i => i.BaseItem.ID == item.ID).Quantity++;
            }
            OnPropertyChanged(nameof(TotalSellValue));
        }
        public void RemoveItemFromSellList(Item item)
        {
            ItemQuantity itemToRemove = ToSellInventory.FirstOrDefault(i => i.BaseItem == item);
            if (itemToRemove != null)
            {
                if (itemToRemove.Quantity == 1)
                {
                    ToSellInventory.Remove(itemToRemove);
                }
                else
                {
                    itemToRemove.Quantity--;
                }
            }
            OnPropertyChanged(nameof(TotalSellValue));
        }
        public Item GetItemFromBuyList(string aString)
        {
            Item item = ToBuyInventory.FirstOrDefault(i => i.BaseItem.Name.ToLower() == aString.ToLower())?.BaseItem ?? default;
            return item;
        }
        public Item GetItemFromSellList(string aString)
        {
            Item item = ToSellInventory.FirstOrDefault(i => i.BaseItem.Name.ToLower() == aString.ToLower())?.BaseItem ?? default;
            return item;
        }
        public bool BuyHasItem(string aString)
        {
            if (ToBuyInventory.Any(i => i.BaseItem.Name.ToLower() == aString.ToLower()))
                return true;

            return false;
        }
        public bool SellHasItem(string aString)
        {
            if (ToSellInventory.Any(i => i.BaseItem.Name.ToLower() == aString.ToLower()))
                return true;

            return false;
        }
        #endregion
    }
}
