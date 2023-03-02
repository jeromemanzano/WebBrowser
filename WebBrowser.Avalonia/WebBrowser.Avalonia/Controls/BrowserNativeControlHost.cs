using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Platform;
using Splat;
using WebBrowser.UI.Controls;

namespace WebBrowser.Avalonia.Controls;

public class BrowserNativeControlHost : NativeControlHost
{
    private readonly INativeWebBrowser _nativeWebBrowser;
    private readonly IPlatformHandle _platformHandle;

    public static readonly DirectProperty<BrowserNativeControlHost, ICommand> ReloadCommandProperty =
        AvaloniaProperty.RegisterDirect<BrowserNativeControlHost, ICommand>(
            name: nameof(INativeWebBrowser.ReloadCommand),
            getter: browser => browser._nativeWebBrowser.ReloadCommand);

    public static readonly DirectProperty<BrowserNativeControlHost, ICommand> ForwardCommandProperty =
        AvaloniaProperty.RegisterDirect<BrowserNativeControlHost, ICommand>(
            name: nameof(INativeWebBrowser.ForwardCommand),
            getter: browser => browser._nativeWebBrowser.ForwardCommand);

    public static readonly DirectProperty<BrowserNativeControlHost, ICommand> BackCommandProperty =
        AvaloniaProperty.RegisterDirect<BrowserNativeControlHost, ICommand>(
            name: nameof(INativeWebBrowser.BackCommand),
            getter: browser => browser._nativeWebBrowser.BackCommand);

    public static readonly DirectProperty<BrowserNativeControlHost, string?> AddressProperty =
        AvaloniaProperty.RegisterDirect<BrowserNativeControlHost, string?>(
            name: nameof(Address),
            getter: browser => browser.Address,
            setter: (browser, address) => browser.Address = address,
            unsetValue: null,
            BindingMode.TwoWay);

    public string? Address
    {
        get => _nativeWebBrowser.Address;
        set
        {
            SetAndRaise(AddressProperty, ref value, value);
            _nativeWebBrowser.Address = value;
        }
    }

    public static readonly DirectProperty<BrowserNativeControlHost, string?> TitleProperty =
        AvaloniaProperty.RegisterDirect<BrowserNativeControlHost, string?>(
            name: nameof(Title),
            browser => browser.Title,
            setter: null,
            unsetValue: null);

    public string? Title => _nativeWebBrowser.Title;

    public BrowserNativeControlHost()
    {
        _nativeWebBrowser = Locator.Current.GetService<INativeWebBrowser>()!;
        _platformHandle = new PlatformHandle(_nativeWebBrowser.NativeHandle ?? IntPtr.Zero,
            _nativeWebBrowser.NativeDescriptor);
    }

    private void NativeWebBrowserOnTitleChanged(string title)
    {
        RaisePropertyChanged(TitleProperty, Title, title);
    }

    private void NativeWebBrowserOnAddressChanged(string address)
    {
        RaisePropertyChanged(AddressProperty, Address, address);
    }

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        _nativeWebBrowser.AddressChanged += NativeWebBrowserOnAddressChanged;
        _nativeWebBrowser.TitleChanged += NativeWebBrowserOnTitleChanged;
        return _platformHandle;
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        _nativeWebBrowser.AddressChanged -= NativeWebBrowserOnAddressChanged;
        _nativeWebBrowser.TitleChanged -= NativeWebBrowserOnTitleChanged;
        _nativeWebBrowser.Dispose();
    }
}