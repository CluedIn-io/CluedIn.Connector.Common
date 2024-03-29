﻿using CluedIn.Core.Connectors;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Clients
{
    public interface IClientBase<TConnection, TParameter>
        where TConnection : IDbConnection, new()
        where TParameter : IDbDataParameter
    {
        Task ExecuteCommandAsync(IConnectorConnection config, string commandText, IEnumerable<TParameter> param = null);
        Task<TConnection> GetConnection(IDictionary<string, object> config);
        Task<DataTable> GetTableColumns(IDictionary<string, object> config, string tableName, string schema = null);
        Task<DataTable> GetTables(IDictionary<string, object> config, string name = null, string schema = null);
    }
}
