using CluedIn.Connector.Common.Clients;
using CluedIn.Connector.Common.Helpers;
using CluedIn.Core;
using CluedIn.Core.Connectors;
using CluedIn.Core.Data;
using CluedIn.Core.DataStore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Connectors
{
    public abstract class SqlConnectorBaseV2<TConnection, TTransaction, TParameter> : CommonTransactionalConnectorBaseV2
        where TConnection : IDbConnection
        where TTransaction : IDbTransaction
        where TParameter : IDbDataParameter
    {
        protected readonly ILogger<SqlConnectorBaseV2<TConnection, TTransaction, TParameter>> _logger;
        private readonly ITransactionalClientBaseV2<TConnection, TTransaction, TParameter> _client;

        protected SqlConnectorBaseV2(
            ILogger<SqlConnectorBaseV2<TConnection, TTransaction, TParameter>> logger,
            ITransactionalClientBaseV2<TConnection, TTransaction, TParameter> client,
            Guid providerId,
            bool supportsRetrievingLatestEntityPersistInfo)
            : base(providerId, supportsRetrievingLatestEntityPersistInfo)
        {
            _logger = logger;
            _client = client;
        }

        public override async Task<ConnectionVerificationResult> VerifyConnection(ExecutionContext executionContext, IReadOnlyDictionary<string, object> config)
        {
            try
            {
                await using var connectionAndTransaction = await _client.BeginTransaction(config);
                var connectionIsOpen = connectionAndTransaction.Connection.State == ConnectionState.Open;
                await connectionAndTransaction.DisposeAsync();

                return new ConnectionVerificationResult(connectionIsOpen);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error verifying connection");
                return new ConnectionVerificationResult(false, e.Message);
            }
        }

        public override Task<string> GetValidMappingDestinationPropertyName(ExecutionContext executionContext, Guid connectorProviderDefinitionId, string propertyName)
        {
            return Task.FromResult(SqlStringSanitizer.Sanitize(propertyName));
        }

        public Task<string> GetValidMappingDestinationPropertyName(string propertyName)
        {
            return Task.FromResult(SqlStringSanitizer.Sanitize(propertyName));
        }

        public override async Task<string> GetValidContainerName(ExecutionContext executionContext, Guid connectorProviderDefinitionId, string containerName)
        {
            ;
            var config = await AuthenticationDetailsHelper.GetAuthenticationDetails(executionContext, connectorProviderDefinitionId);
            await using var connectionAndTransaction = await _client.BeginTransaction(config.Authentication);
            return await GetValidContainerNameInTransaction(executionContext, connectorProviderDefinitionId, connectionAndTransaction.Transaction, containerName);
        }

        public async Task<string> GetValidContainerNameInTransaction(ExecutionContext executionContext, Guid connectorProviderDefinitionId, TTransaction transaction, string containerName)
        {
            var cleanName = SqlStringSanitizer.Sanitize(containerName);

            if (!await CheckTableExists(executionContext, connectorProviderDefinitionId, transaction, cleanName))
                return cleanName;

            // If exists, append count like in windows explorer
            var count = 0;
            string newName;
            do
            {
                count++;
                newName = $"{cleanName}{count}";
            } while (await CheckTableExists(executionContext, connectorProviderDefinitionId, transaction, newName));

            return newName;
        }

        protected virtual string BuildEmptyContainerSql(string tableName)
        {
            return $"TRUNCATE TABLE [{SqlStringSanitizer.Sanitize(tableName)}]";
        }


        protected virtual async Task<bool> CheckTableExists(ExecutionContext executionContext, Guid connectorProviderDefinitionId, TTransaction transaction, string name)
        {
            try
            {
                var tables = await _client.GetTables(transaction, name);

                return tables.Rows.Count > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
