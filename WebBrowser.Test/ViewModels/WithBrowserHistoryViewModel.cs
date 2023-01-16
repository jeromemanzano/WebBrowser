using System.Collections.Immutable;
using DynamicData;
using Moq;
using WebBrowser.Entities;
using WebBrowser.Models;
using WebBrowser.Services;
using WebBrowser.ViewModels;

namespace WebBrowser.Test.ViewModels;

public class WithBrowserHistoryViewModel : BaseViewModelTest<BrowserHistoryViewModel>
{
    private Mock<IBrowserHistoryService> _browserHistoryService;
    private readonly IEnumerable<HistoryEntity> _storedEntities;

    public WithBrowserHistoryViewModel()
    {
        _storedEntities = Enumerable
            .Range(0, 10).Select(index => new HistoryEntity() { Url = $"https://testing.com/{index}"})
            .ToImmutableList();
    }
    
    protected override BrowserHistoryViewModel CreateViewModel()
    {
        _browserHistoryService = new Mock<IBrowserHistoryService>();

        var sourceCache = new SourceCache<HistoryEntity, string>(entity => entity.Id);
        sourceCache.AddOrUpdate(_storedEntities);
        _browserHistoryService.SetupGet(service => service.BrowserHistory).Returns(sourceCache);
        return new BrowserHistoryViewModel(_browserHistoryService.Object);
    }

    [Test]
    public void OnCreate_Should_Map_BrowserHistoryService_BrowserHistory_To_History()
    {
        CollectionAssert.IsNotEmpty(ViewModel.History);
        CollectionAssert.AreEqual(_storedEntities.Select(entity => entity.Url), ViewModel.History.Select(x => x.Url));
    }

    [Test]
    public void DeleteHistoryItem_Should_Call_BrowserHistoryService_DeleteItem_Once()
    {
        var historyItem = ViewModel.History.First();
        
        ViewModel
            .DeleteHistoryItem
            .Execute(historyItem)
            .Subscribe();
        
        _browserHistoryService.Verify(service => service.DeleteItemAsync(It.Is<HistoryModel>(item => item == historyItem)), Times.Once);
    }
}