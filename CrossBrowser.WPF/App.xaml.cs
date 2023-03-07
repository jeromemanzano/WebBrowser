using System.Windows;
using Splat;
using CrossBrowser.Entities;
using CrossBrowser.Services;
using CrossBrowser.Services.Implementation;
using CrossBrowser.ViewModels;

namespace CrossBrowser.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Akavache.Registrations.Start("CrossBrowser");

            SplatRegistrations.SetupIOC(Locator.GetLocator());
            RegisterServices();
            RegisterViewModels();
        }
        
        private void RegisterServices()
        {
            SplatRegistrations.RegisterLazySingleton<IBrowserHistoryService, BrowserHistoryService>();
            SplatRegistrations.RegisterLazySingleton<IDuckDuckGoApiService, DuckDuckGoApiService>();
            SplatRegistrations.RegisterLazySingleton<IAutoCompleteService, AutoCompleteService>();
            SplatRegistrations.RegisterLazySingleton<IRepositoryService<HistoryEntity>, RepositoryService<HistoryEntity>>();
            SplatRegistrations.RegisterLazySingleton<ITabService, TabService>();
        }

        private void RegisterViewModels()
        {
            SplatRegistrations.RegisterLazySingleton<MainViewModel>();
            SplatRegistrations.RegisterLazySingleton<BrowserHistoryViewModel>();
            SplatRegistrations.Register<TabContentViewModel>();
        }
    }
}