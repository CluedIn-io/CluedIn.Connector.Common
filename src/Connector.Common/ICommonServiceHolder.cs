using CluedIn.Core;
using CluedIn.Core.Connectors;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common
{
    public interface ICommonServiceHolder
    {
        Task<bool> CheckTableExists<TConnector, TConnection, TParameter>(ExecutionContext executionContext,
            Guid providerDefinitionId, string name,
            IClientBase<TConnection, TParameter> client, TConnector connector, ILogger<TConnector> logger)
            where TConnector : ConnectorBase
            where TConnection : IDbConnection, new()
            where TParameter : IDbDataParameter;

        Task<string> GetValidContainerName(ExecutionContext executionContext, Guid providerDefinitionId, string name,
            Func<ExecutionContext, Guid, string, Task<bool>> checkTableExistPredicate);
    }
}
