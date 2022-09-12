using CluedIn.Connector.Common.Clients;
using CluedIn.Connector.Common.Helpers;
using CluedIn.Core;
using CluedIn.Core.DataStore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Connectors
{
    public abstract class SqlConnectorBase<TConnector, TConnection, TParameter> : CommonConnectorBase<TConnector, IClientBase<TConnection, TParameter>>
        where TConnector : SqlConnectorBase<TConnector, TConnection, TParameter>
        where TConnection : IDbConnection, new()
        where TParameter : IDbDataParameter
    {
        protected SqlConnectorBase(IConfigurationRepository repository, ILogger<TConnector> logger, IClientBase<TConnection, TParameter> client,
            Guid providerId) : base(repository, logger, client, providerId)
        {
        }

        public override async Task<bool> VerifyConnection(ExecutionContext executionContext,
            IDictionary<string, object> config)
        {
            try
            {
                var connection = await _client.GetConnection(config);

                return connection.State == ConnectionState.Open;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error verifying connection");
                return false;
            }
        }
        public override Task<string> GetValidDataTypeName(ExecutionContext executionContext, Guid providerDefinitionId,
            string name)
        {
            return Task.FromResult(SqlStringSanitizer.Sanitize(name));
        }

        public override async Task<string> GetValidContainerName(ExecutionContext executionContext,
            Guid providerDefinitionId, string name)
        {
            var cleanName = SqlStringSanitizer.Sanitize(name);

            if (!await CheckTableExists(executionContext, providerDefinitionId, cleanName))
                return cleanName;

            // If exists, append count like in windows explorer
            var count = 0;
            string newName;
            do
            {
                count++;
                newName = $"{cleanName}{count}";
            } while (await CheckTableExists(executionContext, providerDefinitionId, newName));

            return newName;
        }

        protected virtual string BuildEmptyContainerSql(string tableName)
        {
            return $"TRUNCATE TABLE [{SqlStringSanitizer.Sanitize(tableName)}]";
        }

        protected virtual async Task<bool> CheckTableExists(ExecutionContext executionContext,
            Guid providerDefinitionId,
            string name)
        {
            try
            {
                var config = await GetAuthenticationDetails(executionContext, providerDefinitionId);
                var tables = await _client.GetTables(config.Authentication, name);

                return tables.Rows.Count > 0;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error checking Container '{name}' exists for Connector {providerDefinitionId}");
                return false;
            }
        }
    }
}
