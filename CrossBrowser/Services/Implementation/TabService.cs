using System.Collections.ObjectModel;
using System.Reactive.Linq;
using CrossBrowser.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace CrossBrowser.Services.Implementation;

public class TabService : ReactiveObject, ITabService
{
    [Reactive] public TabContentViewModel? SelectedTab { get; set; }

    [Reactive] public int SelectedIndex { get; set; } 

    public ObservableCollection<TabContentViewModel> Tabs { get; } = new ();

    public TabService()
    {
        this.WhenAnyValue(x => x.SelectedTab)
            .WhereNotNull()
            .Do(_ =>
            {
                for (int i = 0; i < Tabs.Count; i++)
                {
                    Tabs[i].IsActiveTab = i == SelectedIndex;
                }
            })
            .Subscribe();
    }

    public void AddNewTab(string? initialAddress = null)
    {
        var tab = Locator.Current.GetService<TabContentViewModel>()!;

        if (initialAddress is not null)
        {
            tab.BrowserAddress = initialAddress;
            Tabs.Insert(SelectedIndex + 1, tab);
            SelectedIndex++;
        }
        else
        {
            Tabs.Add(tab);
            SelectedIndex = Tabs.Count - 1;
        }
    }

    public void RemoveTab(TabContentViewModel tab)
    {
        if (Tabs.Count == 1)
        {
            SelectedTab = null;
        }
        
        if (tab == SelectedTab)
        {
            if (SelectedIndex == 0)
            {
                SelectedIndex = 1;
            }    
            else
            {
                SelectedIndex--;
            }
        }
        
        Tabs.Remove(tab);
    }
}