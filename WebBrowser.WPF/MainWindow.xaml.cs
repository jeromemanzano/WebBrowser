using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using ReactiveUI;
using Splat;
using WebBrowser.ViewModels;

namespace WebBrowser.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainViewModel>
    {
        private readonly ListBox _suggestionsListBox;
        private readonly TextBox _addressBarTextBox;
        
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = Locator.Current.GetService<MainViewModel>();
            DataContext = viewModel;
            ViewModel = viewModel;
            _suggestionsListBox = (ListBox)FindName("SuggestionsListBox");
            _addressBarTextBox = (TextBox)FindName("AddressBarTextBox");
        }
        
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Akavache.BlobCache.Shutdown().Wait();
        }

        // We are handling it here since this is probably not needed in other platforms
        private void OnTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && _suggestionsListBox.SelectedIndex < _suggestionsListBox.Items.Count - 1)
            {
                _suggestionsListBox.SelectedIndex++;
                e.Handled = true;
            }
            else if (e.Key == Key.Up && _suggestionsListBox.SelectedIndex > 0)
            {
                _suggestionsListBox.SelectedIndex--;
                e.Handled = true;
            }
            else
            {
                return;
            }
            
            _addressBarTextBox.Text = _suggestionsListBox.SelectedItem.ToString()!;
            _addressBarTextBox.CaretIndex = _addressBarTextBox.Text.Length;
        }
    }
}