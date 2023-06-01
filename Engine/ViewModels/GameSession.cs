using Engine.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Engine.Factories;
using StoryRPG;

namespace Engine.ViewModels
{
    public class GameSession : BaseNotificationClass
    {
        public event EventHandler<GameMessageEventArgs> OnMessageRaised;
        public event EventHandler<OnEncounterEventArgs> OnEncounterEngaged;

        #region Private Variables
        private Player _currentPlayer;
        private Location _currentLocation;
        private Monster _currentMonster;
        private Monster _currentMonster2;
        private Monster _currentMonster3;
        private Monster _currentMonster4;
        private Monster _currentMonster5;
        private Encounter _currentEncounter;
        private List<Monster> _currentMonsters;

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
                        if(monster.HasAction == false)
                        monster.OnActionPerformed += OnMonsterAction;
                        monster.OnKilled += OnMonsterKilled;
                    }
                }
                EncounterWatch();
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasEncounter));
            }
        }

        public bool HasNorthExit => 
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1) != null;
        public bool HasSouthExit =>
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1) != null;
        public bool HasEastExit =>
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate) != null;
        public bool HasWestExit =>
            CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate) != null;
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

            CurrentWorld = WorldFactory.CreateWorld();

            CurrentLocation = CurrentWorld.LocationAt(0, 0);
        }

        #endregion
        public void RaiseMessage(string message)
        {
            OnMessageRaised?.Invoke(this, new GameMessageEventArgs(message));
            //if there is anything subscribed to OnMessageRaised, pass in itself and GameMessageEventArgs with the message
        }

        public void EncounterWatch()
        {
            OnEncounterEngaged?.Invoke(this, new OnEncounterEventArgs(CurrentEncounter));

        }

        #region Movement Functions
        public void MoveNorth()
        {
            if (HasNorthExit)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1);
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
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1);
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
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate);
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
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate);
            }
            else
            {
                RaiseMessage("There is no exit that way");
            }
        }
        #endregion

        #region Encounter Functions
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
            CurrentLocation = CurrentWorld.LocationAt(0, 0);
            CurrentPlayer.FullHeal();
        }
        #endregion

        #region Player Action Functions
        public void OnPlayerAction(object sender, string result)
        {
            RaiseMessage(result);
        }
        public void WindowToSession(string aString)
        {
            Do(aString);
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
                    //Examine(noun);
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

                    break;
                case "character":
                case "char":

                    break;
                case "use":
                    //Use(noun);
                    break;
                case "drop":
                    Drop(noun);
                    break;
                case "buy":
                    //Buy(noun);
                    break;
                case "sell":
                    //Sell(noun);
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
                case "loot":
                    foreach(Monster monster in CurrentEncounter.Monsters)
                    {
                        RaiseMessage($"Count: {monster.Inventory.Count()}");
                        foreach(ItemQuantity item in monster.Inventory)
                        {
                            RaiseMessage($" {item.Quantity} {item.BaseItem.Name}");
                        }
                    }
                    break;
                default:
                    RaiseMessage($"{aString} is not a valid command");
                    break;
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
                default:
                    break;
            }
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
        public void Equip(string aString)
        {
            foreach(ItemQuantity item in CurrentPlayer.Inventory.ToList())
            {
                if(item.BaseItem.Name.ToLower() == aString.ToLower())
                {
                    if (item.BaseItem.Category == Item.ItemCategory.Weapon)
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
        private void DetermineActors()
        {

        }

        #endregion

    }
}
