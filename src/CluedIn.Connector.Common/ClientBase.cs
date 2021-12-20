using CluedIn.Core.Connectors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common
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

                    // check if parameter type is safe to add
                    if (!(parameterType.IsArray
                          || parameterType.IsGenericType))
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

        public virtual async Task<DataTable> GetTableColumns(IDictionary<string, object> config, string tableName)
        {
            return await GetRestrictedSchema(config, "Columns", tableName);
        }

        public virtual async Task<DataTable> GetTables(IDictionary<string, object> config, string name = null)
        {
            return await GetRestrictedSchema(config, "Tables", name);
        }

        protected virtual async Task<DataTable> GetRestrictedSchema(IDictionary<string, object> config,
            string collectionName, string restrictionValue = null)
        {
            await using var connection = await GetConnection(config);
            if (string.IsNullOrWhiteSpace(restrictionValue))
                return connection.GetSchema(collectionName);

            var restrictions = new string[4];
            restrictions[2] = restrictionValue;
            return connection.GetSchema(collectionName, restrictions);
        }

        public abstract string BuildConnectionString(IDictionary<string, object> config);
    }
}
