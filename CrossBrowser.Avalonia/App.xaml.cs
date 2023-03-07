using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CrossBrowser.Avalonia.Views;
using Splat;
using CrossBrowser.ViewModels;

namespace CrossBrowser.Avalonia
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