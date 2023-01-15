using System.Text.RegularExpressions;

namespace WebBrowser.Extensions;

public static class StringExtensions
{
    private static readonly Regex _urlPattern = new(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$",
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
}