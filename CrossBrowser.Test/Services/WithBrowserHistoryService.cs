using System.Collections.Immutable;
using Microsoft.Reactive.Testing;
using Moq;
using CrossBrowser.Entities;
using CrossBrowser.Models;
using CrossBrowser.Services;
using CrossBrowser.Services.Implementation;

namespace CrossBrowser.Test.Services;

public class WithBrowserHistoryService
{
    private readonly IEnumerable<HistoryEntity> _storedEntities;
    private Mock<IRepositoryService<HistoryEntity>> _repositoryService;
    private IBrowserHistoryService _browserHistoryService;

    public WithBrowserHistoryService()
    {
        _storedEntities = Enumerable
            .Range(0, 10).Select(index => new HistoryEntity() { Url = $"https://testing.com/{index}"})
            .ToImmutableList();
    }

    [SetUp]
    public void SetUp()
    {
        _repositoryService = new Mock<IRepositoryService<HistoryEntity>>();
        _repositoryService
            .Setup(service => service.GetAllAsync())
            .ReturnsAsync(_storedEntities);
        
        _browserHistoryService = new BrowserHistoryService(_repositoryService.Object);
    }

    [Test]
    public void OnCreate_Should_Call_RepositoryService_GetAll_Once_And_Bind_StoreEntities_To_BrowserHistory()
    {
        _repositoryService.Verify(service => service.GetAllAsync(), Times.Once);
        _storedEntities.AssertEqual(_browserHistoryService.BrowserHistory.Items);
    }
    
    [Test]
    public async Task AddSearchToBrowserHistory_Should_Call_RepositoryService_AddAsync_And_Add_HistoryEntity_To_BrowserHistory_With_SearchTerm_Mapped_To_Query()
    {
        var searchTerm = "search term";
        var url = "duckduckgo.com";
        
        await _browserHistoryService.AddSearchToBrowserHistoryAsync(searchTerm, url);
        
        _repositoryService
            .Verify(service => service.AddAsync(It.Is<HistoryEntity>(entity => entity.Url == url && entity.Query == searchTerm)), Times.Once);


        Assert.IsTrue(_browserHistoryService.BrowserHistory.Items.Any(history => history.Query == searchTerm && history.Url == url));
    }
    
    [Test]
    public async Task AddWebsiteToHistory_Should_Call_RepositoryService_AddAsync_And_Add_HistoryEntity_To_BrowserHistory_With_Uri_Query_Mapped_To_Query()
    {
        var url = "https://duckduckgo.com/?q=query";
        var uri = new Uri(url);

        await _browserHistoryService.AddWebsiteToHistoryAsync(url);
        
        _repositoryService
            .Verify(service => service.AddAsync(It.Is<HistoryEntity>(entity => entity.Url == url && entity.Query == uri.AbsolutePath)), Times.Once);
        Assert.IsTrue(_browserHistoryService.BrowserHistory.Items.Any(history => history.Query == uri.AbsolutePath && history.Url == url));
    }
    
    [Test]
    public async Task When_Url_Is_The_Same_As_Last_BrowserHistory_Entry_Then_AddWebsiteToHistory_Should_Not_Add_New_HistoryEntity()
    {
        var url = "https://duckduckgo.com/?q=query";
        await _browserHistoryService.AddWebsiteToHistoryAsync(url);
        _repositoryService.Invocations.Clear();

        await _browserHistoryService.AddWebsiteToHistoryAsync(url);
        
        // Verify that the repository service was not called again
        _repositoryService
            .Verify(service => service.AddAsync(It.Is<HistoryEntity>(entity => entity.Url == url)), Times.Never);
        
        // Verify that the browser history only contains one entry for the given url
        Assert.That(_browserHistoryService.BrowserHistory.Items.Count(history => history.Url == url), Is.EqualTo(1));
    }
    
    [Test]
    public async Task When_Last_BrowserHistory_Host_Is_DuckDuckGo_And_New_Url_Contains_Last_Url_Then_AddWebsiteToHistory_Should_Update_Last_BrowserHistory_Entry_Url_And_Call_RepositoryService_Update_Once()
    {
        var lastUrl = "https://duckduckgo.com/?q=query";
        var newUrl = "https://duckduckgo.com/?q=query&ia=web";
        await _browserHistoryService.AddWebsiteToHistoryAsync(lastUrl);
        var lastEntity = _browserHistoryService.BrowserHistory.Items.Single(entity => entity.Url == lastUrl);
        
        await _browserHistoryService.AddWebsiteToHistoryAsync(newUrl);
        
        Assert.IsTrue(newUrl.Contains(lastUrl));
        // Verify that the repository service UpdateAsync was called once
        _repositoryService
            .Verify(service => service.UpdateAsync(It.Is<HistoryEntity>(entity => entity.Url == newUrl && entity.Id == lastEntity.Id)), Times.Once);
        
        // Verify that the browser history was updated
        Assert.That(_browserHistoryService.BrowserHistory.Items.Single(entity => entity.Url == newUrl).Id, Is.EqualTo(lastEntity.Id));
    }
    
    
    [Test]
    public async Task When_Last_BrowserHistory_Host_Is_Not_DuckDuckGo_And_New_Url_Contains_Last_Url_Then_AddWebsiteToHistory_Should_Not_Update_Last_BrowserHistory_Entry_Url_And_Never_Call_RepositoryService_Update()
    {
        // Same setup as above except we change the last url to be a non duckduckgo url
        var lastUrl = "https://notduckduckgo.com/?q=query";
        var newUrl = "https://notduckduckgo.com/?q=query&ia=web";
        await _browserHistoryService.AddWebsiteToHistoryAsync(lastUrl);
        var lastEntity = _browserHistoryService.BrowserHistory.Items.Single(entity => entity.Url == lastUrl);
        
        await _browserHistoryService.AddWebsiteToHistoryAsync(newUrl);
        
        Assert.IsTrue(newUrl.Contains(lastUrl));
        // Verify that the repository service UpdateAsync was called once
        _repositoryService
            .Verify(service => service.UpdateAsync(It.Is<HistoryEntity>(entity => entity.Url == newUrl && entity.Id == lastEntity.Id)), Times.Never);
        
        // Verify that the browser history was updated
        Assert.That(_browserHistoryService.BrowserHistory.Items.Single(entity => entity.Url == newUrl).Id, Is.Not.EqualTo(lastEntity.Id));
    }
    
    [Test]
    public async Task Delete_Should_Clear_BrowserHistory_And_Call_RepositoryService_RemoveAll_Once()
    {
        await _browserHistoryService.DeleteAllAsync();
        
        _repositoryService.Verify(service => service.RemoveAllAsync(), Times.Once);
        Assert.That(_browserHistoryService.BrowserHistory.Items, Is.Empty);
    }

    [Test]
    public async Task DeleteItem_Should_Remove_Item_From_BrowserHistory_And_Call_RepositoryService_RemoveById_Once()
    {
        var entityToRemove = _browserHistoryService.BrowserHistory.Items.First();
        var model = new HistoryModel() { Id = entityToRemove.Id };
        
        await _browserHistoryService.DeleteItemAsync(model);
        
        _repositoryService.Verify(service => service.RemoveByIdAsync(entityToRemove.Id), Times.Once);
        Assert.That(_browserHistoryService.BrowserHistory.Items, Does.Not.Contain(entityToRemove));
    }
}