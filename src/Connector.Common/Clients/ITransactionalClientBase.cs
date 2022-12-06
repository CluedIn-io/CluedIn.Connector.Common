using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Clients
{
    public interface ITransactionalClientBase<TTransaction, TParameter>
        where TTransaction : IDbTransaction
        where TParameter : IDbDataParameter
    {
        Task ExecuteCommandInTransactionAsync(TTransaction transaction, string commandText, IEnumerable<TParameter> param = null);
        Task<TTransaction> BeginTransaction(IDictionary<string, object> config);
        Task<DataTable> GetTableColumns(TTransaction transaction, string tableName, string schema = null);
        Task<DataTable> GetTables(TTransaction transaction, string name = null, string schema = null);
    }
}
