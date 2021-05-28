using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RichCanvas.Converters
{
    public class BoolToGeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var enableGrid = (bool)value;
            if (enableGrid)
            {
                return parameter;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
