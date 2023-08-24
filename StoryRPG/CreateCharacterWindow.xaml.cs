using Engine.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace StoryRPG
{
    /// <summary>
    /// Interaction logic for CreateCharacterWindow.xaml
    /// </summary>
    public partial class CreateCharacterWindow : Window
    {
        public GameSession Session => DataContext as GameSession;
        public CreateCharacterWindow()
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
