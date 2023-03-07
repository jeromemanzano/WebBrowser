using System;
using System.Globalization;
using System.Windows.Data;

namespace CrossBrowser.WPF.Converters;

public abstract class BaseValueConverter<T> : IValueConverter where T : new()
{
    private static T _instance;
    public static T Instance => _instance ??= new T();

    public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}