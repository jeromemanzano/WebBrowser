using WebBrowser.ViewModels;

namespace WebBrowser.Test.ViewModels;

public abstract class BaseViewModelTest<TViewModel> where TViewModel : BaseViewModel
{
    protected TViewModel ViewModel { get; set; }

    [SetUp]
    public void Setup()
    {
        ViewModel = CreateViewModel();
    }

    protected abstract TViewModel CreateViewModel();
}