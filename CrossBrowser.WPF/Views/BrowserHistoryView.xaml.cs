using ReactiveUI;
using CrossBrowser.ViewModels;

namespace CrossBrowser.WPF.Views;

public partial class BrowserHistoryView : ReactiveUserControl<BrowserHistoryViewModel>
{
    public BrowserHistoryView()
    {
        InitializeComponent();
    }
}