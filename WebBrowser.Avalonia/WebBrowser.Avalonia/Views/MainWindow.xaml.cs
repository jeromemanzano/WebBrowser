using Avalonia;
using Avalonia.Controls;

namespace WebBrowser.Avalonia.Views
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