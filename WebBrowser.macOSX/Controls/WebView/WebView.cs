using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using WebBrowser.UI.Controls;
using WebKit;

namespace WebBrowser.macOSX.Controls.WebView;

public sealed class WebView : ReactiveObject, INativeWebBrowser
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
    public ICommand ReloadCommand { get; }
    public ICommand ForwardCommand { get; }
    public ICommand BackCommand { get; }

    public WebView()
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
            .Select(FormatUrl)
            .ObserveOn(RxApp.MainThreadScheduler)
            .Do(formattedUrl => _webView.LoadRequest(new NSUrlRequest(new NSUrl(formattedUrl))))
            .Subscribe();
    }

    private string FormatUrl(string url)
    {
        if (url.StartsWith(@"http://") || url.StartsWith(@"https://"))
        {
            return url;
        }

        return $"https://{url}";
    }

    public void Dispose()
    {
        _webView.Dispose();
    }
}