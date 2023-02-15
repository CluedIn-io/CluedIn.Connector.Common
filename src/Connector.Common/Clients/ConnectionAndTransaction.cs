using System;
using System.Data;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Clients
{
    public class ConnectionAndTransaction<TConnection, TTransaction> : IDisposable, IAsyncDisposable
        where TConnection : IDbConnection
        where TTransaction : IDbTransaction
    {
        public readonly TConnection Connection;
        public readonly TTransaction Transaction;

        public ConnectionAndTransaction(TConnection connection, TTransaction transaction)
        {
            Connection = connection;
            this.Transaction = transaction;
        }

        public void Dispose()
        {
            Transaction.Dispose();
            Connection.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            if (Transaction is IAsyncDisposable t)
            {
                await t.DisposeAsync();
            }
            else
            {
                Transaction.Dispose();
            }

            if (Connection is IAsyncDisposable c)
            {
                await c.DisposeAsync();
            }
            else
            {
                Connection.Dispose();
            }
        }
    }
}
