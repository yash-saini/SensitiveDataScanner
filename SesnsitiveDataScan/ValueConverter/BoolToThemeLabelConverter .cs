using System.Globalization;

namespace SesnsitiveDataScan.ValueConverter
{
    public class BoolToThemeLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isDark = (bool)value;
            return isDark ? "Dark Mode" : "Light Mode";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
