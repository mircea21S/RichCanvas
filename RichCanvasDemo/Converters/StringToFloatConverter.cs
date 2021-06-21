using System;
using System.Globalization;
using System.Windows.Data;

namespace RichCanvasDemo.Converters
{
    public class StringToFloatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (parameter != null && (string)parameter == "double")
                {
                    return double.Parse((string)value);
                }
                return float.Parse((string)value);
            }
            return 0f;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
}
