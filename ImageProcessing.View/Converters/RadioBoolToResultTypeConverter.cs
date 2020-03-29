using ImageProcessing.View.Helpers;
using System;
using System.Globalization;
using System.Windows.Data;

namespace ImageProcessing.View.Converters
{
    public class RadioBoolToResultTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ResultType integer = (ResultType)value;
            if (integer == (ResultType)Enum.Parse(typeof(ResultType), parameter.ToString()))
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}
