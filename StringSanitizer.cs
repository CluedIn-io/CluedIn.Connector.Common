using System.Text.RegularExpressions;

namespace Connector.Common
{
    public static class StringSanitizer
    {
        /// <summary>
        ///     Bare-bones sanitization to prevent Sql Injection. Extra info here http://sommarskog.se/dynamic_sql.html
        /// </summary>
        public static string SqlSanitize(this string str)
        {
            return Regex.Replace(str, @"[^_A-Za-z0-9]+", "");
        }
    }
}
