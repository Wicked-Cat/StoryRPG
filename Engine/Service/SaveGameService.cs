using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Engine.Factories;
using Engine.Models;
using Engine.ViewModels;

namespace Engine.Service
{
    public static class SaveGameService
    {
        public static void Save(GameSession gameSession, string fileName)
        {
            File.WriteAllText(fileName,
                              JsonConvert.SerializeObject(gameSession, Formatting.Indented));
        }
        public static GameSession LoadLastSavedOrCreateNew(string fileName)
        {
            if(!File.Exists(fileName))
            {
                //if there is no save game, create new default
                return new GameSession(); 
            }

            try
            {
                JObject data = JObject.Parse(File.ReadAllText(fileName));

                //populate player object
                Player player = CreatePlayer(data);
                World world = CreateWorld(data);
                Time time = CreateTime(data);

                //multidimentional array
                //find currentlocation, then find it's child property
                int x = (int)data[nameof(GameSession.CurrentLocation)][nameof(Location.XCoordinate)];
                int y = (int)data[nameof(GameSession.CurrentLocation)][nameof(Location.YCoordinate)];
                int z = (int)data[nameof(GameSession.CurrentLocation)][nameof(Location.ZCoordinate)];

                //create gamesession object with save game data
                return new GameSession(player, world, time, x, y, z);
            }
            catch(Exception ex)
            {
                //if there was an error loading/deserializing the saved game, create a new gamesession object
                return new GameSession();
            }
        }

