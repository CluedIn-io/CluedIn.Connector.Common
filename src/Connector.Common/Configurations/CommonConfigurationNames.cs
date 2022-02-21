using RestSharp.Extensions;
using System.ComponentModel.DataAnnotations;

namespace CluedIn.Connector.Common.Configurations
{

    public static class CommonConfigurationNames
    {
        [Display(Name = "Host")]
        public static string Host = nameof(Host);

        [Display(Name = "Database Name")]
        public static string DatabaseName = nameof(DatabaseName);

        [Display(Name= "Username")]
        public static string Username = nameof(Username);

        [Display(Name = "Password")]
        public static string Password = nameof(Password);

        [Display(Name = "Port Number")]
        public static string PortNumber = nameof(PortNumber);

        [Display(Name = "Schema")]
        public static string Schema = nameof(Schema);

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
