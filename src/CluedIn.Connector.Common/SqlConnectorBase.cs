using CluedIn.Core;
using CluedIn.Core.DataStore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common
{
    public abstract class SqlConnectorBase<TConnector, TClient> : CommonConnectorBase<TConnector, TClient>
        where TConnector : SqlConnectorBase<TConnector, TClient>
    {
        protected SqlConnectorBase(IConfigurationRepository repository, ILogger<TConnector> logger, TClient client,
            ICommonServiceHolder serviceHolder,
            Guid providerId) : base(repository, logger, client, serviceHolder, providerId)
        {
        }

        public override async Task<bool> VerifyConnection(ExecutionContext executionContext,
            IDictionary<string, object> config)
        {
            try
            {
                var connection = await GetDbConnection(config);

                return connection.State == ConnectionState.Open;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error verifying connection");
                return false;
            }
        }

        protected abstract Task<IDbConnection> GetDbConnection(IDictionary<string, object> config);

        protected virtual string BuildEmptyContainerSql(string tableName)
        {
            return $"TRUNCATE TABLE [{tableName.SqlSanitize()}]";
        }
    }
}
