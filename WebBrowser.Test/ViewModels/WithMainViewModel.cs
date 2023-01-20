using System.Reactive.Linq;
using System.Web;
using DynamicData;
using Microsoft.Reactive.Testing;
using ReactiveUI;
using WebBrowser.Extensions;
using WebBrowser.Services;
using WebBrowser.ViewModels;
using Moq;
using ReactiveUI.Testing;

namespace WebBrowser.Test.ViewModels;

public class WithMainViewModel : BaseViewModelTest<MainViewModel>
{
    private Mock<IBrowserHistoryService> _browserHistoryService;
    private Mock<IAutoCompleteService> _autoCompleteService;

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
        
        _browserHistoryService
            .Verify(service => service.AddWebsiteToHistoryAsync(ViewModel.BrowserAddress), Times.Once);
    }
    
    [Test]
    public void When_BrowserAddress_Changed_And_AddressBarText_Is_Not_Valid_Url_It_Should_Not_Call_IBrowserHistoryService_AddWebsiteToHistory_With_BrowserAddress()
    {
        ViewModel.AddressBarText = "this is not a valid url";

        ViewModel.BrowserAddress = "sample.com";
        
        _browserHistoryService
            .Verify(service => service.AddWebsiteToHistoryAsync(It.IsAny<string>()), Times.Never);
    }
    
    [Test]
    public void ClearHistory_Should_Call_IBrowserHistoryService_DeleteAll_Once()
    {
        ViewModel.ClearHistory.Execute().Subscribe();
        
        _browserHistoryService
            .Verify(service => service.DeleteAllAsync(), Times.Once);
    }

    [Test]
    public void Go_Should_Clear_Suggestions()
    {
        ViewModel.Suggestions.AddRange(Enumerable.Range(0, 10).Select(_ => Guid.NewGuid().ToString()));
        
        ViewModel.Go.Execute("search term").Subscribe();
        
        CollectionAssert.IsEmpty(ViewModel.Suggestions);
    }
    
    [Test]
    public void When_AddressBarText_Changed_Suggestions_Should_Be_Cleared()
    {
        ViewModel.Suggestions.AddRange(Enumerable.Range(0, 10).Select(_ => Guid.NewGuid().ToString()));

        ViewModel.AddressBarText = "test";
        
        CollectionAssert.IsEmpty(ViewModel.Suggestions);
    }

    [Test]
    public void When_AddressBarText_Changed_And_Text_Is_Same_As_SelectedSuggestion_Suggestions_Should_Not_Be_Cleared()
    {
        var suggestions = Enumerable.Range(0, 10).Select(_ => Guid.NewGuid().ToString()).ToList();
        ViewModel.Suggestions.AddRange(suggestions);
        var text = "test";

        ViewModel.SelectedSuggestion = text;
        ViewModel.AddressBarText = text;
        
        
        CollectionAssert.IsNotEmpty(ViewModel.Suggestions);
        CollectionAssert.AreEquivalent(suggestions, ViewModel.Suggestions);
    }

    [Test]
    public void While_AddressBarText_Is_Changing_Should_Wait_For_Throttle_Before_Calling_GetSuggestions()
    {
        new TestScheduler().With(scheduler =>
        {
            ViewModel = new MainViewModel(_browserHistoryService.Object, _autoCompleteService.Object, scheduler);
            _autoCompleteService
                .Setup(service => service.GetSuggestions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Observable.Return(Enumerable.Empty<string>().ToList().AsReadOnly()));

            var finalText = "final text";
            ViewModel.AddressBarText = "first text";
            ViewModel.AddressBarText = "second text";
            ViewModel.AddressBarText = finalText;
            scheduler.AdvanceByMs(299);
            
            // Should not call GetSuggestions before 300ms throttle
            _autoCompleteService.Verify(
                service => service.GetSuggestions(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            
            scheduler.AdvanceByMs(1);
            _autoCompleteService.Verify(
                service => service.GetSuggestions(finalText, It.IsAny<CancellationToken>()), Times.Once);
        });
    }
    
    [Test]
    public void GetSuggestions_Result_Should_Be_Assigned_To_Suggestions()
    {
        new TestScheduler().With(scheduler =>
        {
            ViewModel = new MainViewModel(_browserHistoryService.Object, _autoCompleteService.Object, scheduler);
            var suggestions = new List<string> {"suggestion 1", "suggestion 2"};
            _autoCompleteService
                .Setup(service => service.GetSuggestions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Observable.Return(suggestions.AsReadOnly()));

            ViewModel.AddressBarText = "test";
            scheduler.AdvanceByMs(301);

            CollectionAssert.AreEqual(suggestions, ViewModel.Suggestions);
        });
    }

    protected override MainViewModel CreateViewModel()
    {
        _browserHistoryService = new Mock<IBrowserHistoryService>();
        _autoCompleteService = new Mock<IAutoCompleteService>();
        return new MainViewModel(_browserHistoryService.Object, _autoCompleteService.Object);
    }
}