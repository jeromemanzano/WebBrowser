using System.Reactive.Threading.Tasks;
using DynamicData;
using WebBrowser.Entities;
using WebBrowser.Models;

namespace WebBrowser.Services.Implementation;

public class BrowserHistoryService : IBrowserHistoryService
{
    private readonly IRepositoryService<HistoryEntity> _historyRepository;
    private readonly SourceCache<HistoryEntity, string> _browserHistory = new(x => x.Id);
    private HistoryEntity _lastAddedEntity;
    public IObservableCache<HistoryEntity, string> BrowserHistory => _browserHistory.AsObservableCache();

    public BrowserHistoryService(IRepositoryService<HistoryEntity> historyRepository)
    {
        _historyRepository = historyRepository;

        _historyRepository
            .GetAllAsync()
            .ToObservable()
            .Subscribe(_browserHistory.AddOrUpdate);
    }

    public async Task AddSearchToBrowserHistoryAsync(string searchTerm, string url)
    {
        
        var historyEntity = new HistoryEntity()
        {
            Query = searchTerm,
            Url = url,
            UtcTimeStamp = DateTime.UtcNow
        };
        
        await AddToHistoryAsync(historyEntity);
    }

    public async Task AddWebsiteToHistoryAsync(string url)
    {
        // This is for handling reload, so we don't add the same url twice.
        if (url == _lastAddedEntity?.Url)
        {
            return;
        }
        
        var uri = new Uri(url);

        var historyEntity = new HistoryEntity()
        {
            Query = uri.AbsolutePath,
            Url = url,
            UtcTimeStamp = DateTime.UtcNow
        };

        // Sometimes duckduckgo will add additional query parameters to the url, so we need to check if the url is already in the history and update it.
        if (uri.Host == "duckduckgo.com" 
            && _lastAddedEntity is not null 
            && url.Contains(_lastAddedEntity.Url))
        {
            _lastAddedEntity.Url = url;
            await _historyRepository.UpdateAsync(_lastAddedEntity);
            return;
        }

        await AddToHistoryAsync(historyEntity);
    }

    public async Task DeleteAllAsync()
    {
        _browserHistory.Clear();
        await _historyRepository.RemoveAllAsync();
    }

    public async Task DeleteItemAsync(HistoryModel historyItem)
    {
        _browserHistory.Remove(historyItem.Id);
        await _historyRepository.RemoveByIdAsync(historyItem.Id);
    }
    
    private async Task AddToHistoryAsync(HistoryEntity historyEntity)
    {
        _lastAddedEntity = historyEntity;
        _browserHistory.AddOrUpdate(historyEntity);
        await _historyRepository.AddAsync(historyEntity);
    }
}