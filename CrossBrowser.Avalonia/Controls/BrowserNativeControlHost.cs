using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Platform;
using Splat;
using CrossBrowser.Native.Common;

namespace CrossBrowser.Avalonia.Controls;

public class BrowserNativeControlHost : NativeControlHost
{
    private readonly INativeWebBrowserAdapter _nativeWebBrowserAdapter;
    private readonly IPlatformHandle _platformHandle;

    public static readonly DirectProperty<BrowserNativeControlHost, ICommand> ReloadCommandProperty =
        AvaloniaProperty.RegisterDirect<BrowserNativeControlHost, ICommand>(
            name: nameof(INativeWebBrowserAdapter.ReloadCommand),
            getter: browser => browser._nativeWebBrowserAdapter.ReloadCommand);

    public static readonly DirectProperty<BrowserNativeControlHost, ICommand> ForwardCommandProperty =
        AvaloniaProperty.RegisterDirect<BrowserNativeControlHost, ICommand>(
            name: nameof(INativeWebBrowserAdapter.ForwardCommand),
            getter: browser => browser._nativeWebBrowserAdapter.ForwardCommand);

    public static readonly DirectProperty<BrowserNativeControlHost, ICommand> BackCommandProperty =
        AvaloniaProperty.RegisterDirect<BrowserNativeControlHost, ICommand>(
            name: nameof(INativeWebBrowserAdapter.BackCommand),
            getter: browser => browser._nativeWebBrowserAdapter.BackCommand);

    public static readonly DirectProperty<BrowserNativeControlHost, string?> AddressProperty =
        AvaloniaProperty.RegisterDirect<BrowserNativeControlHost, string?>(
            name: nameof(Address),
            getter: browser => browser.Address,
            setter: (browser, address) => browser.Address = address,
            unsetValue: null,
            BindingMode.TwoWay);

    public string? Address
    {
        get => _nativeWebBrowserAdapter.Address;
        set
        {
            SetAndRaise(AddressProperty, ref value, value);
            _nativeWebBrowserAdapter.Address = value;
        }
    }

    public static readonly DirectProperty<BrowserNativeControlHost, string?> TitleProperty =
        AvaloniaProperty.RegisterDirect<BrowserNativeControlHost, string?>(
            name: nameof(Title),
            browser => browser.Title,
            setter: null,
            unsetValue: null);

    public string? Title => _nativeWebBrowserAdapter.Title;

    public BrowserNativeControlHost()
    {
        _nativeWebBrowserAdapter = Locator.Current.GetService<INativeWebBrowserAdapter>()!;
        
        _platformHandle = new PlatformHandle(_nativeWebBrowserAdapter.NativeHandle ?? IntPtr.Zero,
            _nativeWebBrowserAdapter.NativeDescriptor);
    }

    private void NativeWebBrowserAdapterWrapperWrapperTitleChanged(string title)
    {
        RaisePropertyChanged(TitleProperty, Title, title);
    }

    private void NativeWebBrowserAdapterOnWrapperAddressChanged(string address)
    {
        RaisePropertyChanged(AddressProperty, Address, address);
    }

    protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
    {
        _nativeWebBrowserAdapter.AddressChanged += NativeWebBrowserAdapterOnWrapperAddressChanged;
        _nativeWebBrowserAdapter.TitleChanged += NativeWebBrowserAdapterWrapperWrapperTitleChanged;
        return _platformHandle;
    }

    protected override void DestroyNativeControlCore(IPlatformHandle control)
    {
        _nativeWebBrowserAdapter.AddressChanged -= NativeWebBrowserAdapterOnWrapperAddressChanged;
        _nativeWebBrowserAdapter.TitleChanged -= NativeWebBrowserAdapterWrapperWrapperTitleChanged;
        _nativeWebBrowserAdapter.Dispose();
    }
}