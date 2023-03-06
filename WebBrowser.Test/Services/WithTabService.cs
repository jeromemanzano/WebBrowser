using Moq;
using Splat;
using WebBrowser.Services;
using WebBrowser.Services.Implementation;
using WebBrowser.ViewModels;

namespace WebBrowser.Test.Services;

public class WithTabService
{
    private ITabService _tabService;

    [SetUp]
    public void SetUp()
    {
        _tabService = new TabService();
        var browserHistoryService = new Mock<IBrowserHistoryService>();
        var autoCompleteService = new Mock<IAutoCompleteService>();
        
        Locator.CurrentMutable.Register(() => new TabContentViewModel(browserHistoryService.Object, autoCompleteService.Object, _tabService));
    }

    [Test]
    public void OnCreate_TabService_Should_Not_Have_Tabs_And_Null_SelectedIndex_0()
    {
        Assert.That(_tabService.Tabs, Is.Empty);
        Assert.That(_tabService.SelectedIndex, Is.EqualTo(0));
    }
    
    [Test]
    public void AddNewTab_Should_Add_New_Tab_With_Null_BrowserAddress()
    {
        _tabService.AddNewTab();

        Assert.That(_tabService.Tabs.Count, Is.EqualTo(1));
        Assert.That(_tabService.Tabs[0].BrowserAddress, Is.Null);
    }
    
    [Test]
    public void AddNewTab_With_Initial_Address_Should_Add_New_Tab_And_Update_BrowserAddress()
    {
        var testInitialAddress = "https://www.duckduckgo.com";
        _tabService.AddNewTab(); // add empty tab
        _tabService.AddNewTab(testInitialAddress);

        Assert.That(_tabService.Tabs.Count, Is.EqualTo(2));
        Assert.That(_tabService.Tabs[0].BrowserAddress, Is.Null);
        Assert.That(_tabService.Tabs[1].BrowserAddress, Is.EqualTo(testInitialAddress));
    }
    
    [Test]
    public void RemoveTab_Should_Remove_Tab()
    {
        var tabCount = 3;

        for (int i = 0; i < tabCount; i++)
        {
            _tabService.AddNewTab();
        }

        
        var tabToRemove = _tabService.Tabs[1];
        _tabService.RemoveTab(tabToRemove);

        Assert.That(_tabService.Tabs.Count, Is.EqualTo(tabCount - 1));
        CollectionAssert.DoesNotContain(_tabService.Tabs, tabToRemove);
    }
    
    [Test]
    public void When_TabCount_Is_One_RemoveTab_Should_Set_SelectedTab_To_Null()
    {
        _tabService.AddNewTab();
        var tabToRemove = _tabService.Tabs[0];
        _tabService.SelectedTab = tabToRemove;
        _tabService.RemoveTab(tabToRemove);

        Assert.That(_tabService.SelectedTab, Is.Null);
    }
    
    [Test]
    public void When_TabCount_Is_Greater_Than_One_And_SelectedIndex_Is_Zero_RemoveTab_Should_Set_SelectedIndex_To_One()
    {
        _tabService.AddNewTab();
        _tabService.AddNewTab();
        var tabToRemove = _tabService.Tabs[0];
        _tabService.SelectedTab = tabToRemove;
        _tabService.SelectedIndex = 0;
        _tabService.RemoveTab(tabToRemove);

        Assert.That(_tabService.SelectedIndex, Is.EqualTo(1));
    }
    
    [Test]
    public void When_TabCount_Is_Greater_Than_One_And_SelectedIndex_Is_Not_Zero_RemoveTab_Should_Decrement_SelectedIndex()
    {
        var selectedTabIndex = 1;
        _tabService.AddNewTab();
        _tabService.AddNewTab();
        var tabToRemove = _tabService.Tabs[selectedTabIndex];
        _tabService.SelectedTab = tabToRemove;
        _tabService.SelectedIndex = 1;
        _tabService.RemoveTab(tabToRemove);

        Assert.That(_tabService.SelectedIndex, Is.EqualTo(selectedTabIndex - 1));
    }

    [Test]
    public void When_SelectedTab_Changed_SelectedTab_IsActiveTab_Should_Be_True()
    {
        var tabsCount = 5;
        foreach (var _ in Enumerable.Range(0, tabsCount))
        {
            _tabService.AddNewTab();
        }

        var selectedTabIndex = 2;
        var selectedTab = _tabService.Tabs[selectedTabIndex];

        _tabService.SelectedIndex = selectedTabIndex;
        _tabService.SelectedTab = selectedTab;

        for (int currentIndex = 0; currentIndex < _tabService.Tabs.Count; currentIndex++)
        {
            var tab = _tabService.Tabs[currentIndex];
            Assert.That(tab.IsActiveTab, Is.EqualTo(selectedTab == tab));
            Assert.That(tab.IsActiveTab, Is.EqualTo(selectedTabIndex == currentIndex));
        }
    }
}