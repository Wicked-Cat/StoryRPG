using Engine.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Models;
using Engine.Factories;
using StoryRPG;
using System.Collections.ObjectModel;
using Engine.Service;
using Newtonsoft.Json;

namespace Engine.ViewModels
{
    public class GameSession : BaseNotificationClass
    {
        private readonly MessageBroker _messageBroker = MessageBroker.GetInstance();
        public event EventHandler<GameMessageEventArgs> OnMessageRaised;
        public event EventHandler<OnEncounterEventArgs> OnEncounterEngaged;
        public event EventHandler OnInventoryOpened;
        public event EventHandler OnCharacterOpened;
        public event EventHandler<OnTradeEventArgs> OnTradeInitiated;
        public event EventHandler<OnChallengeEventArgs> OnChallengeInitiated;
        public event EventHandler OnQuit;

        #region Private Variables
        private Player _currentPlayer;
        private Location _currentLocation;
        private Encounter _currentEncounter;
        private Merchant _currentMerchant;
        private string _writtenTime;

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
                    _currentPlayer.OnKilled -= OnPlayerKilled;
                }
                _currentPlayer = value;
                if (_currentPlayer != null)
                {
                    _currentPlayer.OnActionPerformed += OnPlayerAction;
                    _currentPlayer.OnKilled += OnPlayerKilled;
                }
            }
        }
        public Location PreviousLocation { get; set; }
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
                if(CurrentLocation.ChallengeHere == null || CurrentLocation.ChallengeHere.ChallengeCompleted)
                     _messageBroker.RaiseMessage($"{CurrentLocation.Description}");
                foreach(Encounter encounter in CurrentLocation.EncountersHere)
                     _messageBroker.RaiseMessage($"{encounter.Name}");
                ChallengeWatch();
                if(CurrentLocation.ChallengeHere != null)
                     if (!CurrentLocation.ChallengeHere.ChallengeCompleted)
                         GenerateChallengeText();

            }
        }
        public Encounter CurrentEncounter
        {
            get { return _currentEncounter; }
            set
            {
                if (_currentEncounter != null)
                {
                    foreach (Monster monster in CurrentEncounter.Monsters)
                    {
                        monster.OnActionPerformed -= OnMonsterAction;
                    }
                }
                _currentEncounter = value;
                if ( _currentEncounter != null)
                {
                    _messageBroker.RaiseMessage("");
                    CurrentEncounter.Monsters[0].OnActionPerformed += OnMonsterAction;
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasEncounter));
                EncounterWatch();
                if( _currentEncounter != null )
                {
                    _messageBroker.RaiseMessage($"You encounter a {CurrentEncounter.Name}");
                }
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
        public Time CurrentTime { get; set; }
        public string WrittenTime 
        {
            get { return _writtenTime; }
            set 
            {
                _writtenTime = value; 
                OnPropertyChanged(); 
            }
        }

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
        public bool HasChallenge => CurrentLocation.ChallengeHere != null;
        #endregion

        #region Constructor
        public GameSession()
        {
            #region Construct Player
            CurrentPlayer = new Player("Laughing Zebra", "Druid", 100, 100, "A Human", 10);
            CurrentPlayer.CurrentAncestry = AncestryFactory.GetAncestry("Human");
            foreach (Tag tag in AncestryFactory.GetAncestry("Human").Tags.ToList())
                CurrentPlayer.CurrentAncestry.Tags.Add(tag);

            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(2001));
            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(9001));
            CurrentPlayer.NumberInventory();

            foreach (Skill skill in SkillFactory._skills)
            {
                if (skill.Category != Skill.Categories.Curse)
                {
                    CurrentPlayer.Skills.Add(skill.Clone());
                }
            }
            foreach (Characteristic attribute in CharacteristicFactory._characteristics)
                CurrentPlayer.Characteristics.Add(attribute.Clone());


            foreach (Multiplier multiplier in CurrentPlayer.CurrentAncestry.Multipliers)
            {
                switch (multiplier.MultiplierType)
                {
                    case Multiplier.Type.Skill:
                        CurrentPlayer.Skills.FirstOrDefault(s => s.Name.ToLower() == multiplier.Name.ToLower()).LevelMultiplier = multiplier.MultiplierValue;
                        break;
                    case Multiplier.Type.Characteristic:
                        CurrentPlayer.Characteristics.FirstOrDefault(c => c.Name.ToLower() == multiplier.Name.ToLower()).LevelMultiplier = multiplier.MultiplierValue;
                        break;
                }
            }
            CurrentPlayer.CurrentBody = BodyFactory.GetBody("Immortal").Clone();
            CurrentPlayer.EquippedWeapon = (ItemFactory.CreateGameItem(1001));
            #endregion 

            CurrentTrade = new Trade(new ObservableCollection<ItemQuantity>(), new ObservableCollection<ItemQuantity>());

            CurrentWorld = WorldFactory.CreateWorld();
            CurrentWorld.RefreshLocations();
            CurrentLocation = CurrentWorld.LocationAt(0, 0, 0);

            CurrentTime = new Time(0, 1, 0, 1, Time.Days.Fridas, Time.Seasons.Spring, Time.Years.Catfish);
            CurrentTime.CurrentTimeOfDay = CurrentTime.ProgressTimeOfDay(CurrentTime.CurrentTimeOfDay, CurrentTime.Hour);
            WrittenTime = CurrentTime.WriteTime();

        }

        #endregion

        public void WindowToSession(string aString)
        {
            _messageBroker.RaiseMessage("");
            _messageBroker.RaiseMessage(aString);
            if (CurrentMerchant != null)
                DoInTrade(aString);
            else if (CurrentEncounter != null)
                DoInCombat(aString);
            else if (CurrentLocation.ChallengeHere != null && !CurrentLocation.ChallengeHere.ChallengeCompleted)
                DoInChallenge(aString);
            else
                Do(aString);
        }

        #region Time Fuctions
        private void PassTime(int minutePassed)
        {
                for (int i = 0; i < minutePassed; i++)
                {
                    CurrentTime.Minute++;

                    if (CurrentTime.Minute > 60)
                    {
                        CurrentTime.Minute = 0;
                        CurrentTime.Hour++;
                        CurrentTime.CurrentTimeOfDay = CurrentTime.ProgressTimeOfDay(CurrentTime.CurrentTimeOfDay, CurrentTime.Hour);

                        if (CurrentTime.Hour > 24)
                        {
                            CurrentTime.Hour = 0;
                            CurrentTime.Day++;
                            CurrentTime.DaysPassed++;
                            CurrentTime.CurrentDay = CurrentTime.ProgressDay(CurrentTime.CurrentDay);
                            MerchantFactory.GenerateMerchants();
                            CurrentWorld.RefreshLocations();
                            CurrentLocation.EncountersText = CurrentLocation.WriteEncounterText();
                            CurrentWorld.TickChallenges();
                            CurrentWorld.RefreshChallenges(CurrentLocation);

                        if (CurrentTime.Day > 30)
                            {
                                CurrentTime.Day = 0;
                                CurrentTime.CurrentSeason = CurrentTime.ProgressSeason(CurrentTime.CurrentSeason);
                               
                                if (CurrentTime.CurrentSeason == Time.Seasons.Spring)
                                {
                                      CurrentTime.CurrentYear = CurrentTime.ProgressYear(CurrentTime.CurrentYear);
                                }
                            }
                        }
                    }
                }

            WrittenTime = CurrentTime.WriteTime();
        }
        #endregion

        #region Movement Functions
        private void MoveNorth()
        {
            if (HasNorthExit)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1, CurrentLocation.ZCoordinate);
            }
            else
            {
                _messageBroker.RaiseMessage("There is no exit that way");
            }
        }
        private void MoveSouth()
        {
            if (HasSouthExit)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1, CurrentLocation.ZCoordinate);
            }
            else
            {
                _messageBroker.RaiseMessage("There is no exit that way");
            }
        }
        private void MoveEast()
        {
            if (HasEastExit)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate, CurrentLocation.ZCoordinate);
            }
            else
            {
                _messageBroker.RaiseMessage("There is no exit that way");
            }
        }
        private void MoveWest()
        {
            if (HasWestExit)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate, CurrentLocation.ZCoordinate);
            }
            else
            {
                _messageBroker.RaiseMessage("There is no exit that way");
            }
        }
        private void MoveUp()
        {
            if (HasUpExit)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate, CurrentLocation.ZCoordinate + 1);
            }
            else
            {
                _messageBroker.RaiseMessage("There is no exit that way");
            }
        }
        private void MoveDown()
        {
            if (HasDownExit)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate, CurrentLocation.ZCoordinate - 1);
            }
            else
            {
                _messageBroker.RaiseMessage("There is no exit that way");
            }
        }
        #endregion

        #region Encounter Functions
        private void EncounterWatch()
        {
            OnEncounterEngaged?.Invoke(this, new OnEncounterEventArgs(CurrentEncounter));
        }
        private void OnMonsterAction(object sender, string result)
        {
            _messageBroker.RaiseMessage(result);
        }
        private void OnMonsterKilled(object sender, System.EventArgs eventArgs)
        {
            _messageBroker.RaiseMessage("deaad");
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
            Encounter tempEncounter = CurrentEncounter;
            CurrentLocation.EncountersHere.Remove(CurrentLocation.EncountersHere.First(e => e.Name.ToLower() == CurrentEncounter.Name.ToLower()));
            EncounterWatch();
            OnPropertyChanged(nameof(CurrentLocation.EncountersText));
            foreach (Monster monster in tempEncounter.Monsters)
            {
                _messageBroker.RaiseMessage("");
                foreach (ItemQuantity item in monster.Inventory)
                {
                    _messageBroker.RaiseMessage($"You recieve {item.Quantity} {item.BaseItem.Name}.");
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        CurrentPlayer.AddItemToInventory(item.BaseItem);
                    }
                }
            }
        }
        private void GetEncounterAtLocation(string aString)
        {
            CurrentEncounter = CurrentLocation.GetEncounter(aString);
        }
        #endregion

        private void ChallengeWatch()
        {
                OnChallengeInitiated?.Invoke(this, new OnChallengeEventArgs(CurrentLocation.ChallengeHere));
        }
        private void GenerateChallengeText()
        {
            int i;
            if (CurrentLocation.ChallengeHere != null)
            {
                for (i = 1; i <= CurrentLocation.ChallengeHere.Obstacles.Count(); i++)
                {
                    CurrentLocation.ChallengeHere.Obstacles[i - 1].SelectionNumber = i;
                }

                _messageBroker.RaiseMessage($"{CurrentLocation.ChallengeHere.Description}");
                foreach (Obstacle obstacle in CurrentLocation.ChallengeHere.Obstacles)
                    _messageBroker.RaiseMessage($"{obstacle.SelectionNumber}. {obstacle.Description}");
                _messageBroker.RaiseMessage($"{i}. Leave");
            }

        }
        private void ChallengeTest(int num)
        {
            string successText = CurrentLocation.ChallengeHere.Obstacles[num - 1].PassText;
            if (CurrentLocation.ChallengeHere.Obstacles[num - 1].Check is Item)
            {
                Item item = CurrentLocation.ChallengeHere.Obstacles[num - 1].Check as Item;
                int quantity = CurrentLocation.ChallengeHere.Obstacles[num - 1].CheckValue;
                if (CurrentPlayer.HasItem(item.Name))
                {
                    CurrentLocation.ChallengeHere.Obstacles[num - 1].Passed = true;
                }
                else
                {
                    _messageBroker.RaiseMessage($"{CurrentLocation.ChallengeHere.Obstacles[num - 1].FailText}");
                    return;
                }
            }
            else if (CurrentLocation.ChallengeHere.Obstacles[num - 1].Check is Skill)
            {
                Skill skill = CurrentLocation.ChallengeHere.Obstacles[num - 1].Check as Skill;
                if(CurrentPlayer.Skills.First(s => s.Name == skill.Name).EffectiveLevel >= CurrentLocation.ChallengeHere.Obstacles[num - 1].CheckValue)
                {
                    CurrentLocation.ChallengeHere.Obstacles[num - 1].Passed = true;
                }
                else
                {
                    _messageBroker.RaiseMessage($"{CurrentLocation.ChallengeHere.Obstacles[num - 1].FailText}");
                    return;
                }
            }
            else if (CurrentLocation.ChallengeHere.Obstacles[num - 1].Check is Characteristic)
            {
                Characteristic characteristiic = CurrentLocation.ChallengeHere.Obstacles[num - 1].Check as Characteristic;
                if (CurrentPlayer.Characteristics.First(c => c.Name == characteristiic.Name).EffectiveLevel >= CurrentLocation.ChallengeHere.Obstacles[num - 1].CheckValue)
                {
                    CurrentLocation.ChallengeHere.Obstacles[num - 1].Passed = true;
                }
                else
                {
                    _messageBroker.RaiseMessage($"{CurrentLocation.ChallengeHere.Obstacles[num - 1].FailText}");
                    return;
                }
            }
            ChallengeWatch();

            if (CurrentLocation.ChallengeHere.ChallengeCompleted)
                OnChallengeSucess(successText);
        }
        private void OnChallengeSucess(string successText)
        {
            _messageBroker.RaiseMessage($"{successText}");
            _messageBroker.RaiseMessage($"{CurrentLocation.Description}");
        }
        private void OnChallengeExit()
        {
            CurrentLocation = PreviousLocation;
            ChallengeWatch();
        }

        #region Player Functions
        private void OnPlayerKilled(object sender, System.EventArgs eventArgs)
        {
            PassTime(900);
            _messageBroker.RaiseMessage("");
            _messageBroker.RaiseMessage("You have been killed.");
            CurrentLocation = CurrentWorld.LocationAt(0, 0, 0);
            CurrentPlayer.FullHeal();
        }
        private void OnPlayerAction(object sender, string result)
        {
            _messageBroker.RaiseMessage(result);
        }

        #endregion

        #region Player Action Functions

        #region Do
        private void DoInCombat(string aString)
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
                case "ancestry":
                    if (CurrentEncounter != null)
                        foreach (Monster monster in CurrentEncounter.Monsters)
                        {
                            _messageBroker.RaiseMessage($"{monster.CurrentAncestry.Name}");
                            foreach (Tag tag in monster.Tags)
                            {
                                _messageBroker.RaiseMessage($"{tag.Name}");
                            }
                            foreach(Characteristic characteristic in monster.Characteristics)
                            {
                                _messageBroker.RaiseMessage($"{characteristic.Name} : EL{characteristic.EffectiveLevel} : M{characteristic.LevelMultiplier}");
                            }
                        }
                    break;
                case "encounter":
                    if (CurrentEncounter != null)
                        _messageBroker.RaiseMessage("true");
                    else
                        _messageBroker.RaiseMessage("false");
                    break;
                default:
                    _messageBroker.RaiseMessage($"You cannot do that now");
                    break;
            }
        }
        private void DoInTrade(string aString)
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
                        _messageBroker.RaiseMessage("Preferred");
                        foreach (Tag tag in CurrentMerchant.PreferredItems)
                            _messageBroker.RaiseMessage($"{tag.Name}");
                        _messageBroker.RaiseMessage("Disliked");
                        foreach (Tag tag in CurrentMerchant.DislikedItems)
                            _messageBroker.RaiseMessage($"{tag.Name}");
                    }
                    break;
                case "merchant":
                    foreach (Merchant merchant in CurrentLocation.MerchantsHere)
                    {
                        _messageBroker.RaiseMessage($"Sell list {merchant._sellList.Count}");
                        foreach (MerchantStock item in merchant._sellList)
                            _messageBroker.RaiseMessage($"Sell Quantity {item.Quantity}");
                        _messageBroker.RaiseMessage($"IInventory {merchant.Inventory.Count}");
                        foreach (ItemQuantity item in merchant.Inventory)
                            _messageBroker.RaiseMessage($"Inventory Quantity {item.Quantity}");

                    }
                    _messageBroker.RaiseMessage($"Current sell list {CurrentMerchant._sellList.Count}");
                    foreach (MerchantStock item in CurrentMerchant._sellList)
                        _messageBroker.RaiseMessage($"{ItemFactory.GetItemByID(item.ID).Name} Q: {item.Quantity}");
                    _messageBroker.RaiseMessage($"IInventory {CurrentMerchant.Inventory.Count}");
                    foreach (ItemQuantity item in CurrentMerchant.Inventory)
                        _messageBroker.RaiseMessage($"Inventory {item.BaseItem.Name} q:{item.Quantity}");
                    break;
                default:
                    _messageBroker.RaiseMessage($"{aString} is not a valid command");
                    break;
            }
        }
        private void DoInChallenge(string aString)
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

            bool parse = int.TryParse(verb, out int num);
            int maxnum = CurrentLocation.ChallengeHere.Obstacles.Count() + 1;

            if (parse)
            {
                if(num < maxnum)
                {
                    ChallengeTest(num);
                }
                else
                {
                    OnChallengeExit();
                }
            }
            else
            {
                switch (verb)
                {
                    case "?":
                    case "help":
                        //WriteCommands();
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
                }
            }
        }
        private void Do(string aString)
        {
            if (aString == "")
                return;
            string verb = "";
            string noun = "";
            string splitNoun = "";
            string tempNum = "";
            int num = 1;

            if (aString.IndexOf(" ") > 0) //if there is a space in the string, split the string into two partsb
            {
                string[] temp = aString.Split(new char[] { ' ' }, 2);
                verb = temp[0].ToLower();
                noun = temp[1].ToLower();
            }
            else
            {
                verb = aString.ToLower();
            }

            if (noun.IndexOf(" ") > 0) //if the split noun has a space, split it again and determine if a number is involved
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
                case "n":
                    MoveTo("north");
                    break;
                case "s":
                    MoveTo("south");
                    break;
                case "e":
                    MoveTo("east");
                    break;
                case "w":
                    MoveTo("west");
                    break;
                case "u":
                    MoveTo("up");
                    break;
                case "d":
                    MoveTo("down");
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
                    Quit();
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
                case "wait":
                    PassTime(Convert.ToInt32(noun));
                    break;
                case "increaseskill":
                    CurrentPlayer.AddExperienceToSkill(noun, num);
                    break;
                case "increasechar":
                    CurrentPlayer.AddExperienceToCharacteristic(noun, num);
                    break;
                case "spawn":
                    Spawn(noun);
                    break;
                case "self":
                    foreach (BodyPart part in CurrentPlayer.CurrentBody.Parts)
                    {
                        _messageBroker.RaiseMessage($"{part.Name} {part.CurrentHealth}/{part.MaximumHealth}");
                        foreach (BodyPart subPart in part.SubParts)
                        {
                            _messageBroker.RaiseMessage($"     {subPart.Name} {subPart.CurrentHealth}/{subPart.MaximumHealth}");
                        }
                    }
                    break;
                case "attack":
                    GetEncounterAtLocation(noun);
                    break;
                case "items":
                    foreach (LocationItems item in CurrentLocation.AllItemsHere)
                    {
                        _messageBroker.RaiseMessage($"{item.ID} Collected?{item.HasBeenCollected} Respawns?{item.Respawns}");
                    }
                    break;
                case "challenge":
                    if (CurrentLocation.ChallengeHere != null)
                    {
                        _messageBroker.RaiseMessage($"{CurrentLocation.ChallengeHere.Name}");
                        foreach (var obstacle in CurrentLocation.ChallengeHere.Obstacles)
                        {
                            if (obstacle.Check is Item)
                                _messageBroker.RaiseMessage($"{(obstacle.Check as Item).Name} {obstacle.CheckValue}");
                            if (obstacle.Check is Skill)
                                _messageBroker.RaiseMessage($"{(obstacle.Check as Skill).Name} {obstacle.CheckValue}");
                            if (obstacle.Check is Characteristic)
                                _messageBroker.RaiseMessage($"{(obstacle.Check as Characteristic).Name} {obstacle.CheckValue}");
                        }
                    }
                    break;
                default:
                    _messageBroker.RaiseMessage($"You cannot do that now");
                    break;
            }
        }
        #endregion

        private void Examine(string aString)
        {
            Item item = CurrentPlayer.FindNumberedItem(aString);
            bool IsItem = false;
            bool IsMonster = false;

            if(item != default)
            {
                IsItem = CurrentPlayer.HasItem(item.Name);
            }
            if(CurrentEncounter != null)
                 IsMonster = CurrentEncounter.Monsters.Any(m => m.Name.ToLower() == aString.ToLower());

            if (IsItem)
            {
                if (item != default)
                {
                    string tags = "";
                    foreach (Tag tag in item.Tags)
                    {
                        tags += $" {tag.Name}";
                    }

                    _messageBroker.RaiseMessage($"{item.Description}, TV: {item.ActualValue}\n Tags: {tags}");
                    _messageBroker.RaiseMessage("");
                }
                else
                {
                    _messageBroker.RaiseMessage($"{aString} is not a valid item");
                }
            }
            else if (IsMonster)
            {
                Monster monster = CurrentEncounter.Monsters.FirstOrDefault(m => m.Name.ToLower() == aString.ToLower());
                if(monster != default)
                {
                    _messageBroker.RaiseMessage($"{monster.Description}");
                    _messageBroker.RaiseMessage("Tags:");
                    foreach (Tag tag in monster.Tags)
                    {
                        _messageBroker.RaiseMessage($"{tag.Name}");
                    }
                    _messageBroker.RaiseMessage("Stats:");
                    foreach(Characteristic characteristic in monster.Characteristics)
                    {
                        _messageBroker.RaiseMessage($"{characteristic.Name} BL{characteristic.BaseLevel} : EL {characteristic.EffectiveLevel}");
                    }
                    _messageBroker.RaiseMessage($"Body: {monster.CurrentBody.Name}");
                    foreach(BodyPart part in monster.CurrentBody.Parts)
                    {
                        _messageBroker.RaiseMessage($"{part.Name}:");
                        foreach(BodyPart childPart in part.SubParts)
                        {
                            _messageBroker.RaiseMessage($"    {childPart.Name}");
                        }
                    }
                }
            }
        }
        private void Spawn(string aString)
        {
            int itemID = ItemFactory.ItemID(aString);
            if (itemID != default)
            {
                _messageBroker.RaiseMessage($"You take the {ItemFactory.ItemName(itemID)}.");
                CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(itemID));
            }
            else
                _messageBroker.RaiseMessage($"{aString} does not exist");
        }
        private void Pickup(string aString)
        {
            Item item = ItemFactory.GetItem(aString);
            if (item != null) //if item exists
            {
                if(CurrentLocation.ItemsHere.Exists(i => i.BaseItem.Name == item.Name)) //if item is in the location
                {
                    _messageBroker.RaiseMessage($"You take the {item.Name}.");
                    CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(ItemFactory.ItemID(item.Name)));
                    CurrentLocation.RemoveItemFromLocation(item);
                }
                else
                {
                    _messageBroker.RaiseMessage($"{aString} is not here");
                }
            }
            else
                _messageBroker.RaiseMessage($"{aString} does not exist");
        }
        private void Drop(string aString)
        {
            Item item = CurrentPlayer.FindNumberedItem(aString);
            if (item != default)
            {
                _messageBroker.RaiseMessage($"You drop the {item.Name}.");
                CurrentPlayer.RemoveItemFromInventory(item);
                CurrentLocation.AddItemToLocation(item);
            }
            else
            {
                _messageBroker.RaiseMessage($"You do not have any {aString}.");
            }
        }
        private void MoveTo(string aString)
        {
            PreviousLocation = CurrentLocation;
            PassTime(20);
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
        private void Inventory()
        {
            OnInventoryOpened?.Invoke(this, System.EventArgs.Empty);
        }
        private void Character()
        {
            OnCharacterOpened?.Invoke(this, System.EventArgs.Empty);
        }
        private void Equip(string aString)
        {
            Item item = CurrentPlayer.FindNumberedItem(aString);

            if (item != default)
            {
                int match = item.Tags.FindIndex(t => t.Name.ToLower() == "Weapon".ToLower());
                if (match >= 0)
                {
                    if (CurrentPlayer.EquippedWeapon != null)
                    {
                        CurrentPlayer.RemoveItemFromInventory(item);
                        CurrentPlayer.AddItemToInventory(CurrentPlayer.EquippedWeapon);

                        CurrentPlayer.EquippedWeapon = item;
                    }
                    else
                    {
                        CurrentPlayer.RemoveItemFromInventory(item);

                        CurrentPlayer.EquippedWeapon = item;
                    }
                }
                else
                {
                    _messageBroker.RaiseMessage($"{item.Name} is not a weapon.");
                }
            }
            else
            {
                _messageBroker.RaiseMessage($"You do not have a {aString}");
            }
        }
        private void Unequip(string aString)
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
                    _messageBroker.RaiseMessage($"{aString} is not equipped.");
                }
            }
            else
            {
                _messageBroker.RaiseMessage("There is nothing to unequip");
            }
        }
        private void Quit()
        {
            OnQuit?.Invoke(this, System.EventArgs.Empty);
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
        private void Attack(string noun)
        {
            PassTime(1);
            if (CurrentEncounter == null)
            {
                _messageBroker.RaiseMessage("There is nothing to attack");
                return;
            }
            if (CurrentPlayer.EquippedWeapon == null)
            {
                _messageBroker.RaiseMessage("You need a weapon equipped to attack");
                return;
            }

            Monster target = DetermineTarget(noun);
            if (target != null && !target.IsDead)
            {
                _messageBroker.RaiseMessage("");
                CurrentPlayer.Attack(target);
                _messageBroker.RaiseMessage("");
            }
            else
            {
                _messageBroker.RaiseMessage($"{noun} is not a valid target");
                return;
            }

            if (CurrentEncounter != null)
            {
                foreach (Monster monster in CurrentEncounter.Monsters)
                {
                    if (!monster.IsDead && !CurrentPlayer.IsDead)
                    {
                        monster.Attack(CurrentPlayer);
                    }
                }
            }
            if (CurrentEncounter != null)
            {
                if (AreAllMonstersDead)
                {
                    OnEncounterEnd();
                    CurrentEncounter = null;
                }
            }

        }
        private void Flee()
        {
            _messageBroker.RaiseMessage($"You flee from the {CurrentEncounter.Name}");
            CurrentLocation.EncountersHere.Remove(CurrentLocation.EncountersHere.First(e => e.Name.ToLower() == CurrentEncounter.Name.ToLower()));
            CurrentEncounter = null;
        }

        #endregion

        #region Trade Functions
        private void TradeWatch()
        {
            OnTradeInitiated?.Invoke(this, new OnTradeEventArgs(CurrentTrade));
        }
        private void Trade(string aString)
        {
            if (CurrentLocation.MerchantsHere.Any(m => m.Name.ToLower() == aString.ToLower()))
            {
                CurrentMerchant = MerchantFactory.GetMerchant(aString);
            }
            else
            {
                _messageBroker.RaiseMessage($"Merchant {aString} is not here");
            }
            
        }
        private void Buy(string aString, int num)
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
                        _messageBroker.RaiseMessage($"{CurrentMerchant.Name} does not have {aString}");
                    }
                }
                else
                {
                    _messageBroker.RaiseMessage($"Item {aString} does not exist");
                }
            }
            else
            {
                _messageBroker.RaiseMessage("You are not trading with a merchant");
            }
        }
        private void Sell(string aString, int num)
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
                        _messageBroker.RaiseMessage($"you do not have {aString}");
                    }
                }
                else
                {
                    _messageBroker.RaiseMessage($"Item {aString} does not exist");
                }
            }
            else
            {
                _messageBroker.RaiseMessage("You are not trading with a merchant");
            }
        }
        private void RemoveBuy(string aString, int num)
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
                        _messageBroker.RaiseMessage($"{CurrentMerchant.Name} does not have {aString}");
                    }
                }
                else
                {
                    _messageBroker.RaiseMessage($"Item {aString} does not exist");
                }
            }
            else
            {
                _messageBroker.RaiseMessage("You are not trading with a merchant");
            }
        }
        private void RemoveSell(string aString, int num) 
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
                        _messageBroker.RaiseMessage($"you have not offered {aString}");
                    }
                }
                else
                {
                    _messageBroker.RaiseMessage($"Item {aString} does not exist");
                }
            }
            else
            {
                _messageBroker.RaiseMessage("You are not trading with a merchant");
            }
        }
        private void ClearTrade()
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
        private void Exchange()
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
                //merchant in current location that shares a name with currentmerchant gets currentmerchant inventory
                MerchantFactory.UpdateMerchant(CurrentMerchant);
                CurrentMerchant = null;
            }
            else
            {
                _messageBroker.RaiseMessage("The merchant does not accept your offer");
            }
        }
        private void CancelTrade()
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
                _messageBroker.RaiseMessage("There is no trade.");
            }
            CurrentMerchant = null;
        }
        private void DetermineItemValues()
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
                    bool PreferredMatch = item.BaseItem.Tags.Any(i => CurrentMerchant.PreferredItems.Any(p => p == i));
                    bool DislikedMatch = item.BaseItem.Tags.Any(i => CurrentMerchant.DislikedItems.Any(d => d == i));

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
