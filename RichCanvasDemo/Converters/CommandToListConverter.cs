using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace RichCanvasDemo.Converters
{
    public class CommandToListConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var commands = new List<ICommand>();
            foreach (ICommand command in values)
            {
                commands.Add(command);
            }
            return commands;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
