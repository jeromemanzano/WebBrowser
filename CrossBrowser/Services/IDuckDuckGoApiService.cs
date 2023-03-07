namespace CrossBrowser.Services;

public interface IDuckDuckGoApiService
{

    /// <summary>
    /// Retrieves auto complete suggestion using duckduckgo api
    /// </summary>
    /// <param name="searchTerm">Text that will be passed on the the request</param>
    /// <param name="token">token for cancellation</param>
    /// <returns>IEnumerable of string result of suggestion</returns>
    Task<IEnumerable<string>> GetAutoCompleteSuggestionsAsync(string searchTerm, CancellationToken token);
}