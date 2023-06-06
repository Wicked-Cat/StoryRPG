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

        private bool IsCombatWindowOpen;
        public MainWindow()
        {
            InitializeComponent();
            _gameSession.OnMessageRaised += OnGameMessageRaised;
            _gameSession.OnEncounterEngaged += CombatWindowControl;

            DataContext = _gameSession; //built in propery for xaml f/iles
        }

        private void OnFight_DisplayComabtScreen(object sender, RoutedEventArgs e)
        {
            IsCombatWindowOpen = true;
            combatWindow = new CombatWindow();
            combatWindow.Owner = this;
            combatWindow.DataContext = _gameSession;
            combatWindow.ShowDialog();
        }

        private void CloseCombatScreen()
        {
            if (combatWindow != null)
            {
                IsCombatWindowOpen = false;
                combatWindow.Close();
            }
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

        public void CombatWindowControl(object sender, OnEncounterEventArgs encounter)
        {
            if (encounter.Encounter != null && _gameSession.AreAllMonstersDead == false)
            {
                OnFight_DisplayComabtScreen(this, new RoutedEventArgs());
            }
            else
            {
                CloseCombatScreen();
            }
        }


    }
}
