using CluedIn.Connector.Common.Clients;
using CluedIn.Connector.Common.Helpers;
using CluedIn.Core;
using CluedIn.Core.DataStore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Connectors
{
    public abstract class SqlConnectorBase<TConnection, TTransaction, TParameter> : CommonTransactionalConnectorBase
        where TConnection : IDbConnection
        where TTransaction : IDbTransaction
        where TParameter : IDbDataParameter
    {
        protected readonly ILogger<SqlConnectorBase<TConnection, TTransaction, TParameter>> _logger;
        private readonly ITransactionalClientBase<TConnection, TTransaction, TParameter> _client;

        protected SqlConnectorBase(IConfigurationRepository repository, ILogger<SqlConnectorBase<TConnection, TTransaction, TParameter>> logger, ITransactionalClientBase<TConnection, TTransaction, TParameter> client,
            Guid providerId) : base(repository, providerId)
        {
            _logger = logger;
            _client = client;
        }

        public override async Task<bool> VerifyConnection(ExecutionContext executionContext, IDictionary<string, object> config)
        {
            try
            {
                await using var connectionAndTransaction = await _client.BeginTransaction(config);
                var connectionIsOpen = connectionAndTransaction.Connection.State == ConnectionState.Open;
                await connectionAndTransaction.DisposeAsync();

                return connectionIsOpen;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error verifying connection");
                return false;
            }
        }

        public override Task<string> GetValidDataTypeName(ExecutionContext executionContext, Guid providerDefinitionId, string name)
        {
            return Task.FromResult(SqlStringSanitizer.Sanitize(name));
        }

        public Task<string> GetValidDataTypeName(string name)
        {
            return Task.FromResult(SqlStringSanitizer.Sanitize(name));
        }

        public override async Task<string> GetValidContainerName(ExecutionContext executionContext, Guid providerDefinitionId, string name)
        {
            var config = await GetAuthenticationDetails(executionContext, providerDefinitionId);
            await using var connectionAndTransaction = await _client.BeginTransaction(config.Authentication);
            return await GetValidContainerNameInTransaction(executionContext, providerDefinitionId, connectionAndTransaction.Transaction, name);
        }

        public async Task<string> GetValidContainerNameInTransaction(ExecutionContext executionContext, Guid providerDefinitionId, TTransaction transaction, string name)
        {
            var cleanName = SqlStringSanitizer.Sanitize(name);

            if (!await CheckTableExists(executionContext, providerDefinitionId, transaction, cleanName))
                return cleanName;

            // If exists, append count like in windows explorer
            var count = 0;
            string newName;
            do
            {
                count++;
                newName = $"{cleanName}{count}";
            } while (await CheckTableExists(executionContext, providerDefinitionId, transaction, newName));

            return newName;
        }

        protected virtual string BuildEmptyContainerSql(string tableName)
        {
            return $"TRUNCATE TABLE [{SqlStringSanitizer.Sanitize(tableName)}]";
        }


        protected virtual async Task<bool> CheckTableExists(ExecutionContext executionContext, Guid providerDefinitionId, TTransaction transaction, string name)
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
