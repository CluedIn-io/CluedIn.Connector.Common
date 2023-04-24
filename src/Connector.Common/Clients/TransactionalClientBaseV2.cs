using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Clients
{
    public abstract class TransactionalClientBaseV2<TConnection, TTransaction, TParameter> : ITransactionalClientBaseV2<TConnection, TTransaction, TParameter>
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

        public async Task<ConnectionAndTransaction<TConnection, TTransaction>> BeginTransaction(IReadOnlyDictionary<string, object> config)
        {
            var connectionString = BuildConnectionString(config);
            var connection = Activator.CreateInstance(typeof(TConnection), connectionString) as TConnection;

            // ReSharper disable once PossibleNullReferenceException
            await connection.OpenAsync();
            return new ConnectionAndTransaction<TConnection, TTransaction>(connection, await connection.BeginTransactionAsync() as TTransaction);
        }

        public async Task<DataTable> GetTableColumns(TTransaction transaction, string tableName, string schema = null)
        {
            return await ClientUtils.GetRestrictedSchema(transaction.Connection, "Columns", tableName, schema);
        }

        public async Task<DataTable> GetTables(TTransaction transaction, string name = null, string schema = null)
        {
            return await ClientUtils.GetRestrictedSchema(transaction.Connection, "Tables", name, schema);
        }

        public abstract string BuildConnectionString(IReadOnlyDictionary<string, object> config);
    }
}
