using System;
using System.Globalization;
using System.Windows;

namespace WebBrowser.WPF.Converters;

/// <summary>
/// Converts an object value to a visibility value.
/// For boolean if the value is true, the visibility is visible, otherwise it is collapsed.
/// For integer if the value is 0, the visibility is collapsed, otherwise it is visible.
/// For string if the value is null or empty, the visibility is collapsed, otherwise it is visible.
/// For all other types, if the value is null, the visibility is collapsed, otherwise it is visible.
/// Pass ! to invert the value.
/// </summary>
public class ObjectToVisibilityConverter : BaseValueConverter<ObjectToVisibilityConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var boolValue = value as bool?;
        
        if (!boolValue.HasValue)
        {
            if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
            {
                boolValue = true;
            }
            else if (value is int intValue)
            {
                boolValue = intValue != 0;
            }
        }

        boolValue ??= value != null;


        if (parameter is "!")
        {
            boolValue = !boolValue;
        }
  
        return boolValue.Value ? Visibility.Visible : Visibility.Collapsed;
    }
}