using CrossBrowser.Entities;
using CrossBrowser.Models;
using DynamicData;

namespace CrossBrowser.Services;

public interface IBrowserHistoryService
{
    /// <summary>
    /// Observable cache of <see cref="HistoryEntity"/> items
    /// </summary>
    IObservableCache<HistoryEntity, string> BrowserHistory { get; }

    /// <summary>
    /// Adds the search term and url to the <see cref="BrowserHistory"/>
    /// </summary>
    /// <param name="searchTerm">Unencoded search term to be added</param>
    /// <param name="url">Loaded url string from the query string</param>
    Task AddSearchToBrowserHistoryAsync(string searchTerm, string url);
    
    /// <summary>
    /// Adds the loaded url to the <see cref="BrowserHistory"/>
    /// </summary>
    /// <param name="url">Loaded url string to be added</param>
    Task AddWebsiteToHistoryAsync(string url);
    
    /// <summary>
    /// Clears the entire browser history in cache and database
    /// </summary>
    /// <returns></returns>
    Task DeleteAllAsync();
    
    /// <summary>
    /// Deletes the specified <see cref="HistoryModel"/> from the <see cref="BrowserHistory"/>
    /// </summary>
    /// <param name="historyItem"></param>
    /// <returns></returns>
    Task DeleteItemAsync(HistoryModel historyItem);
}