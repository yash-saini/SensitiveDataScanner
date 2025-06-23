using SesnsitiveDataScan.Utilities;
using System.Globalization;

namespace SesnsitiveDataScan.ValueConverter
{
    public class MaskingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string input && !string.IsNullOrWhiteSpace(input))
            {
                var colonIndex = input.IndexOf(':');
                if (colonIndex >= 0 && colonIndex + 2 < input.Length)
                {
                    var type = input.Substring(0, colonIndex).Trim();
                    var original = input.Substring(colonIndex + 2).Trim();

                    return type switch
                    {
                        "Email" => MaskingUtils.MaskEmail(original),
                        "SSN" => MaskingUtils.MaskSSN(original),
                        "Credit Card" => MaskingUtils.MaskCreditCard(original),
                        _ => new string('*', original.Length)
                    };
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
