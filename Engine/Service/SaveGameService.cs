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
        private const string SAVE_GAME_FILE_NAME = "StoryRPG.json";
        public static void Save(GameSession gameSession)
        {
            File.WriteAllText(SAVE_GAME_FILE_NAME,
                              JsonConvert.SerializeObject(gameSession, Formatting.Indented));
        }
        public static GameSession LoadLastSavedOrCreateNew()
        {
            if(!File.Exists(SAVE_GAME_FILE_NAME))
            {
                //if there is no save game, create new default
                return new GameSession(); 
            }

            try
            {
                JObject data = JObject.Parse(File.ReadAllText(SAVE_GAME_FILE_NAME));

                //populate player object
                Player player = CreatePlayer(data);

                //multidimentional array
                //find currentlocation, then find it's child property
                int x = (int)data[nameof(GameSession.CurrentLocation)][nameof(Location.XCoordinate)];
                int y = (int)data[nameof(GameSession.CurrentLocation)][nameof(Location.YCoordinate)];
                int z = (int)data[nameof(GameSession.CurrentLocation)][nameof(Location.ZCoordinate)];

                //create gamesession object with save game data
                return new GameSession(player, x, y, z);
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
        private static void PopulatePlayerInventory(JObject data, Player player)
        {
            string fileVersion = FileVersion(data);
            switch (fileVersion)
            {
                case "0.1.000":
                    foreach(JToken itemToken in (JArray)data[nameof(GameSession.CurrentPlayer)]
                        [nameof(Player.Inventory)])
                        //[nameof(Inventory.BaseItem)])
                    {
                        int itemId = (int)itemToken[nameof(ItemQuantity.BaseItem.ID)];
                        player.AddItemToInventory(ItemFactory.CreateGameItem(itemId));
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
                    // player.Ancestry = AncestryFactory.GetAncestry((string)data[nameof(GameSession.Player)][nameof(Player.Ancestry.Name)]);

                   

                    break;
            }
        }

        private static string FileVersion(JObject data)
        {
            return (string)data[nameof(GameSession.Version)];
        }
    }
}
