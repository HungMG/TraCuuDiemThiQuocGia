using System.Globalization;
using Microsoft.Maui.Controls;

namespace TraCuuDiemThiQuocGia.Converters;

public class NotNullConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var invert = parameter is string str && str.Equals("Invert", StringComparison.OrdinalIgnoreCase);
        var isNotNull = value != null;
        return invert ? !isNotNull : isNotNull;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
