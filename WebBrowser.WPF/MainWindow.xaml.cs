using ReactiveUI;
using Splat;
using WebBrowser.Services;
using WebBrowser.ViewModels;

namespace WebBrowser.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = Locator.Current.GetService<MainViewModel>();
            DataContext = viewModel;
            ViewModel = viewModel;
        }
    }
}