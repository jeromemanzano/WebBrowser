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
    [TestCase("https://accounts.google.com/v3/signin/test?dsh=S1929591889%3A1674000243309800&continue=https%3A%2F%2Fmail.google.com%2Fmail%2Fu%2F0%2F&emr=1&followup=https%3A%2F%2Fmail.google.com%2Fmail%2Fu%2F0%2F&osid=1&passive=12096440&service=mail&flowName=GlifWebSignIn&flowEntry=ServiceLogin&ifkv=AWnogHc48CPsQ5AUQ_eE_looo3CKNLSoOPsdfUhKFga984LuaGHC7Q6ym43oXeSfh0CQ")]
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