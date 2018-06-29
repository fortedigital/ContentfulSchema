namespace Forte.ContentfulSchema
{
    internal static class StringExtensions
    {
        public static string ToCamelcase(this string s)
        {
            return char.ToLower(s[0]) + s.Substring(1);
        }
    }
}