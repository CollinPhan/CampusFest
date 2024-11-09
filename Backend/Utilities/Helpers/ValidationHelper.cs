using Backend.Cores.Entities;
using System.Text.RegularExpressions;

namespace Backend.Utilities.Helpers
{
    public class ValidationHelper
    {
        public static bool ValidateString(string value, string expression)
        {
            Regex regex = new Regex(expression);

            return regex.IsMatch(value);
        }

        public static bool ValidatePassword(string value)
        {
            return ValidateString(
                value,
                @"^(?=.*[A-Za-z])(?=.*\d)[\d\w!@#$%^&*_]{8,30}$"
            );
        }

        public static bool ValidatePhoneNumber(string value)
        {
            return ValidateString(
                value,
                @"^[\d]{9,11}$"
            );
        }

        public static bool ValidateEmail(string value)
        {
            return ValidateString(
                value, 
                @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"
            );
        }

        public static bool ValidateUsername(string value)
        {
            return ValidateString(value, @"^[\w-_]{9,11}$");
        }
    }
}
