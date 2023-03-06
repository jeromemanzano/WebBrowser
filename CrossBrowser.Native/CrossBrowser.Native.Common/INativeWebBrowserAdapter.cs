using System.Windows.Input;

namespace CrossBrowser.Native.Common;

public interface INativeWebBrowserAdapter : IDisposable
{
    /// <summary>
    /// Event that will be invoked when browser title changed
    /// </summary>
    event Action<string>? TitleChanged;
    
    /// <summary>
    /// Event that will be invoked when browser address changed
    /// </summary>
    event Action<string>? AddressChanged;
    
    /// <summary>
    /// Native handle pointer
    /// </summary>
    IntPtr? NativeHandle { get; }
    
    /// <summary>
    /// Native descriptor
    /// </summary>
    string? NativeDescriptor { get; }
    
    /// <summary>
    /// Title of the web page
    /// </summary>
    string? Title { get; }
    
    /// <summary>
    /// Address of the web page
    /// </summary>
    string? Address { get; set; }
    
    /// <summary>
    /// Flag that indicates if the web page is loading
    /// </summary>
    bool IsLoading { get; }
    
    /// <summary>
    /// Flag that indicates if the web page loading failed.
    /// This is set to false on every initial load attempt.
    /// </summary>
    bool LoadingFailed { get; }    

    /// <summary>
    /// Reloads the web page
    /// </summary>
    ICommand ReloadCommand { get; }
    
    /// <summary>
    /// Go to next web page
    /// </summary>
    ICommand ForwardCommand { get; }
    
    /// <summary>
    /// Go to previous web page
    /// </summary>
    ICommand BackCommand { get; }
}