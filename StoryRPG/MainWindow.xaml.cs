using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Engine.EventArgs;
using System.Windows.Threading;
using Engine.ViewModels;
using System.Windows.Media;
using Engine.Service;
using System.ComponentModel;
using Microsoft.Win32;

namespace StoryRPG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string SAVE_GAME_FILE_EXTENSION = "StoryRPG";

        private GameSession _gameSession;
        CombatWindow combatWindow;
        InventoryWindow inventoryWindow;
        CharacterWindow characterWindow;
        EquipmentWindow equipmentWindow;
        TradeWindow tradeWindow;
        SkillWindow skillWindow;
        ChallengeWindow challengeWindow;
        CreateCharacterWindow createCharacterWindow;

        private bool IsCombatWindowOpen;
        private bool IsChallengeWindowOpen;

        private readonly MessageBroker _messageBroker = MessageBroker.GetInstance();

        public MainWindow()
        {
            InitializeComponent();

            //_gameSession = SaveGameService.LoadLastSavedOrCreateNew();
            SetActiveGameSessionTo(new GameSession());

            DataContext = _gameSession; //built in propery for xaml f/iles
            CreateTimer();
            TimeOfDayToColourConverter();

            combatWindow = new CombatWindow();
            combatWindow.DataContext = _gameSession;
            tradeWindow = new TradeWindow();
            tradeWindow.DataContext = _gameSession;
            challengeWindow = new ChallengeWindow();
            challengeWindow.DataContext = _gameSession;
            createCharacterWindow = new CreateCharacterWindow();
            createCharacterWindow.DataContext = _gameSession;
        }
        private void OnGameMessageRaised(object sender, GameMessageEventArgs e)
        {
            if (combatWindow.Visibility == Visibility.Visible)
            {
                combatWindow.GameMessages.Document.Blocks.Add(new Paragraph(new Run(e.Message)));
                combatWindow.GameMessages.ScrollToEnd();
            }
            else if (challengeWindow.Visibility == Visibility.Visible)
            {
                challengeWindow.GameMessages.Document.Blocks.Add(new Paragraph(new Run(e.Message)));
                challengeWindow.GameMessages.ScrollToEnd();
            }
            else if(createCharacterWindow.Visibility == Visibility.Visible)
            {
                createCharacterWindow.GameMessages.Document.Blocks.Add(new Paragraph(new Run((e.Message))));
                createCharacterWindow.GameMessages.ScrollToEnd();
            }
            else
            {
                GameMessages.Document.Blocks.Add(new Paragraph(new Run(e.Message)));
                GameMessages.ScrollToEnd();
            }
        }
        private void OnEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string input = TextBoxInput.Text;
                _gameSession.WindowToSession(input);
                TextBoxInput.Text = "";
            }
        }

      
        #region Timer Functions
        private void CreateTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;
            timer.Start();
        }
        private void TimerTick(object sender, EventArgs e)
        {
            //_gameSession.PassTime(10);
            TimeOfDayToColourConverter();
        }
        public void TimeOfDayToColourConverter()
        {
            switch (_gameSession.CurrentTime.CurrentTimeOfDay.ToString().ToLower())
            {
                case "day":
                    GameMessages.Background = new SolidColorBrush(Colors.Beige);
                    GameMessages.Foreground = new SolidColorBrush(Colors.Black);
                    break;
                case "dusk":
                    GameMessages.Background = new SolidColorBrush(Colors.LightYellow);
                    GameMessages.Foreground = new SolidColorBrush(Colors.Black);
                    break;
                case "night":
                    GameMessages.Background = new SolidColorBrush(Colors.DarkBlue);
                    GameMessages.Foreground = new SolidColorBrush(Colors.White);
                    break;
                case "dawn":
                    GameMessages.Background = new SolidColorBrush(Colors.Plum);
                    GameMessages.Foreground = new SolidColorBrush(Colors.Black);
                    break;
            }


        }

        #endregion

        #region Window Control Functions

        private void OpenInventoryScreen(object sender, EventArgs e)
        {
            inventoryWindow = new InventoryWindow();
            inventoryWindow.Owner = this;
            inventoryWindow.DataContext = _gameSession;
            equipmentWindow = new EquipmentWindow();
            equipmentWindow.Owner = this;
            equipmentWindow.DataContext = _gameSession;
            equipmentWindow.Show();
            inventoryWindow.Show();
        }
        private void OpenCharacterScreen(object sender, EventArgs e)
        {
            characterWindow = new CharacterWindow();
            characterWindow.Owner = this;
            characterWindow.DataContext = _gameSession;
            characterWindow.Show();

            skillWindow = new SkillWindow();
            skillWindow.Owner = this;
            skillWindow.DataContext = _gameSession;
            skillWindow.Show();
        }
        private void CombatWindowControl(object sender, OnEncounterEventArgs encounter)
        {
            if(combatWindow.Visibility == Visibility.Visible)
            {
                if(_gameSession.CurrentEncounter == null || _gameSession.AreAllMonstersDead)
                    combatWindow.Hide();
            }
            else
            {
                if(_gameSession.CurrentEncounter != null)
                {
                    combatWindow.GameMessages.Document.Blocks.Clear();
                    combatWindow.Owner = this;
                    combatWindow.Show();
                }
            }
        }
        private void TradeWindowControl(object sender, OnTradeEventArgs trade)
        {
            if (_gameSession.CurrentMerchant != null)
            {
                //tradeWindow = new TradeWindow();
                tradeWindow.Owner = this;
                //tradeWindow.DataContext = _gameSession;
                tradeWindow.Show();
            }
            else
            {
                if (tradeWindow != null)
                {
                    tradeWindow.Hide();
                }
            }
        }
        private void ChallengeWindowControl(object sender, EventArgs e)
        {
           if (challengeWindow.Visibility == Visibility.Visible)
           {
                if (_gameSession.CurrentLocation.ChallengeHere == null || _gameSession.CurrentLocation.ChallengeHere.ChallengeCompleted)
                    challengeWindow.Hide();
           }
           else
           {
                if (_gameSession.CurrentLocation.ChallengeHere != null && _gameSession.CurrentLocation.ChallengeHere.ChallengeCompleted == false)
                {
                    challengeWindow.GameMessages.Document.Blocks.Clear();
                    challengeWindow.Owner = this;
                    challengeWindow.Show();
                }
           }
        }
        private void CreateCharacterWindowControl(object sender, EventArgs e)
        {
            if(createCharacterWindow.Visibility == Visibility.Visible)
            {
                createCharacterWindow.Hide();
            }
            else
            {
                createCharacterWindow.GameMessages.Document.Blocks.Clear();
                createCharacterWindow.Owner = this;
                createCharacterWindow.Show();
            }
        }
        private void QuitGame(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            SaveGameService.Save(_gameSession, "autosave");
        }

        #endregion

        #region Load/Save Functions
        private void SetActiveGameSessionTo(GameSession gameSession)
        {
            _messageBroker.OnMessageRaised -= OnGameMessageRaised;
            if (_gameSession != null)
            {
                 _gameSession.OnEncounterEngaged -= CombatWindowControl;
                 _gameSession.OnInventoryOpened -= OpenInventoryScreen;
                 _gameSession.OnCharacterOpened -= OpenCharacterScreen;
                 _gameSession.OnTradeInitiated -= TradeWindowControl;
                 _gameSession.OnChallengeInitiated -= ChallengeWindowControl; 
                 _gameSession.OnQuit -= QuitGame;
                 _gameSession.OnSave -= SaveGame;
                 _gameSession.OnNewGame -= NewGame;
            }

            _gameSession = gameSession;
            DataContext = _gameSession;

            combatWindow = new CombatWindow();
            combatWindow.DataContext = _gameSession;
            tradeWindow = new TradeWindow();
            tradeWindow.DataContext = _gameSession;
            challengeWindow = new ChallengeWindow();
            challengeWindow.DataContext = _gameSession;
            createCharacterWindow = new CreateCharacterWindow();
            createCharacterWindow.DataContext = _gameSession;

            GameMessages.Document.Blocks.Clear();
            if(combatWindow != null)
                combatWindow.GameMessages.Document.Blocks.Clear();
            if(challengeWindow != null)
                challengeWindow.GameMessages.Document.Blocks.Clear();

            _messageBroker.OnMessageRaised += OnGameMessageRaised;
            _gameSession.OnEncounterEngaged += CombatWindowControl;
            _gameSession.OnInventoryOpened += OpenInventoryScreen;
            _gameSession.OnCharacterOpened += OpenCharacterScreen;
            _gameSession.OnTradeInitiated += TradeWindowControl;
            _gameSession.OnChallengeInitiated += ChallengeWindowControl;
            _gameSession.OnQuit += QuitGame;
            _gameSession.OnSave += SaveGame;
            _gameSession.OnLoad += LoadGame;
            _gameSession.OnNewGame += NewGame;
        }

        private void _gameSession_OnNewGame(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void LoadGame(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog =
                new OpenFileDialog
                {
                    InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    Filter = $"Saved games (*.{SAVE_GAME_FILE_EXTENSION})|*.{SAVE_GAME_FILE_EXTENSION}"
                };
            if(openFileDialog.ShowDialog() == true)
            {
                SetActiveGameSessionTo(SaveGameService.LoadLastSavedOrCreateNew(openFileDialog.FileName));
            }
        }
        private void SaveGame(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog =
                new SaveFileDialog
                {
                    InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    Filter = $"Saved games (*.{SAVE_GAME_FILE_EXTENSION})|*.{SAVE_GAME_FILE_EXTENSION}"
                };

            if(saveFileDialog.ShowDialog() == true)
            {
                SaveGameService.Save(_gameSession, saveFileDialog.FileName);
            }
        }
        private void NewGame(object sender, EventArgs e)
        {
            SetActiveGameSessionTo(new GameSession());
        }
        #endregion
    }
}
