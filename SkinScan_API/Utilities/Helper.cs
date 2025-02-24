namespace SkinScan_API.Utilities
{
    public static class Helper
    {
        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;

            var hasUpperCase = password.Any(char.IsUpper);
            var hasLowerCase = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);
            var hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));
            var isLongEnough = password.Length >= 8;

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar && isLongEnough;
        }

    }
}
