using CluedIn.Core;
using CluedIn.Core.Connectors;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Connector.Common
{
    public class CommonServiceHolder : ICommonServiceHolder
    {
        public async Task<bool> CheckTableExists<TConnector, TConnection, TParameter>(ExecutionContext executionContext,
            Guid providerDefinitionId,
            string name,
            IClientBase<TConnection, TParameter> client,
            TConnector connector,
            ILogger<TConnector> logger)
            where TConnector : ConnectorBase
            where TConnection : IDbConnection, new()
            where TParameter : IDbDataParameter
        {
            try
            {
                var config = await connector.GetAuthenticationDetails(executionContext, providerDefinitionId);
                var tables = await client.GetTables(config.Authentication, name);

                return tables.Rows.Count > 0;
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error checking Container '{name}' exists for Connector {providerDefinitionId}");
                return false;
            }
        }

        public async Task<string> GetValidContainerName(ExecutionContext executionContext, Guid providerDefinitionId,
            string name,
            Func<ExecutionContext, Guid, string, Task<bool>> checkTableExistPredicate)
        {
            // Strip non-alpha numeric characters
            var cleanName = Regex.Replace(name, @"[^A-Za-z0-9]+", "");

            if (!await checkTableExistPredicate(executionContext, providerDefinitionId, cleanName)) return cleanName;

            // If exists, append count like in windows explorer
            var count = 0;
            string newName;
            do
            {
                count++;
                newName = $"{cleanName}{count}";
            } while (await checkTableExistPredicate(executionContext, providerDefinitionId, newName));

            cleanName = newName;

            return cleanName;
        }
    }
}
