using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Splat;
using CrossBrowser.Entities;
using CrossBrowser.Services;
using CrossBrowser.Services.Implementation;
using CrossBrowser.Native.Common;
using CrossBrowser.ViewModels;

namespace CrossBrowser.Avalonia;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        Setup();
        SetupNative();
        RegisterServices();
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
        Akavache.Registrations.Start("CrossBrowser");
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

    private static void SetupNative()
    {
#if MACOS
        AppKit.NSApplication.Init(); // This needs to be called once when loading native controls
        SplatRegistrations.Register<INativeWebBrowserAdapter, CrossBrowser.Native.macOSX.Controls.WebView.WkWebViewAdapter>();
        
        
#else
        SplatRegistrations.Register<INativeWebBrowserAdapter, CrossBrowser.Native.Windows.Controls.WebView.WebViewAdapter>();
#endif
    }
}