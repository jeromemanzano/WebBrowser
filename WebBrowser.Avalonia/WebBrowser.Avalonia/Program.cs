using System;
using AppKit;
using Avalonia;
using Avalonia.ReactiveUI;
using Splat;
using WebBrowser.Entities;
using WebBrowser.Services;
using WebBrowser.Services.Implementation;
using WebBrowser.UI.Controls;
using WebBrowser.ViewModels;

namespace WebBrowser.Avalonia;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Setup();
        RegisterServices();
        RegisterControls();
        RegisterViewModels();
        
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseReactiveUI()
            .LogToTrace();

    private static void Setup()
    {
        NSApplication.Init();
        Akavache.Registrations.Start("WebBrowser");
        SplatRegistrations.SetupIOC(Locator.GetLocator());
    }
    
    private static void RegisterServices()
    {            
        SplatRegistrations.RegisterLazySingleton<IBrowserHistoryService, BrowserHistoryService>();
        SplatRegistrations.RegisterLazySingleton<IDuckDuckGoApiService, DuckDuckGoApiService>();
        SplatRegistrations.RegisterLazySingleton<IAutoCompleteService, AutoCompleteService>();
        SplatRegistrations.RegisterLazySingleton<IRepositoryService<HistoryEntity>, RepositoryService<HistoryEntity>>();
        SplatRegistrations.RegisterLazySingleton<ITabService, TabService>();
    }

    private static void RegisterViewModels()
    {
        SplatRegistrations.RegisterLazySingleton<MainViewModel>();
        SplatRegistrations.RegisterLazySingleton<BrowserHistoryViewModel>();
        SplatRegistrations.Register<TabContentViewModel>();
    }

    private static void RegisterControls()
    {
        SplatRegistrations.Register<INativeWebBrowser, macOSX.Controls.WebView.WebView>();
    }
}