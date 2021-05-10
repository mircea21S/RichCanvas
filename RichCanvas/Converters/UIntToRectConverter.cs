using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RichCanvas.Converters
{
    public class UIntToRectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            uint size = System.Convert.ToUInt32(value);
            return new Rect(0, 0, size, size);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
