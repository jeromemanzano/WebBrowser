using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace WebBrowser.Avalonia.Converters;

public abstract class BaseMultiValueConverter<T> : IMultiValueConverter where T : new()
{
    private static T? _instance;
    
    public static T Instance => _instance ??= new T();

    public abstract object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture);
}