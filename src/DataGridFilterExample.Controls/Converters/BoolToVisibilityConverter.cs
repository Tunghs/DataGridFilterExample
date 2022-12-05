using System;
using System.Windows;
using System.Windows.Data;

namespace DataGridFilterExample.Controls.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool curValue = (bool)value;
            bool eqValue;

            if (bool.TryParse((string)parameter, out eqValue))
                curValue = curValue == eqValue;

            return curValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}