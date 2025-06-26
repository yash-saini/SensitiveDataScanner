using System.Globalization;

namespace SesnsitiveDataScan.ValueConverter
{
    public class BoolToColorConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is bool isTrue && parameter is string colors)
                {
                    string[] colorOptions = colors.Split(',');
                    if (colorOptions.Length >= 2)
                    {
                        return isTrue ? colorOptions[1] : colorOptions[0];
                    }
                }
                return "#cccccc"; // Default fallback color
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public class InvertBoolConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is bool boolValue)
                {
                    return !boolValue;
                }
                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is bool boolValue)
                {
                    return !boolValue;
                }
                return value;
            }
        }
}
