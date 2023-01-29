using CefSharp;
using CefSharp.Wpf.Handler;

namespace WebBrowser.WPF.Controls;

public class ContextMenuHandler : CefSharp.Wpf.Handler.ContextMenuHandler
{
    private const int Open = 28501;
    private string _linkUrl;

    protected override void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters,
        IMenuModel model)
    {
        var reloadIndex = 2;
        if (parameters.TypeFlags.HasFlag(ContextMenuType.Link))
        {
            model.InsertItemAt(0, (CefMenuCommand)Open, "Open");
            model.InsertSeparatorAt(1);
            reloadIndex += 2;
        }

        model.InsertItemAt(reloadIndex, CefMenuCommand.Reload, "Reload");
        _linkUrl = parameters.LinkUrl;
    }

    protected override void ExecuteCommand(IBrowser browser, ContextMenuExecuteModel model)
    {
        if (model.MenuCommand == (CefMenuCommand)Open)
        {
            browser.FocusedFrame.LoadUrl(_linkUrl);
        }
        else
        {        
            base.ExecuteCommand(browser, model);
        }
    }
}