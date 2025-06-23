using System.Globalization;

namespace SesnsitiveDataScan.ValueConverter
{
    public class MaskingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string original && !string.IsNullOrWhiteSpace(original))
            {
                var colonIndex = original.IndexOf(':');
                if (colonIndex >= 0 && colonIndex + 2 < original.Length)
                {
                    var valuePart = original.Substring(colonIndex + 2);
                    return new string('*', valuePart.Length);
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
