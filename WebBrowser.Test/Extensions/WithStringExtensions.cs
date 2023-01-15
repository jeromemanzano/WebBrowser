using WebBrowser.Extensions;

namespace WebBrowser.Test.Extensions;

public class WithStringExtensions
{
    [TestCase("http://duckduckgo.com")]
    [TestCase("https://duckduckgo.com")]
    [TestCase("https://duckduckgo.com/")]
    [TestCase("https://duckduckgo.com/?q=hello")]
    [TestCase("duckduckgo.com")]
    [TestCase("www.duckduckgo.com")]
    [TestCase("127.0.0.1")]
    [TestCase("http://127.0.0.1")]
    public void IsValidUrl_Should_Return_True_When_Url_Is_Valid(string url)
    {
        Assert.IsTrue(url.IsValidUrl());
    }
    
    [TestCase(null)]
    [TestCase("text")]
    [TestCase("This is a long search string")]
    public void IsValidUrl_Should_Return_False_When_Url_Is_Not_Valid(string url)
    {
        Assert.IsFalse(url.IsValidUrl());
    }
}