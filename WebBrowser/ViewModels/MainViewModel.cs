using System.Reactive;
using ReactiveUI;
using WebBrowser.Services;

namespace WebBrowser.ViewModels;

public class MainViewModel : BaseViewModel
{
    public ITabService TabService { get; }
    public ReactiveCommand<Unit, Unit> AddNewTab { get; }
    
    public MainViewModel(ITabService tabService)
    {
        TabService = tabService;
        AddNewTab = ReactiveCommand.Create(() => TabService.AddNewTab());
        TabService.AddNewTab();
    }
}
