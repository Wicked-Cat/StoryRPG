using System.Windows;
using Engine.EventArgs;
using System.Windows.Documents;
using System.Windows.Input;
using Engine.Models;
using Engine.ViewModels;

namespace StoryRPG
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class InventoryWindow : Window
    {
        public GameSession Session => DataContext as GameSession;
        public InventoryWindow()
        {
            InitializeComponent();
        }
    }
}
