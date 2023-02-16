using System.Windows;

namespace WebBrowser.WPF.Extensions;

public static class TreeViewHelper
{
    public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
    {
        if (parent == null) return null;

        var childrenCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);

            if (child is T childType)
            {
                return childType;
            }

            var foundChild = FindChild<T>(child);

            if (foundChild is not null)
            {
                return foundChild;
            }
        }

        return default;
    }
}