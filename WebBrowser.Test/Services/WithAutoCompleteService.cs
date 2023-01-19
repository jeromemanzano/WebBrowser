using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using DynamicData;
using Microsoft.Reactive.Testing;
using Moq;
using ReactiveUI.Testing;
using WebBrowser.Entities;
using WebBrowser.Services;
using WebBrowser.Services.Implementation;

namespace WebBrowser.Test.Services;

public class WithAutoCompleteService
{
    private readonly Mock<IDuckDuckGoApiService> _duckDuckGoApiService = new();
    private readonly Mock<IBrowserHistoryService> _browserHistoryService = new();
    private readonly SourceCache<HistoryEntity, string> _browserHistory = new(x => x.Id);
    private readonly IReadOnlyCollection<string> _apiResults = new List<string>();
    private IAutoCompleteService? _autoCompleteService;

    [SetUp]
    public void SetUp()
    {
        _browserHistory.Clear();
        _browserHistoryService
            .SetupGet(x => x.BrowserHistory)
            .Returns(_browserHistory);
        
        _duckDuckGoApiService
            .Setup(x => x.GetAutoCompleteSuggestionsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_apiResults);
        
        _autoCompleteService = new AutoCompleteService(_duckDuckGoApiService.Object, _browserHistoryService.Object);
    }
    
    [Test]
    public void GetSuggestions_Should_Return_Empty_Result_When_History_Is_Empty()
    {
        IReadOnlyCollection<string>? searchResults = null;
        Exception? getSuggestionsException = null;
        
        _autoCompleteService!.GetSuggestions("search term", CancellationToken.None).Subscribe(result => searchResults = result, onError:
            exception => getSuggestionsException = exception);
        
        Assert.IsNotNull(searchResults);
        Assert.IsNull(getSuggestionsException);
        CollectionAssert.IsEmpty(searchResults);
    }

