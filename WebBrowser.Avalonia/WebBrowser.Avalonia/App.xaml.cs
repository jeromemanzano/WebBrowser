using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Splat;
using WebBrowser.Avalonia.Views;
using WebBrowser.ViewModels;

namespace WebBrowser.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow()
                {
                    DataContext = Locator.Current.GetService<MainViewModel>()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}