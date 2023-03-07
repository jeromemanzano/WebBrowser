
namespace CrossBrowser.Services;

public interface IAutoCompleteService
{
    /// <summary>
    /// Retrieves search suggestion from ac.duckduckgo.com and local storage
    /// </summary>
    /// <param name="searchTerm">the text used for searching</param>
    /// <param name="token">token for cancellation</param>
    /// <returns>Read only collection of string suggestions</returns>
    IObservable<IReadOnlyCollection<string>?> GetSuggestions(string searchTerm, CancellationToken token);
}