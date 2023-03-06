using System.Collections.ObjectModel;
using CrossBrowser.ViewModels;

namespace CrossBrowser.Services;

public interface ITabService
{
    /// <summary>
    /// Tab currently selected
    /// </summary>
    TabContentViewModel? SelectedTab { get; set; }

    /// <summary>
    /// The index of selected tab
    /// </summary>
    int SelectedIndex { get; set; } 
    
    /// <summary>
    /// Collection of tabs
    /// </summary>
    ObservableCollection<TabContentViewModel> Tabs { get; }
    
    /// <summary>
    /// Add a new tab to the collection
    /// </summary>
    /// <param name="initialAddress">The initial address that will be loaded on the new tab</param>
    void AddNewTab(string initialAddress = null);
    
    /// <summary>
    /// Removes a tab from the collection.
    /// </summary>
    void RemoveTab(TabContentViewModel tab);
}