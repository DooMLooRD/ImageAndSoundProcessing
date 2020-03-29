using ImageProcessing.View.Helpers;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ImageProcessing.View.Converters
{
    public class ResultTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var param = (string)parameter;
            var arg = (ResultType)value;
            switch (arg)
            {
                case ResultType.Image:
                    if (param == "Image")
                        return Visibility.Visible;
                    break;
                case ResultType.Histogram:
                    if (param == "Histogram")
                        return Visibility.Visible;
                    break;
                case ResultType.Phase:
                    if (param == "Phase")
                        return Visibility.Visible;
                    break;
                case ResultType.Magnitude:
                    if (param == "Magnitude")
                        return Visibility.Visible;
                    break;
            }
            return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}