    [Test]
    public void GetSuggestions_Should_Not_Throw_Exception_And_Return_Empty_Result_When_Api_Throws_Exception()
    {
        IReadOnlyCollection<string>? searchResults = null;
        Exception? getSuggestionsException = null;
        
        _duckDuckGoApiService
            .Setup(x => x.GetAutoCompleteSuggestionsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Throws<Exception>();
        
        _autoCompleteService!.GetSuggestions("search term", CancellationToken.None).Subscribe(result => searchResults = result, onError:
            exception => getSuggestionsException = exception);
        
        Assert.IsNotNull(searchResults);
        Assert.IsNull(getSuggestionsException);
        CollectionAssert.IsEmpty(searchResults);
    }
    
    [Test]
    public void GetSuggestions_Should_Return_History_With_Url_That_Contains_SearchTerm_If_Entity_Query_And_Url_AbsolutePath_Are_Equal()
    {
        var searchTerm = "searchTerm";
        var history = Enumerable
            .Range(0, 5).Select(index => new HistoryEntity() { Url = $"https://{searchTerm}.com/{index}", Query = $"/{index}"})
            .ToArray();
        _browserHistory.AddOrUpdate(history);
    
        IReadOnlyCollection<string>? searchResults = null;
        _autoCompleteService!.GetSuggestions(searchTerm,CancellationToken.None).Subscribe(result => searchResults = result);
        
        Assert.IsTrue(history.All(entity => entity.Query == new Uri(entity.Url).AbsolutePath));
        Assert.IsNotNull(searchResults);
        CollectionAssert.IsNotEmpty(searchResults);
        CollectionAssert.AreEquivalent(history.Select(x => x.Url), searchResults);
    }
    
    [Test]
    public void GetSuggestions_Should_Not_Return_History_With_Url_That_Contains_SearchTerm_If_Entity_Query_And_Url_AbsolutePath_Are_Not_Equal()
    {
        var searchTerm = "searchTerm";
        var history = Enumerable
            .Range(0, 5).Select(index => new HistoryEntity() { Url = $"https://{searchTerm}.com/{index}", Query = $"/{index}-x"})
            .ToArray();
        _browserHistory.AddOrUpdate(history);
    
        IReadOnlyCollection<string>? searchResults = null;
        _autoCompleteService!.GetSuggestions(searchTerm, CancellationToken.None).Subscribe(result => searchResults = result);
        
        Assert.IsFalse(history.Any(entity => entity.Query == new Uri(entity.Url).AbsolutePath));
        Assert.IsNotNull(searchResults);
        CollectionAssert.IsEmpty(searchResults);
    }
    
    [Test]
    public void GetSuggestions_Should_Return_History_With_Query_That_Contains_SearchTerm_If_Entity_Query_And_Url_AbsolutePath_Are_Not_Equal()
    {
        var searchTerm = "searchTerm";
        var history = Enumerable
            .Range(0, 5).Select(index => new HistoryEntity() { Url = $"https://{searchTerm}.com/{index}", Query = $"{searchTerm} {index}"})
            .ToArray();
        _browserHistory.AddOrUpdate(history);
    
        IReadOnlyCollection<string>? searchResults = null;
        _autoCompleteService!.GetSuggestions(searchTerm, CancellationToken.None).Subscribe(result => searchResults = result);
        
        Assert.IsFalse(history.Any(entity => entity.Query == new Uri(entity.Url).AbsolutePath));
        Assert.IsNotNull(searchResults);
        CollectionAssert.IsNotEmpty(searchResults);
        CollectionAssert.AreEquivalent(history.Select(x => x.Query), searchResults);
    }
    
    [Test]
    public void GetSuggestions_Should_Return_Only_The_Latest_Five_Match_From_History_When_There_Is_More_Than_Five_Matches()
    {
        var searchTerm = "searchTerm";
        var latestFive = Enumerable
            .Range(0, 5).Select(index => new HistoryEntity() { Url = $"https://{searchTerm}.com/{index}", Query = $"{searchTerm} {index}", UtcTimeStamp = DateTime.Now.ToUniversalTime()})
            .ToArray();
        var olderFive = Enumerable
            .Range(5, 5).Select(index => new HistoryEntity() { Url = $"https://{searchTerm}.com/{index}", Query = $"{searchTerm} {index}", UtcTimeStamp = DateTime.Now.ToUniversalTime().AddDays(-1)})
            .ToArray();
        _browserHistory.AddOrUpdate(latestFive);
        _browserHistory.AddOrUpdate(olderFive);
        
        IReadOnlyCollection<string>? searchResults = null;
        _autoCompleteService!.GetSuggestions(searchTerm, CancellationToken.None).Subscribe(result => searchResults = result);
        
        CollectionAssert.IsNotEmpty(searchResults);
        CollectionAssert.AreEquivalent(latestFive.Select(x => x.Query), searchResults);
    }

    [Test]
    public void GetSuggestions_Should_Return_History_And_DuckDuckGo_Results()
    {
        new TestScheduler().With(scheduler =>
        {
            var subject = new Subject<IEnumerable<string>>();

            var acSuggestionsTask = subject.ToTask(scheduler);
            var apiResults = new List<string> {"apiResult1", "apiResult2"};

            subject.OnNext(apiResults);
            scheduler.Schedule(TimeSpan.FromMilliseconds(1000),
                () => subject.OnCompleted());

            var searchTerm = "searchTerm";
            var history = Enumerable
                .Range(0, 5).Select(index => new HistoryEntity()
                    {Url = $"https://{searchTerm}.com/{index}", Query = $"{searchTerm} {index}"})
                .ToArray();
            _browserHistory.AddOrUpdate(history);

            _duckDuckGoApiService
                .Setup(x => x.GetAutoCompleteSuggestionsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(acSuggestionsTask);

            IReadOnlyCollection<string>? onNextSearchResult = null;
            _autoCompleteService
                !.GetSuggestions(searchTerm, CancellationToken.None)
                .ObserveOn(scheduler)
                .Subscribe(onNext: result => onNextSearchResult = result);

            scheduler.Start();

            CollectionAssert.AreEquivalent(history.Select(x => x.Query), onNextSearchResult);

            scheduler.AdvanceToMs(1001);
            CollectionAssert.AreEquivalent(apiResults, onNextSearchResult);
        });
    }
}