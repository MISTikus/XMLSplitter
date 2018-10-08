namespace XMLSplitter.Helpers
{
    public static class StringExtensions
    {
        public static string AddSpaceIfNotEmpty(this string value) => string.IsNullOrWhiteSpace(value)
            ? " " + value
            : value;
    }
}