using System.Text.RegularExpressions;

namespace CrossBrowser.Extensions;

public static class StringExtensions
{
    private static readonly Regex _urlPattern = new(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!%\$&'\(\)\*\+,;=.]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    /// <summary>
    /// Checks if the string is a valid URL.
    /// </summary>
    /// <param name="url">String URL to validate</param>
    /// <returns>Returns true if string is a valid URL, otherwise returns false.</returns>
    public static bool IsValidUrl(this string? url)
    {
        if (url is null)
        {
            return false;
        }
        
        return _urlPattern.IsMatch(url);
    }
    
    /// <summary>
    /// Ensures that string url starts with http:// or https://
    /// </summary>
    /// <param name="url">String url to be formatted</param>
    /// <returns>Formatted url that starts with http:// or https:// </returns>
    public static string FormatStringUrl(string url)
    {
        if (url.StartsWith(@"http://") || url.StartsWith(@"https://"))
        {
            return url;
        }

        return $"https://{url}";
    }
}