        private static Player CreatePlayer(JObject data)
        {
            string fileVersion = FileVersion(data);
            Player player;
            switch (fileVersion)
            {
                case "0.1.000":
                    //player = new Player((string)data[nameof(GameSession.CurrentPlayer)][nameof(Player.Name)],
                    //   (string)data[nameof(GameSession.CurrentPlayer)][nameof(Player.Description)]);
                    player = new Player("I loaded", "yay");

                    break;
                default:
                    throw new InvalidDataException($"File version '{fileVersion}' not recognized");
            }
            PopulatePlayerAttributes(data, player);
            PopulatePlayerInventory(data, player);
            return player;
        }
        private static World CreateWorld(JObject data)
        {
            string fileVersion = FileVersion(data);
            World world;
            switch (fileVersion)
            {
                case "0.1.000":
                    world = new World();


                    break;
                default:
                    throw new InvalidDataException($"File version '{fileVersion}' not recognized");
            }
            PopulateLocations(data, world);
            PopulateGameSessionAttributes(data);
            return world;
        }
        private static Time CreateTime(JObject data)
        {
            int day = (int)data[nameof(GameSession.CurrentTime)][nameof(Time.Day)];
            int hour = (int)data[nameof(GameSession.CurrentTime)][nameof(Time.Hour)];
            int minute = (int)data[nameof(GameSession.CurrentTime)][nameof(Time.Minute)];
            int daysPassed = (int)data[nameof(GameSession.CurrentTime)][nameof(Time.DaysPassed)];
            string currentDay = (string)data[nameof(GameSession.CurrentTime)][nameof(Time.CurrentDay)];
            string currentSeason = (string)data[nameof(GameSession.CurrentTime)][nameof(Time.CurrentSeason)];
            string currentYear = (string)data[nameof(GameSession.CurrentTime)][nameof(Time.CurrentYear)];

            Time time = new Time(daysPassed, day, hour, minute, currentDay, currentSeason, currentYear);

            return time;
        }
        private static void PopulatePlayerInventory(JObject data, Player player)
        {
            string fileVersion = FileVersion(data);
            switch (fileVersion)
            {
                case "0.1.000":
                    foreach(JToken itemToken in (JArray)data[nameof(GameSession.CurrentPlayer)]
                        [nameof(Player.Inventory)])
                        //[nameof(BaseItem.Name)])
                    {
                        int itemId = (int)itemToken[nameof(ItemQuantity.BaseItem)]
                            [nameof(ItemQuantity.BaseItem.ID)];
                        int quantity = (int)itemToken[nameof(ItemQuantity.Quantity)];
                        int weaponId = (int)data[nameof(GameSession.CurrentPlayer)][nameof(Player.EquippedWeapon)][nameof(Item.ID)];
                        player.AddItemsToInventory(ItemFactory.CreateGameItem(itemId), quantity);
                        player.EquippedWeapon = ItemFactory.CreateGameItem(weaponId);
                    }
                    player.NumberInventory();
                    break;
            }
        }
        private static void PopulatePlayerAttributes(JObject data, Player player)
        {
            string fileVersion = FileVersion(data);
            switch(fileVersion)
            {
                case "0.1.000":
                    player.Ancestry = AncestryFactory.GetAncestry((string)data[nameof(GameSession.CurrentPlayer)][nameof(Player.Ancestry)]
                        [nameof(Ancestry.Name)]);
                    foreach(JToken skillToken in (JArray)data[nameof(GameSession.CurrentPlayer)]
                        [nameof(Player.Skills)])
                    {
                        string skillName = (string)skillToken[nameof(Skill.Name)];
                        int baseLevel = (int)skillToken[nameof(Skill.BaseLevel)];
                        double levelMulti = (double)skillToken[nameof(Skill.LevelMultiplier)];
                        player.LoadSkills(skillName, baseLevel, levelMulti);
                    }
                    foreach(JToken charToken in (JArray)data[nameof(GameSession.CurrentPlayer)]
                        [nameof(Player.Characteristics)])
                    {
                        string charName = (string)charToken[nameof(Characteristic.Name)];
                        int baseLevel = (int)charToken[nameof(Characteristic.BaseLevel)];
                        double levelMulti = (double)charToken[nameof(Characteristic.LevelMultiplier)];
                        player.LoadCharacteristics(charName, baseLevel, levelMulti);
                    }
                    player.CurrentBody = BodyFactory.GetBody
                        ($"{(string)data[nameof(GameSession.CurrentPlayer)][nameof(Player.CurrentBody)][nameof(Body.Name)]}").Clone();
                    break;
            }
        }
        private static void PopulateLocations(JObject data, World world)
        {
            string fileVersion = FileVersion(data);
            switch(fileVersion)
            {
                case "0.1.000":
                    foreach(JToken locToken in (JArray)data[nameof(GameSession.CurrentWorld)][nameof(World._locations)])
                    {
                        Location loc;
                        int xCoord = (int)locToken[nameof(Location.XCoordinate)];
                        int yCoord = (int)locToken[nameof(Location.YCoordinate)];
                        int zCoord = (int)locToken[nameof(Location.ZCoordinate)];
                        string name = (string)locToken[nameof(Location.Name)];
                        string description = (string)locToken[nameof(Location.Description)];
                        loc = new Location(xCoord, yCoord, zCoord, name, description);

                        foreach (JToken itemToken in (JArray)locToken[nameof(Location.ItemsHere)])
                        {
                            int itemID = (int)itemToken[nameof(ItemQuantity.BaseItem)][nameof(ItemQuantity.BaseItem.ID)];
                            int quantity = (int)itemToken[nameof(ItemQuantity.Quantity)];
                            loc.AddItemsToLocation(ItemFactory.GetItemByID(itemID), quantity);
                        }
                        foreach (JToken itemToken in (JArray)locToken[nameof(Location.AllItemsHere)])
                        {
                            int itemID = (int)itemToken[nameof(LocationItems.ID)];
                            int percent = (int)itemToken[nameof(LocationItems.Percentage)];
                            int quantity = (int)itemToken[nameof(LocationItems.Quantity)];
                            bool respawns = (bool)itemToken[nameof(LocationItems.Respawns)];
                            bool collected = (bool)itemToken[nameof(LocationItems.HasBeenCollected)];
                            loc.LoadAllItems(itemID, percent, quantity, respawns, collected);
                        }
                        foreach(JToken encounterToken in (JArray)locToken[nameof(Location.EncountersHere)])
                        {
                            int encID = (int)encounterToken[nameof(Encounter.ID)];
                            Encounter encounter = EncounterFactory.GetEncounter(encID);
                            loc.EncountersHere.Add(encounter);
                        }
                        foreach(JToken encounterToken in (JArray)locToken[nameof(Location.AllEncountersHere)])
                        {
                            int encID = (int)encounterToken[nameof(EncounterPercent.EncounterID)];
                            int percent = (int)encounterToken[nameof(EncounterPercent.ChanceOfEncounter)];
                            loc.AddEncounter(encID, percent);
                        }
                        foreach(JToken merchToken in (JArray)locToken[nameof(Location.MerchantsHere)])
                        {
                            int merchID = (int)merchToken[nameof(Merchant.ID)];
                            loc.MerchantsHere.Add(MerchantFactory.GetMerchantByID(merchID));
                        }
                        foreach (JToken merchToken in (JArray)locToken[nameof(Location.AllMerchantsHere)])
                        {
                            int merchID = (int)merchToken[nameof(MerchantPercent.MerchantID)];
                            int percent = (int)merchToken[nameof(MerchantPercent.ChanceOfEncounter)];
                            loc.AddMerchant(merchID, percent);
                        }
                        world.AddLocation(loc);
                    }
                    break;
                default:
                    break;
            }
        }
        private static void PopulateGameSessionAttributes(JObject data)
        {
            string fileVersion = FileVersion(data);
            switch (fileVersion)
            {
                case "0.1.000":
                    break;
                default:
                    throw new InvalidDataException($"File version '{fileVersion}' not recognized");
            }
        }
        private static string FileVersion(JObject data)
        {
            return (string)data[nameof(GameSession.Version)];
        }
    }
}
