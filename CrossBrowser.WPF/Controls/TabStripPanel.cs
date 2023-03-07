using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ReactiveUI;
using CrossBrowser.WPF.Extensions;

namespace CrossBrowser.WPF.Controls;

public class TabStripPanel : UniformGrid, IActivatableView
{
    private const double RightPadding = 60;
    private const double TabItemMaxWidth = 150.0;
    private const double TabPaddingOffset = 10.0;
    public double MaxTabStripWidth
    {
        get => (double)GetValue(MaxTabStripWidthProperty);
        set => SetValue(MaxTabStripWidthProperty, value);
    }

    public static readonly DependencyProperty MaxTabStripWidthProperty =
        DependencyProperty.Register(nameof(MaxTabStripWidth), typeof(double), typeof(TabStripPanel), new PropertyMetadata(0.0, Value_PropertyChanged));

    private static void Value_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((TabStripPanel)d).MeasureOverride(new Size((double)e.NewValue - RightPadding, double.PositiveInfinity));
    }

    protected override Size MeasureOverride(Size constraint)
    {
        if (double.IsInfinity(constraint.Width) && MaxTabStripWidth > double.Epsilon)
        {
            constraint.Width = MaxTabStripWidth - RightPadding;
        }
        
        for (int i = 0, count = InternalChildren.Count; i < count; ++i)
        {
            var child = InternalChildren[i];
            
            var dockPanel = TreeViewHelper.FindChild<DockPanel>(child);
            ResizeChildren(dockPanel, constraint.Width);
        }

        return base.MeasureOverride(constraint);
    }
    
    private void ResizeChildren(Panel panel, double parentWidth)
    {
        if (panel == null)
        {
            return;
        }
        
        if (TabItemMaxWidth * InternalChildren.Count < MaxTabStripWidth - RightPadding)
        {
            panel.Width = TabItemMaxWidth;
            return;
        }
        
        panel.Width = parentWidth / InternalChildren.Count - TabPaddingOffset;
    }
}