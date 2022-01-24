using RestSharp.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace CluedIn.Connector.Common.Configurations
{

    public static class CommonConfigurationNames
    {
        public static string Host = nameof(Host).ToCamelCase(CultureInfo.CurrentCulture);

        [Display(Name = "Database Name")]
        public static string DatabaseName = nameof(DatabaseName).ToCamelCase(CultureInfo.CurrentCulture);
        
        public static string Username = nameof(Username).ToCamelCase(CultureInfo.CurrentCulture);
        
        public static string Password = nameof(Password).ToCamelCase(CultureInfo.CurrentCulture);

        [Display(Name = "Port Number")]
        public static string PortNumber = nameof(PortNumber).ToCamelCase(CultureInfo.CurrentCulture);
        
        public static string Schema = nameof(Schema).ToCamelCase(CultureInfo.CurrentCulture);

        public static string ToDisplayName(this string propertyName)
        {
            var prop = typeof(CommonConfigurationNames).GetField(propertyName);

            if (prop != null)
            {
                var attr = prop.GetAttribute<DisplayAttribute>();
                if (attr != null) return attr.Name;
            }

            return propertyName;
        }
    }
}
