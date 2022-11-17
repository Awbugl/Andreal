using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Andreal.Window.Common;

internal class EnumDescriptionConverter : IValueConverter
{
    private string GetEnumDescription(Enum enumObj)
    {
        var fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
        var attribArray = fieldInfo?.GetCustomAttributes(false);
        if (attribArray?.Any() != true) return enumObj.ToString();

        DescriptionAttribute? attrib = null;

        foreach (var att in attribArray)
        {
            if (att is DescriptionAttribute attribute)
                attrib = attribute;
        }

        if (attrib != null) return attrib.Description;

        return enumObj.ToString();
    }

    object IValueConverter.Convert(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        var myEnum = (Enum)value;
        var description = GetEnumDescription(myEnum);
        return description;
    }

    object IValueConverter.ConvertBack(
        object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
        => string.Empty;
}
