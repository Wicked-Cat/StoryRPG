using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Engine.EventArgs;
using Engine.Models;
using Engine.ViewModels;

namespace StoryRPG
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       private readonly GameSession _gameSession = new GameSession();
        CombatWindow combatWindow;
        InventoryWindow inventoryWindow;
        CharacterWindow characterWindow;
        EquipmentWindow equipmentWindow;
        TradeWindow tradeWindow;

        private bool IsCombatWindowOpen;
        public MainWindow()
        {
            InitializeComponent();
            _gameSession.OnMessageRaised += OnGameMessageRaised;
            _gameSession.OnEncounterEngaged += CombatWindowControl;
            _gameSession.OnInventoryOpened += OpenInventoryScreen;
            _gameSession.OnCharacterOpened += OpenCharacterScreen;
            _gameSession.OnTradeInitiated += TradeWindowControl;

            DataContext = _gameSession; //built in propery for xaml f/iles
        }
        private void OnGameMessageRaised(object sender, GameMessageEventArgs e)
        {
            if (IsCombatWindowOpen)
            {
                GameMessages.Document.Blocks.Add(new Paragraph(new Run(e.Message)));
                GameMessages.ScrollToEnd();
                combatWindow.GameMessages.Document.Blocks.Add(new Paragraph(new Run(e.Message)));
                combatWindow.GameMessages.ScrollToEnd();
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
        }
        private void CombatWindowControl(object sender, OnEncounterEventArgs encounter)
        {
            if (encounter.Encounter != null && _gameSession.AreAllMonstersDead == false)
            {
                IsCombatWindowOpen = true;
                combatWindow = new CombatWindow();
                combatWindow.Owner = this;
                combatWindow.DataContext = _gameSession;
                combatWindow.ShowDialog();
            }
            else
            {
                if (combatWindow != null)
                {
                    IsCombatWindowOpen = false;
                    combatWindow.Close();
                }
            }
        }
        private void TradeWindowControl(object sender, OnTradeEventArgs trade)
        {
            if (_gameSession.CurrentMerchant != null)
            {
                tradeWindow = new TradeWindow();
                tradeWindow.Owner = this;
                tradeWindow.DataContext = _gameSession;
                tradeWindow.ShowDialog();
            }
            else
            {
                if (tradeWindow != null)
                {
                    tradeWindow.Close();
                }
            }
        }
        
        #endregion

    }
}
