using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace RichCanvasUITests.App.Converters
{
    public class PointToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var point = (Point)value;
            return $"{point.X}, {point.Y}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
