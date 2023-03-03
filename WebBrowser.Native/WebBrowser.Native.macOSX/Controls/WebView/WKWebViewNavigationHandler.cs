using WebKit;

namespace WebBrowser.macOSX.Controls.WebView;

internal sealed class WebViewNavigationHandler : WKNavigationDelegate
{
    private Action<string>? _titleChanged;
    private Action<string>? _addressChanged;
    
    public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
    {
        if (!string.IsNullOrEmpty(webView.Title))
        {
            _titleChanged?.Invoke(webView.Title);
        }

        if (!string.IsNullOrEmpty(webView.Url?.AbsoluteString))
        {
            _addressChanged?.Invoke(webView.Url?.AbsoluteString!);
        }
    }
    
    /// <summary>
    /// Sets the action that will be invoked when Title changed
    /// </summary>
    /// <param name="titleChanged">The action that will be invoked</param>
    internal WebViewNavigationHandler OnTitleChanged(Action<string> titleChanged)
    {
        _titleChanged = titleChanged;
        return this;
    }
    
    /// <summary>
    /// Sets the action that will be invoked when Address changed
    /// </summary>
    /// <param name="addressChanged">The action that will be invoked</param>
    internal WebViewNavigationHandler OnAddressChanged(Action<string> addressChanged)
    {
        _addressChanged = addressChanged;
        return this;
    }
}