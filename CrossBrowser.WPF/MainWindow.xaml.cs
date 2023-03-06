using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;
using CrossBrowser.Services;
using CrossBrowser.ViewModels;

namespace CrossBrowser.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = Locator.Current.GetService<MainViewModel>();
            DataContext = viewModel;

            var tabService = Locator.Current.GetService<ITabService>();
            this.WhenActivated(disposable =>
            {
                tabService.WhenAnyValue(x => x.Tabs.Count)
                    .Where(tabsCount => tabsCount < 1)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(_ => Close())
                    .Subscribe()
                    .DisposeWith(disposable);
            });
            
        }
        
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Akavache.BlobCache.Shutdown().Wait();
        }
    }
}