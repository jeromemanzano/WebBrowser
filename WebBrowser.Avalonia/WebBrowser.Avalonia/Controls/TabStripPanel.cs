using System.Linq;
using Avalonia;
using Avalonia.Controls;

namespace WebBrowser.Avalonia.Controls;

public class TabStripPanel : WrapPanel
{
    private const double RightPaddingOffset = 50;
    private const double TabItemDefaultWidth = 150.0;
    public double MaxTabStripWidth { get; set; }

    public static readonly DirectProperty<TabStripPanel, double> MaxTabStripWidthProperty =
        AvaloniaProperty.RegisterDirect<TabStripPanel, double>(
            name: nameof(MaxTabStripWidth),
            getter: panel => panel.MaxTabStripWidth,
            setter: (panel, maxTabStripWidth) =>
            {
                panel.MaxTabStripWidth = maxTabStripWidth;
                panel.InvalidateMeasure();
            });

    protected override Size MeasureOverride(Size constraint)
    {
        if (double.IsInfinity(constraint.Width) && MaxTabStripWidth > double.Epsilon)
        {
            constraint = new Size(MaxTabStripWidth - RightPaddingOffset, constraint.Height);
        }
        
        for (int i = 0, count = Children.Count; i < count; ++i)
        {
            var child = Children[i];
            
            if (child is TabItem tabItem)
            {
                ResizeChildren(tabItem);
            }
        }
    
        return base.MeasureOverride(constraint);
    }
    
    private void ResizeChildren(TabItem tabItem)
    {
        var tabCount = Children.Count(child => child is TabItem);
        var maxLength = MaxTabStripWidth - RightPaddingOffset;
        if (TabItemDefaultWidth * tabCount < maxLength)
        {
            tabItem.Width = TabItemDefaultWidth;
            return;
        }
        
        tabItem.Width = maxLength/ tabCount;
    }
}