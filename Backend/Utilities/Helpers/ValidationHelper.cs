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
    }
}
