using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ImageProcessing.View.Converters
{
    [ValueConversion(typeof(bool), typeof(VerticalAlignment))]
    public class BootToVerticalAlignmentConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? VerticalAlignment.Stretch : VerticalAlignment.Top;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {    // Don't need any convert back
            return null;
        }
    }
}