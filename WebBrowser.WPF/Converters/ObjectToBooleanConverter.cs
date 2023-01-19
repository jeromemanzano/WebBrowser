using System;
using System.Globalization;

namespace WebBrowser.WPF.Converters;

/// <summary>
/// Converts an object value to boolean value.
/// For integer if the value is 0, the return value is false, otherwise it is true.
/// For string if the value is null or empty, the return value is false, otherwise it is true.
/// For all other types, if the value is null, the return value is false, otherwise it is true.
/// Pass ! to invert the value.
/// </summary>
public class ObjectToBooleanConverter : BaseValueConverter<ObjectToBooleanConverter>
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var booleanValue = ConvertObjectToBoolean(value);

        if (parameter is "!")
        {
            booleanValue = !booleanValue;
        }

        return booleanValue;
    }

    public static bool ConvertObjectToBoolean(object value)
    {
        bool? boolValue = null;

        if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
        {
            boolValue = true;
        }
        else if (value is int intValue)
        {
            boolValue = intValue != 0;
        }
            
        return boolValue ?? value != null;
    }
}