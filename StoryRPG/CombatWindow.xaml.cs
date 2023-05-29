using System.Windows;
using Engine.EventArgs;
using System.Windows.Documents;
using System.Windows.Input;
using Engine.Models;
using Engine.ViewModels;

namespace StoryRPG
{
    /// <summary>
    /// Interaction logic for CombatWindow.xaml
    /// </summary>
    public partial class CombatWindow : Window
    {
        public GameSession Session => DataContext as GameSession;
        public CombatWindow()
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
