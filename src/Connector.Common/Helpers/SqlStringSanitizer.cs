using System.Text.RegularExpressions;

namespace CluedIn.Connector.Common.Helpers
{
    public static class SqlStringSanitizer
    {
        /// <summary>
        ///     Bare-bones sanitization to prevent Sql Injection. Extra info here http://sommarskog.se/dynamic_sql.html
        /// </summary>
        public static string Sanitize(string str)
        {
            return Regex.Replace(str, @"[^_A-Za-z0-9]+", "");
        }
    }
}
