using System.Reactive.Linq;
using System.Web;
using DynamicData;
using Microsoft.Reactive.Testing;
using ReactiveUI;
using CrossBrowser.Extensions;
using CrossBrowser.Services;
using CrossBrowser.ViewModels;
using Moq;
using ReactiveUI.Testing;

namespace CrossBrowser.Test.ViewModels;

public class WithTabContentViewModel : BaseViewModelTest<TabContentViewModel>
{
    private Mock<IBrowserHistoryService> _browserHistoryService;
    private Mock<IAutoCompleteService> _autoCompleteService;
    private Mock<ITabService> _tabService;

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
    public void When_BrowserAddress_Changed_And_Not_A_Valid_Url_And_Not_IsActiveTab_It_Should_Call_IBrowserHistoryService_AddWebsiteToHistory_With_BrowserAddress_Once()
    {
        ViewModel.IsActiveTab = false;
        ViewModel.BrowserAddress = "this is not a valid url";
        
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
            ViewModel = new TabContentViewModel(_browserHistoryService.Object, _autoCompleteService.Object, _tabService.Object, scheduler);
            ViewModel.IsActiveTab = true;
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
            ViewModel = new TabContentViewModel(_browserHistoryService.Object, _autoCompleteService.Object, _tabService.Object, scheduler);
            ViewModel.IsActiveTab = true;
            var suggestions = new List<string> {"suggestion 1", "suggestion 2"};
            var suggestions2 = new List<string> {"suggestion 3", "suggestion 4"};

            var observable = scheduler.CreateColdObservable(
                ReactiveTest.OnNext<IReadOnlyCollection<string>>(1, Enumerable.Empty<string>().ToList().AsReadOnly()),
                ReactiveTest.OnNext<IReadOnlyCollection<string>>(TimeSpan.FromMilliseconds(500).Ticks, suggestions.AsReadOnly()),
                ReactiveTest.OnNext<IReadOnlyCollection<string>>(TimeSpan.FromMilliseconds(1000).Ticks, suggestions2.AsReadOnly()));
            
            _autoCompleteService
                .Setup(service => service.GetSuggestions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(observable);

            ViewModel.AddressBarText = "test";
            
            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(300).Ticks);
            CollectionAssert.IsEmpty(ViewModel.Suggestions);
            
            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(501).Ticks);
            CollectionAssert.AreEqual(suggestions, ViewModel.Suggestions);
            
            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(500).Ticks);
            CollectionAssert.AreEqual(suggestions.Concat(suggestions2), ViewModel.Suggestions);
        });
    }

    [Test]
    public void When_Go_Is_Executed_GetSuggestions_Result_Should_Not_Be_Assigned_To_Suggestions()
    {
        new TestScheduler().With(scheduler =>
        {
            ViewModel = new TabContentViewModel(_browserHistoryService.Object, _autoCompleteService.Object, _tabService.Object, scheduler);
            var suggestions = new List<string> {"suggestion 1", "suggestion 2"};

            var observable = scheduler.CreateColdObservable(
                ReactiveTest.OnNext<IReadOnlyCollection<string>>(1, Enumerable.Empty<string>().ToList().AsReadOnly()),
                ReactiveTest.OnNext<IReadOnlyCollection<string>>(TimeSpan.FromMilliseconds(500).Ticks, suggestions.AsReadOnly()));
            
            _autoCompleteService
                .Setup(service => service.GetSuggestions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(observable);

            ViewModel.AddressBarText = "test";
            
            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(300).Ticks);
            CollectionAssert.IsEmpty(ViewModel.Suggestions);

            // We execute go before the next suggestions in 500ms
            ViewModel.Go.Execute().Subscribe();
            
            // Next GetSuggestions result shouldn't be assigned to Suggestions
            scheduler.AdvanceBy(TimeSpan.FromMilliseconds(501).Ticks);
            CollectionAssert.IsEmpty(ViewModel.Suggestions);
        });
    }

    [Test]
    public void When_AddressBarText_Changed_Previous_Cancellation_Token_Should_Be_Cancelled()
    {
        new TestScheduler().With(scheduler =>
        {            
            ViewModel = new TabContentViewModel(_browserHistoryService.Object, _autoCompleteService.Object, _tabService.Object, scheduler);
            ViewModel.IsActiveTab = true;
            var firstSearchTerm = "first search term";
            var secondSearchTerm = "second search term";
            var tokenDictionary = new Dictionary<string, CancellationToken>();
            IObservable<IReadOnlyCollection<string>> CreateObservable(string searchTerm, CancellationToken token)
            {
                tokenDictionary.Add(searchTerm, token);
                return Observable.Return(new List<string>());
            }

            _autoCompleteService
                .Setup(service => service.GetSuggestions(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(CreateObservable);

            // When we change the address bar text, we should get a new cancellation token
            ViewModel.AddressBarText = firstSearchTerm;
            scheduler.AdvanceByMs(300);
            var firstToken = tokenDictionary[firstSearchTerm];
            Assert.IsNotNull(firstToken);
            Assert.IsFalse(firstToken.IsCancellationRequested);

            ViewModel.AddressBarText = secondSearchTerm;
            scheduler.AdvanceByMs(300);
            
            // When we change the address bar text again, the previous token should be cancelled
            Assert.IsTrue(firstToken.IsCancellationRequested);

            // and we get a new cancellation token
            var secondToken = tokenDictionary[secondSearchTerm];
            Assert.IsNotNull(secondToken);
            Assert.IsFalse(secondToken.IsCancellationRequested);
        });
    }
    
    [Test]
    public void RemoveTab_Should_Call_TabService_RemoveTab()
    {
        ViewModel.RemoveTab.Execute().Subscribe();
        
        _tabService.Verify(service => service.RemoveTab(ViewModel), Times.Once);
    }
    
    [Test]
    public void When_IsActiveTab_Is_True_And_BrowserAddress_Is_Null_Should_Update_Title_To_NewTab()
    {
        Assert.That(ViewModel.BrowserAddress, Is.Null);
        Assert.That(ViewModel.Title, Is.EqualTo("New tab"));
    }
    
    [Test]
    public void When_BrowserTitle_Is_Changed_Should_Update_Title()
    {
        var updatedTitle = "updated title";
        ViewModel.BrowserTitle = updatedTitle;
        Assert.That(ViewModel.Title, Is.EqualTo(updatedTitle));
    }

    protected override TabContentViewModel CreateViewModel()
    {
        _browserHistoryService = new Mock<IBrowserHistoryService>();
        _autoCompleteService = new Mock<IAutoCompleteService>();
        _tabService = new Mock<ITabService>();
        var tabContentViewModel = new TabContentViewModel(_browserHistoryService.Object, _autoCompleteService.Object, _tabService.Object);
        tabContentViewModel.IsActiveTab = true;
        return tabContentViewModel;
    }
}