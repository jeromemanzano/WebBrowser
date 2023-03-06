using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CrossBrowser.Entities;
using DynamicData;
using ReactiveUI;
using Splat;

namespace CrossBrowser.Services.Implementation;

public class AutoCompleteService : IAutoCompleteService, IEnableLogger
{
    private readonly IDuckDuckGoApiService _duckDuckGoApiService;
    private readonly ReadOnlyObservableCollection<HistoryEntity> _history;

    public AutoCompleteService(IDuckDuckGoApiService duckDuckGoApiService,
        IBrowserHistoryService browserHistoryService)
    {
        _duckDuckGoApiService = duckDuckGoApiService;

        browserHistoryService
            .BrowserHistory
            .Connect()
            .Bind(out _history)
            .Subscribe();
    }

    public IObservable<IReadOnlyCollection<string>?> GetSuggestions(string searchTerm, CancellationToken token)
    {
        var apiResultObservable = Observable
            .FromAsync(() => _duckDuckGoApiService.GetAutoCompleteSuggestionsAsync(searchTerm, token))
            .LoggedCatch(this, Observable.Return(Enumerable.Empty<string>())); // We don't want to crash the app if the API fails so we catch the exception and return an empty list

        var localSearchResultObservable = Observable.Return(_history
            .Select(entity => GetMatchingTextWithDate(entity, searchTerm))
            .Where(result => result.HasValue)
            .OrderByDescending(result => result!.Value.dateTime)
            .Take(5)
            .Select(result => result!.Value.Text));

        return apiResultObservable
            .Merge(localSearchResultObservable)
            .Select(result => result.ToList().AsReadOnly());
    }

    private (string Text, DateTime dateTime)? GetMatchingTextWithDate(HistoryEntity entity, string searchTerm)
    {
        var uri = new Uri(entity.Url);
        
        if (uri.AbsolutePath == entity.Query)
        {
            return entity.Url.Contains(searchTerm) ? (entity.Url, entity.UtcTimeStamp) : null;
        }
        
        return entity.Query.Contains(searchTerm) ? (entity.Query, entity.UtcTimeStamp) : null;
    }
}
