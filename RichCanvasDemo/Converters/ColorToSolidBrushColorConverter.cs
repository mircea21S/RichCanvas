using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RichCanvasDemo.Converters
{
    public class ColorToSolidBrushColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
