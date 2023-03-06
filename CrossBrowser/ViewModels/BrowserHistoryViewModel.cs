using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using CrossBrowser.Entities;
using CrossBrowser.Models;
using CrossBrowser.Services;
using DynamicData;
using ReactiveUI;

namespace CrossBrowser.ViewModels;

public class BrowserHistoryViewModel : BaseViewModel
{
    private readonly IBrowserHistoryService _browserHistoryService;
    private readonly ReadOnlyObservableCollection<HistoryModel> _history;
    
    public ReadOnlyObservableCollection<HistoryModel> History => _history;
    
    public ReactiveCommand<HistoryModel, Unit> DeleteHistoryItem { get; }
    
    public BrowserHistoryViewModel(IBrowserHistoryService browserHistoryService)
    {
        _browserHistoryService = browserHistoryService;
        
        browserHistoryService
            .BrowserHistory
            .Connect()
            .Transform(MapHistoryEntityToHistoryModel)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _history)
            .Subscribe();

        DeleteHistoryItem = ReactiveCommand.CreateFromTask<HistoryModel>(DeleteItemFromHistoryAsync);
    }

    private async Task DeleteItemFromHistoryAsync(HistoryModel historyModel)
    {
        await _browserHistoryService.DeleteItemAsync(historyModel);
    }
    
    private HistoryModel MapHistoryEntityToHistoryModel(HistoryEntity historyEntity)
    {
        var uri = new Uri(historyEntity.Url);
        
        return new HistoryModel()
        {
            Url = historyEntity.Url,
            Id = historyEntity.Id,
            Host = uri.Host,
            Query = historyEntity.Query,
            LocalTimeStamp = historyEntity.UtcTimeStamp.ToLocalTime()
        };
    }
}