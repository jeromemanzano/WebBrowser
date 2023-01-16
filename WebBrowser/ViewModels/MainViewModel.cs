using System.Reactive;
using System.Reactive.Linq;
using System.Web;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using WebBrowser.Extensions;
using WebBrowser.Services;

namespace WebBrowser.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly IBrowserHistoryService _historyService;
    public ReactiveCommand<string,Unit> Go { get; }
    
    public ReactiveCommand<Unit, Unit> ClearHistory { get; }

    [Reactive]
    public string BrowserAddress { get; set; } // This is the address that will be displayed in the browser
    
    [Reactive]
    public string AddressBarText { get; set; } // This is the text shown in the address bar

    public BrowserHistoryViewModel BrowserHistory { get; }

    public MainViewModel(IBrowserHistoryService browserHistoryService)
    {
        _historyService = browserHistoryService;
        BrowserHistory = Locator.Current.GetService<BrowserHistoryViewModel>()!;
        
        var canGo = this.WhenAnyValue(x => x.AddressBarText, x => !string.IsNullOrWhiteSpace(x));
        Go = ReactiveCommand.Create<string>(GoImpl, canGo);

        ClearHistory = ReactiveCommand.CreateFromTask(ClearHistoryImplAsync);
        
        this.WhenAnyValue(x => x.BrowserAddress)
            .Where(x => x != AddressBarText) 
            .SelectMany(HandleNewUrlAsync)
            .Subscribe();
    }
    
    private async Task ClearHistoryImplAsync()
    {
        await _historyService.DeleteAllAsync();
    }
    
    private async Task<Unit> HandleNewUrlAsync(string url)
    {
        if (AddressBarText.IsValidUrl())
        {
            await _historyService.AddWebsiteToHistoryAsync(url);
        }
        else
        {
            await _historyService.AddSearchToBrowserHistoryAsync(AddressBarText, url);
        }
        
        AddressBarText = url;
        return Unit.Default;
    }
    
    private void GoImpl(string queryString)
    {
        BrowserAddress = queryString.IsValidUrl()
            ? queryString
            : $"https://duckduckgo.com/?q={HttpUtility.UrlEncode(queryString)}"; // When user types a query, we search it on duckduckgo.com
    }
}