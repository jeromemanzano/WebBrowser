using Moq;
using WebBrowser.Services;
using WebBrowser.ViewModels;

namespace WebBrowser.Test.ViewModels;

public class WithMainViewModel : BaseViewModelTest<MainViewModel>
{
    private Mock<ITabService> _tabService;

    [Test]
    public void OnCreate_MainViewModel_Should_Call_TabService_AddNewTab()
    {
        _tabService.Verify(x => x.AddNewTab(null), Times.Once);
    }
    
    [Test]
    public void AddNewTab_Should_Call_TabService_AddNewTab()
    {
        ViewModel.AddNewTab.Execute().Subscribe();
        
        _tabService.Verify(x => x.AddNewTab(null), Times.Exactly(2));
    }
    
    protected override MainViewModel CreateViewModel()
    {
        _tabService = new Mock<ITabService>();
        return new MainViewModel(_tabService.Object);
    }
}