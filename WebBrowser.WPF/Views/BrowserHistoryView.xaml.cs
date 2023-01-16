using ReactiveUI;
using WebBrowser.ViewModels;

namespace WebBrowser.WPF.Views;

public partial class BrowserHistoryView : ReactiveUserControl<BrowserHistoryViewModel>
{
    public BrowserHistoryView()
    {
        InitializeComponent();
    }
}