using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Clients
{
    internal static class ClientUtils
    {
        public static void SanitizeParameter(IDbDataParameter parameter)
        {
            // We need to check if the object is an typeof List, Array, etc. If it is, then the process will fail because of "The CLR Type <> isn't supported" error.

            parameter.Value ??= DBNull.Value;

            // FIX: for Npgsql mismatches an empty string "" with System.Type
            var parameterType = parameter.Value?.ToString() != string.Empty
                ? parameter.Value.GetType()
                : typeof(string);

            // check if parameterType it's an Array or List, covert the Value to string, to be supported.
            if (parameterType.IsArray || parameterType.IsGenericType)
            {
                parameter.Value = string.Join(",", ((IList)parameter.Value).Cast<string>());
            }

            // TODO: Further investigate, futureproof and test a proper way to hadle if the Value's object is not a List<>.
            // If not handled in the above condition, it will fail when we add the Parameter to the command.
        }

        public static async Task<DataTable> GetRestrictedSchema(
            DbConnection connection,
            string collectionName,
            string tableName = null,
            string schema = null)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                tableName = null;
            }

            // Read more about syntax of restrictions here: https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/schema-restrictions
            var restrictions = new[] { null, schema, tableName, null };
            return connection.GetSchema(collectionName, restrictions);
        }
    }
}
