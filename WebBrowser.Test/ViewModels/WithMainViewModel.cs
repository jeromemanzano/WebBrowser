using System.Web;
using DynamicData;
using ReactiveUI;
using WebBrowser.Extensions;
using WebBrowser.Services;
using WebBrowser.ViewModels;
using Moq;

namespace WebBrowser.Test.ViewModels;

public class WithMainViewModel : BaseViewModelTest<MainViewModel>
{
    private Mock<IBrowserHistoryService> _browserHistoryServiceMock;
    
    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase(" ", false)]
    [TestCase("valid", true)]
    public void Go_Command_Should_Only_Execute_When_AddressBarText_Is_Not_Null_Empty_Or_WhiteSpace(string text, bool expectedCanExecute)
    {
        ViewModel.AddressBarText = text;

        ViewModel
            .Go
            .CanExecute
            .ToObservableChangeSet()
            .Bind(out var canExecute)
            .Subscribe();

        Assert.That(canExecute.Count, Is.EqualTo(1));
        Assert.That(canExecute.Last, Is.EqualTo(expectedCanExecute));
    }
    
    [TestCase("http://duckduckgo.com")]
    [TestCase("https://duckduckgo.com")]
    [TestCase("https://duckduckgo.com/")]
    [TestCase("https://duckduckgo.com/?q=hello")]
    [TestCase("duckduckgo.com")]
    [TestCase("www.duckduckgo.com")]
    [TestCase("127.0.0.1")]
    [TestCase("http://127.0.0.1")]
    public void Go_Command_Should_Copy_Url_To_BrowserAddress_When_Text_Is_Valid_Url(string text)
    {
        ViewModel
            .Go
            .Execute(text)
            .Subscribe();

        Assert.IsTrue(text.IsValidUrl());
        Assert.That(ViewModel.BrowserAddress, Is.EqualTo(text));
    }

    [Test]
    public void When_Text_Is_Not_Valid_Url_And_Contains_Url_Encoded_String_Go_Command_Should_Use_Formatted_Text()
    {
        var text= "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_.!*()";
        ViewModel
            .Go
            .Execute(text)
            .Subscribe();

        Assert.IsFalse(text.IsValidUrl());
        Assert.That(ViewModel.BrowserAddress, Is.EqualTo($"https://duckduckgo.com/?q={text}"));
    }
    
    [Test]
    public void When_Text_Is_Not_Valid_Url_And_Not_Url_Encoded_Go_Command_Should_Use_Formatted_Url_Encoded_Text()
    {
        var text= "this is a not url encoded string";
        var urlEncodedText = HttpUtility.UrlEncode(text);
        ViewModel
            .Go
            .Execute(text)
            .Subscribe();

        Assert.IsFalse(text.IsValidUrl());
        Assert.That(urlEncodedText, Is.Not.EqualTo(text));
        Assert.That(ViewModel.BrowserAddress, Is.EqualTo($"https://duckduckgo.com/?q={urlEncodedText}"));
    }
    
    [Test]
    public void When_BrowserAddress_Is_Changed_And_Value_Is_Not_Equal_To_AddressBarText_It_Should_Update_AddressBarText()
    {
        var initialAddressBarText = "initial address bar text";
        ViewModel.AddressBarText = initialAddressBarText;
        
        this.WhenAnyValue(x => x.ViewModel.AddressBarText)
            .ToObservableChangeSet()
            .Bind(out var addressBarChange)
            .Subscribe();
        
        ViewModel.BrowserAddress = "https://duckduckgo.com";

        Assert.That(addressBarChange.Count, Is.EqualTo(2)); // AddressBarText is changed twice, once when it is set and once when BrowserAddress is changed
        Assert.That(ViewModel.AddressBarText, Is.Not.EqualTo(initialAddressBarText));
        Assert.That(ViewModel.AddressBarText, Is.EqualTo(ViewModel.BrowserAddress));
    }
    
    [Test]
    public void When_BrowserAddress_Is_Changed_And_Value_Is_Equal_To_AddressBarText_It_Should_Not_Update_AddressBarText()
    {
        var initialAddressBarText = "initial address bar text";
        ViewModel.AddressBarText = initialAddressBarText;
        
        this.WhenAnyValue(x => x.ViewModel.AddressBarText)
            .ToObservableChangeSet()
            .Bind(out var addressBarChange)
            .Subscribe();
        
        ViewModel.BrowserAddress = initialAddressBarText;
        Assert.That(addressBarChange.Count, Is.EqualTo(1)); // AddressBarText is changed once, when it is set
    }

    [TestCase("http://duckduckgo.com")]
    [TestCase("https://duckduckgo.com")]
    [TestCase("https://duckduckgo.com/")]
    [TestCase("https://duckduckgo.com/?q=hello")]
    [TestCase("duckduckgo.com")]
    [TestCase("www.duckduckgo.com")]
    [TestCase("127.0.0.1")]
    [TestCase("http://127.0.0.1")]
    public void When_BrowserAddress_Changed_And_AddressBarText_Is_Valid_Url_It_Should_Call_IBrowserHistoryService_AddWebsiteToHistory_With_BrowserAddress_Once(string validUrl)
    {
        ViewModel.AddressBarText = validUrl;

        ViewModel.BrowserAddress = "sample.com";
        
        _browserHistoryServiceMock
            .Verify(service => service.AddWebsiteToHistoryAsync(ViewModel.BrowserAddress), Times.Once);
    }
    
    [Test]
    public void When_BrowserAddress_Changed_And_AddressBarText_Is_Not_Valid_Url_It_Should_Not_Call_IBrowserHistoryService_AddWebsiteToHistory_With_BrowserAddress()
    {
        ViewModel.AddressBarText = "this is not a valid url";

        ViewModel.BrowserAddress = "sample.com";
        
        _browserHistoryServiceMock
            .Verify(service => service.AddWebsiteToHistoryAsync(It.IsAny<string>()), Times.Never);
    }
    
    [Test]
    public void ClearHistory_Should_Call_IBrowserHistoryService_DeleteAll_Once()
    {
        ViewModel.ClearHistory.Execute().Subscribe();
        
        _browserHistoryServiceMock
            .Verify(service => service.DeleteAllAsync(), Times.Once);
    }
    
    protected override MainViewModel CreateViewModel()
    {
        _browserHistoryServiceMock = new Mock<IBrowserHistoryService>();
        return new MainViewModel(_browserHistoryServiceMock.Object);
    }
}