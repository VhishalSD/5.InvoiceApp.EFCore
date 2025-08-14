using System;
using System.Globalization;
using System.Net.Mail;

namespace InvoiceApp.EFCore.Validators
{
    public static class Validator
    {
        // Validate name: only letters, spaces or hyphens, at least 3 characters
        public static bool IsValidName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3) return false;

            foreach (char c in name)
            {
                if (!char.IsLetter(c) && c != ' ' && c != '-') return false;
            }

            return true;
        }

        // Validate email using built-in MailAddress
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Validate date in dd-MM-yyyy format and realistic range
        public static bool TryParseInvoiceDate(string input, out DateTime date)
        {
            date = DateTime.MinValue;

            if (!DateTime.TryParseExact(input, "dd-MM-yyyy", new CultureInfo("nl-NL"), DateTimeStyles.None, out date))
                return false;

            if (date > DateTime.Today || date < DateTime.Today.AddYears(-100))
                return false;

            return true;
        }

        // Validate decimal number > 0
        public static bool IsValidDecimal(string input, out decimal value)
        {
            value = 0m;
            if (!decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                return false;

            return value > 0;
        }

        // Validate tax rate 0 - 1 (supports %, commas, dots)
        public static bool IsValidTaxRate(string input, out decimal taxRate)
        {
            taxRate = 0m;
            if (string.IsNullOrWhiteSpace(input)) return false;

            string cleaned = input.Trim().Replace("%", "").Replace(" ", "").Replace(",", ".");

            if (!decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out taxRate))
                return false;

            if (taxRate > 1m) taxRate /= 100m;

            return taxRate >= 0m && taxRate <= 1m;
        }
    }
}
