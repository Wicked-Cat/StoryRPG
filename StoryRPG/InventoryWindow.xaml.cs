using Engine.ViewModels;
using System.Windows;

namespace StoryRPG
{
    public partial class InventoryWindow : Window
    {
        public GameSession Session => DataContext as GameSession;
        public InventoryWindow()
        {
            InitializeComponent();
        }
    }
}
