using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;

namespace WebBrowser.Avalonia.Converters;

public class CoalesceConverter : BaseMultiValueConverter<CoalesceConverter>
{
    public override object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return values.FirstOrDefault(value =>
        {
            if (value is string stringValue)
                return !string.IsNullOrEmpty(stringValue);
            return value is not UnsetValueType && value is not null;
        });
    }
}