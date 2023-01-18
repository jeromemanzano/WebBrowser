using System.Web;
using Newtonsoft.Json;

namespace WebBrowser.Services.Implementation;

public class DuckDuckGoApiService : IDuckDuckGoApiService
{
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://ac.duckduckgo.com/")
    };
    
    public async Task<IEnumerable<string>> GetAutoCompleteSuggestionsAsync(string searchTerm, CancellationToken token)
    {
        var encodedQuery = HttpUtility.UrlEncode(searchTerm);
        using var response = await _httpClient.GetAsync($"ac/?q={encodedQuery}&type=list", token);

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(token);
            var deserializedContent = JsonConvert.DeserializeObject<IEnumerable<object>>(content);
            return JsonConvert.DeserializeObject<IEnumerable<string>>(deserializedContent?.LastOrDefault()?.ToString() ?? string.Empty) 
                   ?? Enumerable.Empty<string>();
        }
        
        throw new HttpRequestException(response.ReasonPhrase);
    }
}