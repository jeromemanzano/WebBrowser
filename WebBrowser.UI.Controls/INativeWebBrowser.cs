using System.Windows.Input;

namespace WebBrowser.UI.Controls;

public interface INativeWebBrowser : IDisposable
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