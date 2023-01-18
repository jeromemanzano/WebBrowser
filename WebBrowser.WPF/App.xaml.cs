using System.Windows;
using Splat;
using WebBrowser.Entities;
using WebBrowser.Services;
using WebBrowser.Services.Implementation;
using WebBrowser.ViewModels;

namespace WebBrowser.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Akavache.Registrations.Start("WebBrowser");

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
        }

        private void RegisterViewModels()
        {
            SplatRegistrations.RegisterLazySingleton<BrowserHistoryViewModel>();
        }
    }
}