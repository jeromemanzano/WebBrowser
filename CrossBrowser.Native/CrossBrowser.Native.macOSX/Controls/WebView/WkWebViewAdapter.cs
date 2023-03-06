using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using CrossBrowser.Extensions;
using CrossBrowser.Native.Common;
using WebKit;

namespace CrossBrowser.Native.macOSX.Controls.WebView;

public sealed class WkWebViewAdapter : ReactiveObject, INativeWebBrowserAdapter
{
    private readonly WKWebView _webView;
    public event Action<string>? TitleChanged;
    public event Action<string>? AddressChanged;
    public IntPtr? NativeHandle => _webView.Handle.Handle;
    public string NativeDescriptor => _webView.Description;
    
    // We are returning null to make it consistent with CEF
    public string? Title => string.IsNullOrEmpty(_webView.Title) ? null : _webView.Title; 
    
    private string? _address;
    public string? Address 
    {
        get => _address;
        set => this.RaiseAndSetIfChanged(ref _address, value);
    }

    public bool IsLoading => throw new NotImplementedException("Property is not yet implemented");
    public bool LoadingFailed { get; }
    public ICommand ReloadCommand { get; }
    public ICommand ForwardCommand { get; }
    public ICommand BackCommand { get; }

    public WkWebViewAdapter()
    {
        _webView = new WKWebView(CGRect.Empty, new WKWebViewConfiguration())
        {
            NavigationDelegate = new WebViewNavigationHandler()
                .OnTitleChanged(newTitle => TitleChanged?.Invoke(newTitle))
                .OnAddressChanged(newAddress =>
                {
                    Address = newAddress;
                    AddressChanged?.Invoke(newAddress);
                }),
        };

        ForwardCommand = ReactiveCommand.Create(_webView.GoForward, 
            this.WhenAnyValue(x => x._webView.CanGoForward));
        BackCommand = ReactiveCommand.Create(_webView.GoBack, 
            this.WhenAnyValue(webView => webView._webView.CanGoBack));
        ReloadCommand = ReactiveCommand.Create(_webView.Reload);
        
        this.WhenAnyValue(webView => webView.Address)
            .WhereNotNull()
            .Where(address => address != _webView.Url?.AbsoluteString)
            .DistinctUntilChanged()
            .Select(StringExtensions.FormatStringUrl)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(formattedUrl => _webView.LoadRequest(new NSUrlRequest(new NSUrl(formattedUrl))))
            .Subscribe();
    }

    public void Dispose()
    {
        _webView.Dispose();
    }
}