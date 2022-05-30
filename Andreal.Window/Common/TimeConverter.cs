using System;
using System.Globalization;
using System.Windows.Data;

namespace Andreal.Window.Common;


internal class TimeConverter: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((DateTime)value).ToString("yyyy/MM/dd HH:mm:ss");
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
