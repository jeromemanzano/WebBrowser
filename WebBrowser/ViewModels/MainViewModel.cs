using System.Reactive;
using System.Reactive.Linq;
using System.Web;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using WebBrowser.Extensions;

namespace WebBrowser.ViewModels;

public class MainViewModel : BaseViewModel
{
    public ReactiveCommand<string,Unit> Go { get; }
    
    [Reactive]
    public string BrowserAddress { get; set; } // This is the address that will be displayed in the browser
    
    [Reactive]
    public string AddressBarText { get; set; } // This is the text shown in the address bar

    public MainViewModel()
    {
        var canGo = this.WhenAnyValue(x => x.AddressBarText, x => !string.IsNullOrWhiteSpace(x));
        Go = ReactiveCommand.Create<string>(GoImpl, canGo);
        
        this.WhenAnyValue(x => x.BrowserAddress)
            .Where(text => text != AddressBarText)
            .Subscribe(text => AddressBarText = text);
    }
    
    private void GoImpl(string queryString)
    {
        BrowserAddress = queryString.IsValidUrl() 
            ? queryString 
            : $"https://www.duckduckgo.com/?q={HttpUtility.UrlEncode(queryString)}";
    }
}