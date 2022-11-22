using CluedIn.Core.Connectors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Clients
{
    public abstract class ClientBase<TConnection, TParameter> : IClientBase<TConnection, TParameter>
        where TConnection : DbConnection, new()
        where TParameter : class, IDbDataParameter
    {
        public virtual async Task ExecuteCommandAsync(IConnectorConnection config, string commandText,
            IEnumerable<TParameter> param = null)
        {
            await using var connection = await GetConnection(config.Authentication);

            var cmd = connection.CreateCommand();
            cmd.CommandText = commandText;

            // use Add instead of AddRange.
            // We need to check if the object is an typeof List, Array, etc. If it is, then the process will fail because of "The CLR Type <> isn't supported" error.
            if (param != null)
                foreach (var parameter in param)
                {
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
                    cmd.Parameters.Add(parameter);

                }

            await cmd.ExecuteNonQueryAsync();
        }

        public virtual async Task<TConnection> GetConnection(IDictionary<string, object> config)
        {
            var connectionString = BuildConnectionString(config);
            var connection = Activator.CreateInstance(typeof(TConnection), connectionString) as TConnection;

            // ReSharper disable once PossibleNullReferenceException
            await connection.OpenAsync();
            return connection;
        }

        public virtual async Task<DataTable> GetTableColumns(IDictionary<string, object> config, string tableName, string schema = null)
        {
            return await GetRestrictedSchema(config, "Columns", tableName: tableName, schema: schema);
        }

        public virtual async Task<DataTable> GetTables(IDictionary<string, object> config, string name = null, string schema = null)
        {
            return await GetRestrictedSchema(config, "Tables", tableName: name, schema: schema);
        }

        protected virtual async Task<DataTable> GetRestrictedSchema(
            IDictionary<string, object> config,
            string collectionName,
            string tableName = null,
            string schema = null)
        {
            await using var connection = await GetConnection(config);
            if (string.IsNullOrWhiteSpace(tableName))
            {
                tableName = null;
            }

            // Read more about syntax of restrictions here: https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/schema-restrictions
            var restrictions = new[] { null, schema, tableName, null };
            return connection.GetSchema(collectionName, restrictions);
        }

        public abstract string BuildConnectionString(IDictionary<string, object> config);
    }
}
