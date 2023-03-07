using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using CrossBrowser.Extensions;
using CrossBrowser.Native.Common;

namespace CrossBrowser.Native.Windows.Controls.WebView;

public sealed class WebViewAdapter : ReactiveObject, INativeWebBrowserAdapter
{
    private bool CanGoForward => !IsLoading && _webView.CanGoForward;
    private bool CanGoBack => !IsLoading && _webView.CanGoBack;
    private readonly WebView2 _webView = new();

    public event Action<string>? TitleChanged;
    public event Action<string>? AddressChanged;
    public IntPtr? NativeHandle => _webView.Handle;
    public string NativeDescriptor => "Hndl";
    public string? Title => _webView.CoreWebView2?.DocumentTitle;

    [Reactive] public string? Address { get; set; }

    [Reactive] public bool IsLoading { get; set; }

    [Reactive] public bool LoadingFailed { get; private set; }

    public ICommand ReloadCommand { get; }
    public ICommand ForwardCommand { get; }
    public ICommand BackCommand { get; }

    public WebViewAdapter()
    {
        if (!IsWebView2Available())
        {
            throw new InvalidOperationException("Webview2 is not available");
        }

        _webView.NavigationStarting += WebViewOnNavigationStarting;
        _webView.NavigationCompleted += WebViewOnNavigationCompleted;

        ForwardCommand = ReactiveCommand.Create(_webView.GoForward,
            this.WhenAnyValue(x => x.CanGoForward));
        BackCommand = ReactiveCommand.Create(_webView.GoBack,
            this.WhenAnyValue(x => x.CanGoBack));
        ReloadCommand = ReactiveCommand.Create(_webView.Reload);

        this.WhenAnyValue(webView => webView.Address)
            .Where(address => address != _webView.Source?.AbsoluteUri)
            .WhereNotNull()
            .DistinctUntilChanged()
            .Select(StringExtensions.FormatStringUrl)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Select(LoadUrlAsync)
            .Subscribe();
    }

    public void Dispose()
    {
        _webView.NavigationStarting -= WebViewOnNavigationStarting;
        _webView.NavigationCompleted -= WebViewOnNavigationCompleted;
    }

    private void WebViewOnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        IsLoading = false;

        this.RaisePropertyChanged(nameof(CanGoBack));
        this.RaisePropertyChanged(nameof(CanGoForward));

        if (!e.IsSuccess)
        {
            LoadingFailed = true;
            return;
        }

        if (_webView.CoreWebView2?.Source is { } address)
        {
            Address = address;
            AddressChanged?.Invoke(Address);
        }

        if (Title is not null)
        {
            TitleChanged?.Invoke(Title);
        }
    }

    private void WebViewOnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        IsLoading = true;
        LoadingFailed = false;
    }

    private async Task LoadUrlAsync(string url)
    {
        await _webView.EnsureCoreWebView2Async();
        _webView.CoreWebView2.Navigate(url);
    }

    private bool IsWebView2Available()
    {
        try
        {
            var versionString = CoreWebView2Environment.GetAvailableBrowserVersionString();
            return !string.IsNullOrWhiteSpace(versionString);
        }
        catch
        {
            return false;
        }
    }
}