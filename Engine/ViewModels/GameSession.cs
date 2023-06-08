using Engine.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Engine.Factories;
using StoryRPG;
using System.Collections.ObjectModel;

namespace Engine.ViewModels
{
    public class GameSession : BaseNotificationClass
    {
        public event EventHandler<GameMessageEventArgs> OnMessageRaised;
        public event EventHandler<OnEncounterEventArgs> OnEncounterEngaged;
        public event EventHandler OnInventoryOpened;
        public event EventHandler OnCharacterOpened;
        public event EventHandler<OnTradeEventArgs> OnTradeInitiated;

        #region Private Variables
        private Player _currentPlayer;
        private Location _currentLocation;
        private Encounter _currentEncounter;
        private Merchant _currentMerchant;

        #endregion

        #region Public Variables
        public World CurrentWorld { get; }
        public Player CurrentPlayer
        {
            get { return _currentPlayer; }
            set
            {
                if (_currentPlayer != null)
                {
                    _currentPlayer.OnActionPerformed -= OnPlayerAction;
                    _currentPlayer.OnKilled -= OnCurrentPlayerKilled;
                }
                _currentPlayer = value;
                if (_currentPlayer != null)
                {
                    _currentPlayer.OnActionPerformed += OnPlayerAction;
                    _currentPlayer.OnKilled += OnCurrentPlayerKilled;
                }
            }
        }
        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasNorthExit));
                OnPropertyChanged(nameof(HasSouthExit));
                OnPropertyChanged(nameof(HasEastExit));
                OnPropertyChanged(nameof(HasWestExit));
                OnPropertyChanged(nameof(HasUpExit));
                OnPropertyChanged(nameof(HasDownExit));
                OnPropertyChanged(nameof(NorthExit));
                OnPropertyChanged(nameof(SouthExit));
                OnPropertyChanged(nameof(EastExit));
                OnPropertyChanged(nameof(WestExit));
                OnPropertyChanged(nameof(UpExit));
                OnPropertyChanged(nameof(DownExit));
                GetEncounterAtLocation();
            }
        }
        public Encounter CurrentEncounter
        {
            get { return _currentEncounter; }
            set
            {
                if ( _currentEncounter != null)
                {
                    foreach (Monster monster in CurrentEncounter.Monsters)
                    {
                        monster.OnActionPerformed -= OnMonsterAction;
                        monster.OnKilled -= OnMonsterKilled;
                    }
                }
                _currentEncounter = value;
                if ( _currentEncounter != null)
                {
                    RaiseMessage("");
                    RaiseMessage($"You encounter a {CurrentEncounter.Name}");
                    foreach(Monster monster in CurrentEncounter.Monsters)
                    {
                        monster.OnActionPerformed += OnMonsterAction;
                        monster.OnKilled += OnMonsterKilled;
                    }
                }
                EncounterWatch();
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasEncounter));
            }
        }
        public Merchant CurrentMerchant
        {
            get { return _currentMerchant; }
            set
            {
                _currentMerchant = value;
                OnPropertyChanged();
                DetermineItemValues();
                TradeWatch();
            }
        }
        public Trade CurrentTrade { get; set; }

        public bool HasNorthExit => 
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1, CurrentLocation.ZCoordinate) != null;
        public bool HasSouthExit =>
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1, CurrentLocation.ZCoordinate) != null;
        public bool HasEastExit =>
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate, CurrentLocation.ZCoordinate) != null;
        public bool HasWestExit =>
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate, CurrentLocation.ZCoordinate) != null;
        public bool HasUpExit => 
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate, CurrentLocation.ZCoordinate + 1) != null;
        public bool HasDownExit =>
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate, CurrentLocation.ZCoordinate - 1) != null;
     
        public string NorthExit => 
            HasNorthExit ? CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1, CurrentLocation.ZCoordinate).Name : "";
        public string SouthExit => 
            HasSouthExit ? CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1, CurrentLocation.ZCoordinate).Name : "";
        public string EastExit => 
            HasEastExit ? CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate, CurrentLocation.ZCoordinate).Name : "";
        public string WestExit => 
            HasWestExit ? CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate, CurrentLocation.ZCoordinate).Name : "";
        public string UpExit =>
            HasUpExit ? CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate, CurrentLocation.ZCoordinate + 1).Name : "";
        public string DownExit =>
            HasDownExit ? CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate, CurrentLocation.ZCoordinate - 1).Name : "";
        public bool HasEncounter => CurrentEncounter != null;
        public bool AreAllMonstersDead => CurrentEncounter.Monsters.All(m => m.IsDead == true);
        #endregion

        #region Constructor
        public GameSession()
        {
            CurrentPlayer = new Player("Laughing Zebra", "Human", "Druid", 100, 100, "A Human", 0, 10, 1, 1, 1, 1, 1, 1, 1, 1, 1);
            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(2001));
            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(9001));
            
            CurrentPlayer.EquippedWeapon = (ItemFactory.CreateGameItem(1001));

            CurrentTrade = new Trade(new ObservableCollection<ItemQuantity>(), new ObservableCollection<ItemQuantity>());
            CurrentWorld = WorldFactory.CreateWorld();

            CurrentLocation = CurrentWorld.LocationAt(0, 0, 0);
        }

        #endregion
        public void RaiseMessage(string message)
        {
            OnMessageRaised?.Invoke(this, new GameMessageEventArgs(message));
            //if there is anything subscribed to OnMessageRaised, pass in itself and GameMessageEventArgs with the message
        }

        #region Movement Functions
        public void MoveNorth()
        {
            if (HasNorthExit)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1, CurrentLocation.ZCoordinate);
            }
            else
            {
                RaiseMessage("There is no exit that way");
            }
        }
        public void MoveSouth()
        {
            if (HasSouthExit)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1, CurrentLocation.ZCoordinate);
            }
            else
            {
                RaiseMessage("There is no exit that way");
            }
        }
        public void MoveEast()
        {
            if (HasEastExit)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate, CurrentLocation.ZCoordinate);
            }
            else
            {
                RaiseMessage("There is no exit that way");
            }
        }
        public void MoveWest()
        {
            if (HasWestExit)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate, CurrentLocation.ZCoordinate);
            }
            else
            {
                RaiseMessage("There is no exit that way");
            }
        }
        public void MoveUp()
        {
            if (HasUpExit)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate, CurrentLocation.ZCoordinate + 1);
            }
            else
            {
                RaiseMessage("There is no exit that way");
            }
        }
        public void MoveDown()
        {
            if (HasDownExit)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate, CurrentLocation.ZCoordinate - 1);
            }
            else
            {
                RaiseMessage("There is no exit that way");
            }
        }
        #endregion

        #region Encounter Functions
        public void EncounterWatch()
        {
            OnEncounterEngaged?.Invoke(this, new OnEncounterEventArgs(CurrentEncounter));

        }
        public void OnMonsterAction(object sender, string result)
        {
            RaiseMessage(result);
        }
        private void OnMonsterKilled(object sender, System.EventArgs eventArgs)
        {
           
            /*
            RaiseMessage("");
            RaiseMessage($"You defeated the {CurrentMonster.Name}!");
            RaiseMessage($"You receive {CurrentMonster.Experience} experience points.");
            CurrentPlayer.AddExperience(CurrentMonster.Experience);

            RaiseMessage($"You receive {CurrentMonster.Cats} cats.");
            CurrentPlayer.ReceiveGold(CurrentMonster.Cats);

            foreach (ItemQuantity gameItem in CurrentMonster.Inventory)
            {
                RaiseMessage($"You receive {gameItem.Quantity} {gameItem.BaseItem.Name}.");
                for (int i = 0; i <= gameItem.Quantity; i++)
                {
                    CurrentPlayer.AddItemToInventory(gameItem.BaseItem);
                }
            }
            */
        }
        private void OnEncounterEnd()
        {
            foreach (Monster monster in CurrentEncounter.Monsters)
            {
                RaiseMessage("");
                foreach (ItemQuantity item in monster.Inventory)
                {
                    RaiseMessage($"You recieve {item.Quantity} {item.BaseItem.Name}.");
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        CurrentPlayer.AddItemToInventory(item.BaseItem);
                    }
                }
            }
                    int totalXp = CurrentEncounter.Monsters.Sum(e => e.Experience);
                    RaiseMessage($"You recieve {totalXp} experience.");
                    CurrentPlayer.AddExperience(totalXp);

                    int totalCats = CurrentEncounter.Monsters.Sum(e => e.Cats);
                    if (totalCats > 0)
                    {
                        RaiseMessage($"You recieve {totalCats} cats.");
                        CurrentPlayer.ReceiveGold(totalCats);
                    }
        }
        private void GetEncounterAtLocation()
        {
            CurrentEncounter = CurrentLocation.GetEncounter();
        }

        #endregion

        #region Player Functions
        private void OnCurrentPlayerKilled(object sender, System.EventArgs eventArgs)
        {
            RaiseMessage("");
            RaiseMessage("You have been killed.");
            CurrentLocation = CurrentWorld.LocationAt(0, 0, 0);
            CurrentPlayer.FullHeal();
        }
        public void OnPlayerAction(object sender, string result)
        {
            RaiseMessage(result);
        }
        public void WindowToSession(string aString)
        {
            RaiseMessage("");
            RaiseMessage(aString);
            if (CurrentMerchant != null)
                DoInTrade(aString);
            else if (CurrentEncounter != null)
                DoInCombat(aString);
            else
                Do(aString);
        }
        #endregion

        #region Player Action Functions

        #region Do
        public void DoInCombat(string aString)
        {
            if (aString == "")
                return;
            string verb = "";
            string noun = "";

            if (aString.IndexOf(" ") > 0)
            {
                string[] temp = aString.Split(new char[] { ' ' }, 2);
                verb = temp[0].ToLower();
                noun = temp[1].ToLower();
            }
            else
            {
                verb = aString.ToLower();
            }

            switch (verb)
            {
                case "?":
                case "help":
                    //WriteCommands();
                    break;
                case "examine":
                    Examine(noun);
                    break;
                case "save":
                    //SaveGame.Save();
                    break;
                case "bag":
                case "inventory":
                    Inventory();
                    break;
                case "character":
                case "char":
                    Character();
                    break;
                case "use":
                    //Use(noun);
                    break;
                case "quit":
                    Program.GameState = Program.GameStates.Quit;
                    break;
                case "fullheal":
                    CurrentPlayer.FullHeal();
                    break;
                case "attack":
                    Attack(noun);
                    break;
                case "equip":
                    Equip(noun);
                    break;
                case "unequip":
                    Unequip(noun);
                    break;
                case "flee":
                    Flee();
                    break;
                default:
                    RaiseMessage($"You cannot do that now");
                    break;
            }
        }
        public void DoInTrade(string aString)
        {
            if (aString == "")
                return;
            string verb = "";
            string noun = "";
            string splitNoun = "";
            string tempNum = "";
            int num = 1;

            if (aString.IndexOf(" ") > 0) //split the string into two sections, action and what's to be acted on
            {
                string[] temp = aString.Split(new char[] { ' ' }, 2);
                verb = temp[0].ToLower();
                noun = temp[1].ToLower();
            }
            else
            {
                verb = aString.ToLower();
            }

            if (noun.IndexOf(" ") > 0) //split to be acted on into two sections and determine if a number is involved
            {
                string[] temp = noun.Split(new char[] { ' ' }, 2);
                tempNum = temp[0].ToLower();
                splitNoun = temp[1].ToLower();
                bool parse = int.TryParse(tempNum, out num); //try to convert tempNum to int, it it doesn't convert, num = 1
                if (!parse)
                {
                    num = 1; //if number doesn't parse, noun remains the same
                }
                else
                {
                    noun = splitNoun.ToLower(); //if the number has parsed, noun becomes splitNoun
                }
            }


            switch (verb)
            {
                case "?":
                case "help":
                    //WriteCommands();
                    break;
                case "examine":
                    Examine(noun);
                    break;
                case "save":
                    //SaveGame.Save();
                    break;
                case "bag":
                case "inventory":
                    Inventory();
                    break;
                case "character":
                case "char":
                    Character();
                    break;
                case "buy":
                    Buy(noun, num);
                    break;
                case "sell":
                    Sell(noun, num);
                    break;
                case "removebuy":
                    RemoveBuy(noun, num);
                    break;
                case "removesell":
                    RemoveSell(noun, num);
                    break;
                case "exchange":
                    Exchange();
                    break;
                case "quit":
                    Program.GameState = Program.GameStates.Quit;
                    break;
                case "close":
                    CancelTrade();
                    break;
                case "clear":
                case "reset":
                    ClearTrade();
                    break;
                case "tags":
                    if (CurrentMerchant != null)
                    {
                        RaiseMessage("Preferred");
                        foreach (Item.ItemProperties tag in CurrentMerchant.PreferredItems)
                            RaiseMessage($"{tag.ToString()}");
                        RaiseMessage("Disliked");
                        foreach (Item.ItemProperties tag in CurrentMerchant.DislikedItems)
                            RaiseMessage($"{tag.ToString()}");
                    }
                    break;
                default:
                    RaiseMessage($"{aString} is not a valid command");
                    break;
            }
        }
        public void Do(string aString)
        {
            if (aString == "")
                return;
            string verb = "";
            string noun = "";

            if (aString.IndexOf(" ") > 0)
            {
                string[] temp = aString.Split(new char[] { ' ' }, 2);
                verb = temp[0].ToLower();
                noun = temp[1].ToLower();
            }
            else
            {
                verb = aString.ToLower();
            }

            switch (verb)
            {
                case "?":
                case "help":
                    //WriteCommands();
                    break;
                case "take":
                case "pickup":
                    Pickup(noun);
                    break;
                case "examine":
                    Examine(noun);
                    break;
                case "eat":
                    //Eat(noun);
                    break;
                case "drink":
                    // Drink(noun);
                    break;
                case "goto":
                case "move":
                case "go":
                    MoveTo(noun);
                    break;
                case "save":
                    //SaveGame.Save();
                    break;
                case "bag":
                case "inventory":
                    Inventory();
                    break;
                case "character":
                case "char":
                    Character();
                    break;
                case "use":
                    //Use(noun);
                    break;
                case "drop":
                    Drop(noun);
                    break;
                case "quit":
                    Program.GameState = Program.GameStates.Quit;
                    break;
                case "fullheal":
                    CurrentPlayer.FullHeal();
                    break;
                case "equip":
                    Equip(noun);
                    break;
                case "unequip":
                    Unequip(noun);
                    break;
                case "trade":
                    Trade(noun);
                    break;
                default:
                    RaiseMessage($"You cannot do that now");
                    break;
            }
        }
        #endregion

        public void Examine(string aString)
        {
            int itemID = ItemFactory.ItemID(aString);
            Item item = ItemFactory.GetItem(itemID);

            if (item != null)
            {
                string tags = "";
                foreach (Item.ItemProperties properties in item.Properties)
                {
                    tags += $", {properties.ToString()}";
                }

                 RaiseMessage($"{item.Description}, TV: {item.Value}\n Tags: {tags}");
                RaiseMessage("");
            }
            else
            {
                RaiseMessage($"{aString} is not a valid item");
            }
        }
        public void Pickup(string aString)
        {
            int itemID = ItemFactory.ItemID(aString);
            if (itemID != default)
            {
                RaiseMessage($"You take the {ItemFactory.ItemName(itemID)}.");
                CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(itemID));
            }
            else
                RaiseMessage($"{aString} is not here.");
        }
        public void Drop(string aString)
        {
            Item item = CurrentPlayer.GetItemFromInventory(aString);
            if (item != default)
            {
                RaiseMessage($"You drop the {item.Name}.");
                CurrentPlayer.RemoveItemFromInventory(item);
            }
            else
            {
                RaiseMessage($"You do not have any {aString}.");
            }

        }
        public void MoveTo(string aString)
        {
            switch (aString.ToLower())
            {
                case "north":
                    MoveNorth();
                    break;
                case "south":
                    MoveSouth();
                    break;
                case "east":
                    MoveEast();
                    break;
                case "west":
                    MoveWest();
                    break;
                case "up":
                    MoveUp();
                    break;
                case "down":
                    MoveDown();
                    break;
                default:
                    break;
            }
        }
        public void Inventory()
        {
            OnInventoryOpened?.Invoke(this, System.EventArgs.Empty);
        }
        public void Character()
        {
            OnCharacterOpened?.Invoke(this, System.EventArgs.Empty);
        }
        public void Equip(string aString)
        {
            foreach(ItemQuantity item in CurrentPlayer.Inventory.ToList())
            {
                if(item.BaseItem.Name.ToLower() == aString.ToLower())
                {
                    if (item.BaseItem.Properties.Contains(Item.ItemProperties.Weapon))
                    {
                        if (CurrentPlayer.EquippedWeapon != null)
                        {
                            CurrentPlayer.RemoveItemFromInventory(item.BaseItem);
                            CurrentPlayer.AddItemToInventory(CurrentPlayer.EquippedWeapon);

                            CurrentPlayer.EquippedWeapon = item.BaseItem;
                            return;
                        }
                        else
                        {
                            CurrentPlayer.RemoveItemFromInventory(item.BaseItem);

                            CurrentPlayer.EquippedWeapon = item.BaseItem;
                            return;
                        }
                    }
                    else
                    {
                        RaiseMessage($"{aString} is not a weapon.");
                        return;
                    }
                }
            }
            RaiseMessage($"You do not have a {aString}.");
        }
        public void Unequip(string aString)
        {
            if(CurrentPlayer.EquippedWeapon != null )
            {
                if(CurrentPlayer.EquippedWeapon.Name.ToLower() == aString.ToLower())
                {
                    CurrentPlayer.AddItemToInventory(CurrentPlayer.EquippedWeapon);

                    CurrentPlayer.EquippedWeapon = null;
                }
                else
                {
                    RaiseMessage($"{aString} is not equipped.");
                }
            }
            else
            {
                RaiseMessage("There is nothing to unequip");
            }
        }

        #endregion

        #region Combat Functions
        private Monster DetermineTarget(string aString)
        {
            foreach(Monster monster in CurrentEncounter.Monsters)
            {
                if (aString == monster.Name.ToLower())
                {
                    return monster;
                }
            }
            return null;
        }
        public void Attack(string noun)
        {
            if (CurrentEncounter == null)
            {
                RaiseMessage("There is nothing to attack");
                return;
            }
            if (CurrentPlayer.EquippedWeapon == null)
            {
                RaiseMessage("You need a weapon equipped to attack");
                return;
            }

            Monster target = DetermineTarget(noun);
            if (target != null)
            {
                RaiseMessage("");
                CurrentPlayer.Attack(target);
                RaiseMessage("");
            }
            else
            {
                RaiseMessage($"{noun} is not a valid target");
                return;
            }

            foreach (Monster monster in CurrentEncounter.Monsters)
            {
                if (!monster.IsDead)
                {
                    monster.Attack(CurrentPlayer);
                }
            }
            if (AreAllMonstersDead)
            {
                OnEncounterEnd();
                CurrentEncounter = null;
            }

        }
        public void Flee()
        {
            RaiseMessage($"You flee from the {CurrentEncounter.Name}");
            CurrentEncounter = null;
        }

        #endregion

        #region Trade Functions
        public void TradeWatch()
        {
            OnTradeInitiated?.Invoke(this, new OnTradeEventArgs(CurrentTrade));
        }
        public void Trade(string aString)
        {
            if (CurrentLocation.MerchantsHere.Any(m => m.Name.ToLower() == aString.ToLower()))
            {
                CurrentMerchant = MerchantFactory.GetMerchant(aString);
            }
            else
            {
                RaiseMessage($"Merchant {aString} is not here");
            }
            
        }
        public void Buy(string aString, int num)
        {
            if (CurrentMerchant != null) //if a trade is in progress
            {
                if (ItemFactory.IsItem(aString)) //if the item exists
                {
                    if (CurrentMerchant.HasItem(aString)) //if the merchant has the item stocked
                    {
                        Item item = CurrentMerchant.GetItemFromInventory(aString);
                        int quantity = CurrentMerchant.Inventory.FirstOrDefault(i => i.BaseItem.Name.ToLower() == item.Name.ToLower()).Quantity;
                        if(num > quantity)
                            num = quantity;

                        for (int i = 0; i < num; i++)
                        {
                            CurrentTrade.AddItemToBuyList(item);
                            CurrentMerchant.RemoveItemFromInventory(item);
                        }
                    }
                    else
                    {
                        RaiseMessage($"{CurrentMerchant.Name} does not have {aString}");
                    }
                }
                else
                {
                    RaiseMessage($"Item {aString} does not exist");
                }
            }
            else
            {
                RaiseMessage("You are not trading with a merchant");
            }
        }
        public void Sell(string aString, int num)
        {
            if (CurrentMerchant != null)
            {
                if (ItemFactory.IsItem(aString))
                {
                    if (CurrentPlayer.HasItem(aString))
                    {
                        Item item = CurrentPlayer.GetItemFromInventory(aString);
                        int quantity = CurrentPlayer.Inventory.FirstOrDefault(i => i.BaseItem.Name.ToLower() == item.Name.ToLower()).Quantity;
                        if (num > quantity)
                            num = quantity;

                        for (int i = 0; i < num; i++)
                        {
                            CurrentTrade.AddItemToSellList(item);
                            CurrentPlayer.RemoveItemFromInventory(item);
                        }

                    }
                    else
                    {
                        RaiseMessage($"you do not have {aString}");
                    }
                }
                else
                {
                    RaiseMessage($"Item {aString} does not exist");
                }
            }
            else
            {
                RaiseMessage("You are not trading with a merchant");
            }
        }
        public void RemoveBuy(string aString, int num)
        {
            if (CurrentMerchant != null) //if a trade is in progress
            {
                if (ItemFactory.IsItem(aString)) //if the item exists
                {
                    if (CurrentTrade.BuyHasItem(aString)) //if ToBuy has the item stocked
                    {
                        Item item = CurrentTrade.GetItemFromBuyList(aString);
                        int quantity = CurrentTrade.ToBuyInventory.FirstOrDefault(i => i.BaseItem.Name.ToLower() == item.Name.ToLower()).Quantity;
                        if (num > quantity)
                            num = quantity;

                        for (int i = 0; i < num; i++)
                        {
                            CurrentTrade.RemoveItemFromBuyList(item);
                            CurrentMerchant.AddItemToInventory(item);
                        }
                    }
                    else
                    {
                        RaiseMessage($"{CurrentMerchant.Name} does not have {aString}");
                    }
                }
                else
                {
                    RaiseMessage($"Item {aString} does not exist");
                }
            }
            else
            {
                RaiseMessage("You are not trading with a merchant");
            }
        }
        public void RemoveSell(string aString, int num) 
        {
            if (CurrentMerchant != null) //if a trade is in progress
            {
                if (ItemFactory.IsItem(aString)) //if the item exists
                {
                    if (CurrentTrade.SellHasItem(aString)) //if ToBuy has the item stocked
                    {
                        Item item = CurrentTrade.GetItemFromSellList(aString);
                        int quantity = CurrentTrade.ToSellInventory.FirstOrDefault(i => i.BaseItem.Name.ToLower() == item.Name.ToLower()).Quantity;
                        if (num > quantity)
                            num = quantity;

                        for (int i = 0; i < num; i++)
                        {
                            CurrentTrade.RemoveItemFromSellList(item);
                            CurrentPlayer.AddItemToInventory(item);
                        }
                    }
                    else
                    {
                        RaiseMessage($"you have not offered {aString}");
                    }
                }
                else
                {
                    RaiseMessage($"Item {aString} does not exist");
                }
            }
            else
            {
                RaiseMessage("You are not trading with a merchant");
            }
        }
        public void ClearTrade()
        {
            foreach (ItemQuantity item in CurrentTrade.ToBuyInventory.ToList())
            {
                int num = item.Quantity - 1;
                for (int i = 0; i <= num; i++)
                {
                    CurrentMerchant.AddItemToInventory(item.BaseItem);
                    CurrentTrade.RemoveItemFromBuyList(item.BaseItem);
                }
            }
            foreach (ItemQuantity item in CurrentTrade.ToSellInventory.ToList())
            {
                int num = item.Quantity - 1;
                for (int i = 0; i <= num; i++)
                {
                    CurrentPlayer.AddItemToInventory(item.BaseItem);
                    CurrentTrade.RemoveItemFromSellList(item.BaseItem);
                }
            }
        }
        public void Exchange()
        {
            if (CurrentTrade.TotalBuyValue <= CurrentTrade.TotalSellValue)
            {
                foreach(ItemQuantity item in CurrentTrade.ToBuyInventory.ToList())
                {
                    int num = item.Quantity - 1;
                    for (int i = 0; i <= num; i++)
                    {
                        CurrentTrade.RemoveItemFromBuyList(item.BaseItem);
                        CurrentPlayer.AddItemToInventory(item.BaseItem);
                    }
                }
                foreach(ItemQuantity item in CurrentTrade.ToSellInventory.ToList())
                {
                    int num = item.Quantity - 1;
                    for (int i = 0; i <= num; i++)
                    {
                        CurrentTrade.RemoveItemFromSellList(item.BaseItem);
                        CurrentMerchant.AddItemToInventory(item.BaseItem);
                    }

                }
            }
            else
            {
                RaiseMessage("The merchant does not accept your offer");
            }
        }
        public void CancelTrade()
        {
            if (CurrentTrade != null)
            {

                foreach (ItemQuantity item in CurrentTrade.ToBuyInventory.ToList())
                {
                    int num = item.Quantity -1;
                    for (int i = 0; i <= num; i++)
                    {
                        CurrentMerchant.AddItemToInventory(item.BaseItem);
                        CurrentTrade.RemoveItemFromBuyList(item.BaseItem);
                    }
                }
                foreach (ItemQuantity item in CurrentTrade.ToSellInventory.ToList())
                {
                    int num = item.Quantity -1;
                    for (int i = 0; i <= num; i++)
                    {
                        CurrentPlayer.AddItemToInventory(item.BaseItem);
                        CurrentTrade.RemoveItemFromSellList(item.BaseItem);
                    }
                }
            }
            else
            {
                RaiseMessage("There is no trade.");
            }
            CurrentMerchant = null;
        }
        public void DetermineItemValues()
        {
            if (CurrentMerchant != null)
            {
                double markup = (CurrentMerchant.Markup / 100d) + 1;
                double preferredMarkup = 1.4d;
                double dislikedMarkdown = 0.6d;

                foreach (ItemQuantity item in CurrentMerchant.Inventory)
                {
                    double value = item.BaseItem.ActualValue * markup;
                    item.BaseItem.Value = Convert.ToInt32(value);
                    OnPropertyChanged();
                }

                foreach(ItemQuantity item in CurrentPlayer.Inventory)
                {
                    bool PreferredMatch = item.BaseItem.Properties.Any(i => CurrentMerchant.PreferredItems.Any(p => p == i));
                    bool DislikedMatch = item.BaseItem.Properties.Any(i => CurrentMerchant.DislikedItems.Any(d => d == i));

                    if (PreferredMatch)
                    {
                        double value = item.BaseItem.ActualValue * preferredMarkup;
                        item.BaseItem.Value = Convert.ToInt32(value);
                        OnPropertyChanged();
                    }
                    else if (DislikedMatch)
                    {
                        double value = item.BaseItem.ActualValue * dislikedMarkdown;
                        item.BaseItem.Value = Convert.ToInt32(value);
                        OnPropertyChanged();
                    }
                    else
                    {
                        item.BaseItem.Value = item.BaseItem.ActualValue;
                        OnPropertyChanged();
                    }
                }
            }
        }

        #endregion

    }
}
