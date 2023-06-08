using System.Windows;
using Engine.EventArgs;
using System.Windows.Documents;
using System.Windows.Input;
using Engine.Models;
using Engine.ViewModels;

namespace StoryRPG
{
    /// <summary>
    /// Interaction logic for CharacterWindnow.xaml
    /// </summary>
    public partial class CharacterWindow : Window
    {
        public GameSession Session => DataContext as GameSession;
        public CharacterWindow()
        {
            InitializeComponent();
        }
    }
}
