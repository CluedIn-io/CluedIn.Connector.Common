using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Clients
{
    public interface ITransactionalClientBaseV2<TConnection, TTransaction, TParameter>
        where TConnection : IDbConnection
        where TTransaction : IDbTransaction
        where TParameter : IDbDataParameter
    {
        Task ExecuteCommandInTransactionAsync(TTransaction transaction, string commandText, IEnumerable<TParameter> param = null);
        Task<ConnectionAndTransaction<TConnection, TTransaction>> BeginTransaction(IReadOnlyDictionary<string, object> config);
        Task<DataTable> GetTableColumns(TTransaction transaction, string tableName, string schema = null);
        Task<DataTable> GetTables(TTransaction transaction, string name = null, string schema = null);
    }
}
