using System.Text;
using System.Text.RegularExpressions;

namespace SesnsitiveDataScan.Utilities
{
    public static class MaskingUtils
    {
        public static string MaskEmail(string email)
        {
            int atIndex = email.IndexOf('@');
            return atIndex > 0 ? new string('*', atIndex) + email[atIndex..] : new string('*', email.Length);
        }

        public static string MaskSSN(string ssn)
        {
            return Regex.IsMatch(ssn, @"^\d{3}-\d{2}-\d{4}$")
                ? "***-**-" + ssn[^4..]
                : new string('*', ssn.Length);
        }

        public static string MaskCreditCard(string card)
        {
            var digits = card.Where(char.IsDigit).ToArray();
            if (digits.Length < 4) return new string('*', card.Length);

            string maskedDigits = new string('*', digits.Length - 4) + new string(digits[^4..]);

            int digitIndex = 0;
            var result = new StringBuilder();
            foreach (char c in card)
            {
                if (char.IsDigit(c))
                    result.Append(maskedDigits[digitIndex++]);
                else
                    result.Append(c);
            }
            return result.ToString();
        }
    }
}
