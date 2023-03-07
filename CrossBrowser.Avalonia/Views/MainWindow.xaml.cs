using Avalonia;
using Avalonia.Controls;

namespace CrossBrowser.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif // DEBUG
        }
    }
}