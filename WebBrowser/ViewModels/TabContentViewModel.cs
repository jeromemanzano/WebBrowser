using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Web;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using WebBrowser.Extensions;
using WebBrowser.Services;

namespace WebBrowser.ViewModels;

public class TabContentViewModel : BaseViewModel
{
    private readonly IBrowserHistoryService _historyService;
    private CancellationTokenSource _cts;
    public ReactiveCommand<string, Unit> Go { get; }
    public ReactiveCommand<Unit, Unit> ClearHistory { get; }
    public ObservableCollection<string> Suggestions { get; } = new();
    
    [Reactive] public string BrowserAddress { get; set; } // This is the address that will be displayed in the browser

    [Reactive] public string AddressBarText { get; set; } // This is the text shown in the address bar

    [Reactive] public string SelectedSuggestion { get; set; }

    public BrowserHistoryViewModel? BrowserHistory { get; }

    public TabContentViewModel(IBrowserHistoryService browserHistoryService,
        IAutoCompleteService autoCompleteService,
        IScheduler? taskPoolScheduler = null)
    {
        _historyService = browserHistoryService;
        BrowserHistory = Locator.Current.GetService<BrowserHistoryViewModel>();

        var canGo = this.WhenAnyValue(x => x.AddressBarText, x => !string.IsNullOrWhiteSpace(x));
        Go = ReactiveCommand.Create<string>(GoImpl, canGo);

        var canClear = BrowserHistory
            ?.History
            .ToObservableChangeSet()
            .ToCollection()
            .Select(history => history.Count > 0) ?? Observable.Return(false);
        ClearHistory = ReactiveCommand.CreateFromTask(ClearHistoryImplAsync, canClear);

        this.WhenAnyValue(x => x.BrowserAddress)
            .Where(x => x != AddressBarText)
            .SelectMany(HandleNewUrlAsync)
            .Subscribe();

        this.WhenAnyValue(x => x.AddressBarText)
            .Where(searchTerm => searchTerm != SelectedSuggestion)
            .Do(_ => Suggestions.Clear())
            .Where(searchTerm => searchTerm != BrowserAddress)
            .Throttle(TimeSpan.FromMilliseconds(300), taskPoolScheduler ?? RxApp.TaskpoolScheduler) //based on average reaction time here https://humanbenchmark.com/tests/reactiontime/statistics
            .DistinctUntilChanged()
            .Select(searchTerm =>
            {
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
                return (searchTerm, _cts.Token);
            })
            .SelectMany(x => autoCompleteService.GetSuggestions(x.searchTerm, x.Token).TakeUntil(Go))
            .WhereNotNull()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(Suggestions.AddRange)
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
        if (string.IsNullOrWhiteSpace(queryString))
            return;
        
        Suggestions.Clear();

        BrowserAddress = queryString.IsValidUrl()
            ? queryString
            : $"https://duckduckgo.com/?q={HttpUtility.UrlEncode(queryString)}"; // When user types a query, we search it on duckduckgo.com
    }
}