using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Clients
{
    public abstract class TransactionalClientBase<TConnection, TTransaction, TParameter> : ITransactionalClientBase<TTransaction, TParameter>
        where TConnection : DbConnection, new()
        where TTransaction : DbTransaction
        where TParameter : IDbDataParameter
    {
        public async Task ExecuteCommandInTransactionAsync(TTransaction transaction, string commandText, IEnumerable<TParameter> param = null)
        {
            var cmd = transaction.Connection.CreateCommand();
            cmd.CommandText = commandText;
            cmd.Transaction = transaction;

            if (param != null)
            {
                foreach (var parameter in param)
                {
                    ClientUtils.SanitizeParameter(parameter);
                    cmd.Parameters.Add(parameter);
                }
            }

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<TTransaction> BeginTransaction(IDictionary<string, object> config)
        {
            var connectionString = BuildConnectionString(config);
            var connection = Activator.CreateInstance(typeof(TConnection), connectionString) as TConnection;

            // ReSharper disable once PossibleNullReferenceException
            await connection.OpenAsync();
            return await connection.BeginTransactionAsync() as TTransaction;
        }

        public async Task<DataTable> GetTableColumns(TTransaction transaction, string tableName, string schema = null)
        {
            return await ClientUtils.GetRestrictedSchema(transaction.Connection, "Columns", tableName: tableName, schema: schema);
        }

        public async Task<DataTable> GetTables(TTransaction transaction, string name = null, string schema = null)
        {
            return await ClientUtils.GetRestrictedSchema(transaction.Connection, "Tables", tableName: name, schema: schema);
        }

        public abstract string BuildConnectionString(IDictionary<string, object> config);
    }
}
