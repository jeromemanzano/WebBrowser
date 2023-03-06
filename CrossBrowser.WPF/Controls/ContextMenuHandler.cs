using CefSharp;
using CefSharp.Wpf.Handler;
using Splat;
using CrossBrowser.Services;

namespace CrossBrowser.WPF.Controls;

public class ContextMenuHandler : CefSharp.Wpf.Handler.ContextMenuHandler
{
    private const int Open = 28501;
    private const int OpenInNewTab = 28502;
    private string _linkUrl;
    private readonly ITabService _tabService;

    public ContextMenuHandler()
    {
        _tabService = Locator.GetLocator().GetService<ITabService>();
    }

    protected override void OnBeforeContextMenu(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IContextMenuParams parameters,
        IMenuModel model)
    {
        var reloadIndex = 2;
        if (parameters.TypeFlags.HasFlag(ContextMenuType.Link))
        {
            model.InsertItemAt(0, (CefMenuCommand)Open, "Open");
            model.InsertItemAt(1, (CefMenuCommand) OpenInNewTab, "Open in new tab");
            model.InsertSeparatorAt(2);
            reloadIndex += 3;
        }

        model.InsertItemAt(reloadIndex, CefMenuCommand.Reload, "Reload");
        _linkUrl = parameters.LinkUrl;
    }

    protected override void ExecuteCommand(IBrowser browser, ContextMenuExecuteModel model)
    {
        switch (model.MenuCommand)
        {
            case (CefMenuCommand)Open:
                browser.FocusedFrame.LoadUrl(_linkUrl);
                break;
            case (CefMenuCommand)OpenInNewTab:
                _tabService.AddNewTab(_linkUrl);
                break;
            default:
                base.ExecuteCommand(browser, model);
                break;
        }
    }
}