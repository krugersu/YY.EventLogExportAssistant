namespace YY.EventLogExportAssistant.Helpers
{
    public static class StringExtensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
        public static string FixNetworkPath(this string sourceValue)
        {
            if (sourceValue.Length > 1 && sourceValue[0] == '\\' && sourceValue[1] != '\\')
                return "\\" + sourceValue;
            else
                return sourceValue;
        }
    }
}
