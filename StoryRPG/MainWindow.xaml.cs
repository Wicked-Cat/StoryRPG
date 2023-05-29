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

        bool CombatWindowOpen;
        public MainWindow()
        {
            InitializeComponent();
            _gameSession.OnMessageRaised += OnGameMessageRaised;
            _gameSession.OnMonsterAppeared += CombatWindowControl;

            DataContext = _gameSession; //built in propery for xaml files
        }

        private void OnFight_DisplayComabtScreen(object sender, RoutedEventArgs e)
        {
            combatWindow = new CombatWindow();
            combatWindow.Owner = this;
            combatWindow.DataContext = _gameSession;
            CombatWindowOpen = true;
            combatWindow.ShowDialog();
        }

        private void CloseCombatScreen()
        {
            CombatWindowOpen = false;
            combatWindow.Close();
        }
        private void OnGameMessageRaised(object sender, GameMessageEventArgs e)
        {
            if (CombatWindowOpen)
            {
                combatWindow.GameMessages.Document.Blocks.Add(new Paragraph(new Run(e.Message)));
                combatWindow.GameMessages.ScrollToEnd();
                GameMessages.Document.Blocks.Add(new Paragraph(new Run(e.Message)));
                GameMessages.ScrollToEnd();
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

        public void CombatWindowControl(object sender, OnMonsterEventArgs monster)
        {
            if (monster.Monster != null)
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
