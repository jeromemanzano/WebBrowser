using System;
using System.Windows.Controls;
using System.Windows.Input;
using CefSharp.Wpf;
using ReactiveUI;
using Splat;
using WebBrowser.ViewModels;

namespace WebBrowser.WPF.Views;

public partial class TabContentView : ReactiveUserControl<TabContentViewModel>
{
        private readonly ListBox _suggestionsListBox;
        private readonly TextBox _addressBarTextBox;
        private readonly ChromiumWebBrowser _webBrowser;

        public TabContentView()
        {
            InitializeComponent();
            var viewModel = Locator.Current.GetService<TabContentViewModel>();
            DataContext = viewModel;
            ViewModel = viewModel;
            _suggestionsListBox = (ListBox) FindName("SuggestionsListBox");
            _addressBarTextBox = (TextBox) FindName("AddressBarTextBox");
            _webBrowser = (ChromiumWebBrowser) FindName("BrowserPane");
        }

        // We are handling it here since this is probably not needed in other platforms
        private void OnTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down && _suggestionsListBox.SelectedIndex < _suggestionsListBox.Items.Count - 1)
            {
                _suggestionsListBox.SelectedIndex++;
            }
            else if (e.Key == Key.Up && _suggestionsListBox.SelectedIndex > 0)
            {
                _suggestionsListBox.SelectedIndex--;
            }
            else
            {
                return;
            }

            _addressBarTextBox.Text = _suggestionsListBox.SelectedItem.ToString()!;
            _addressBarTextBox.CaretIndex = _addressBarTextBox.Text.Length;
            e.Handled = true;
        }

        private void ListBoxItem_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBoxItem item)
            {
                var suggestion = item.DataContext.ToString()!;
                ViewModel!.SelectedSuggestion = suggestion;
                _addressBarTextBox.Text = suggestion;
                _addressBarTextBox.CaretIndex = _addressBarTextBox.Text.Length;

                ViewModel.Go.Execute(suggestion).Subscribe();
                e.Handled = true;
            }
        }

        private void OnUserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers is ModifierKeys.Alt && e.SystemKey == Key.Left && _webBrowser.BackCommand.CanExecute(null))
            {
                _webBrowser.BackCommand.Execute(null);
                e.Handled = true;
            }
            else if (Keyboard.Modifiers is ModifierKeys.Alt && e.SystemKey == Key.Right && _webBrowser.ForwardCommand.CanExecute(null))
            {
                _webBrowser.ForwardCommand.Execute(null);
                e.Handled = true;
            }
            else if (Keyboard.Modifiers is ModifierKeys.Control && e.Key == Key.R && _webBrowser.ReloadCommand.CanExecute(null))
            {
                _webBrowser.ReloadCommand.Execute(null);
                e.Handled = true;
            }
            else if (e.Key == Key.F5 && _webBrowser.ReloadCommand.CanExecute(null))
            {
                _webBrowser.ReloadCommand.Execute(null);
                e.Handled = true;
            }
        }
}