using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace RichCanvas.Converters
{
    /// <summary>
    /// Represents the converter that converts <see cref="uint"/> values to a <see cref="Rect"/> used to apply the spacing of a <see cref="DrawingBrush"/>.
    /// </summary>
    public class UIntToRectConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            uint size = System.Convert.ToUInt32(value);
            return new Rect(0, 0, size, size);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
