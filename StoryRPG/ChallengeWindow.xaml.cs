﻿using Engine.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StoryRPG
{
    /// <summary>
    /// Interaction logic for ChallengeWindow.xaml
    /// </summary>
    public partial class ChallengeWindow : Window
    {
        public GameSession Session => DataContext as GameSession;
        public ChallengeWindow()
        {
            InitializeComponent();
        }

        private void OnEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string input = TextBoxInput.Text;
                Session.WindowToSession(input);
                TextBoxInput.Text = "";
            }
        }
    }
}
