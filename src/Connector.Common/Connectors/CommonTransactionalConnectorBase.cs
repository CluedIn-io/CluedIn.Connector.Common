using CluedIn.Core;
using CluedIn.Core.Connectors;
using CluedIn.Core.DataStore;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Connectors
{
    [Obsolete("Use CommonTransactionalConnectorBaseV2 instead.")]
    public abstract class CommonTransactionalConnectorBase : ConnectorBase
    {
        protected CommonTransactionalConnectorBase(IConfigurationRepository configurationRepository, Guid providerId) : base(configurationRepository)
        {
            ProviderId = providerId;
        }

        protected string GetEdgesContainerName(string containerName) => $"{containerName}Edges";

        /// <summary>
        /// Strip non-alpha numeric characters
        /// </summary>
        public override Task<string> GetValidDataTypeName(
            ExecutionContext executionContext,
            Guid providerDefinitionId,
            string name)
        {
            return Task.FromResult(Regex.Replace(name, @"[^A-Za-z0-9]+", ""));
        }

        protected virtual async Task<string> GetValidContainerName(
            ExecutionContext executionContext,
            Guid providerDefinitionId,
            string name,
            Func<ExecutionContext, Guid, string, Task<bool>> checkTableExistPredicate)
        {
            // Strip non-alpha numeric characters
            var cleanName = Regex.Replace(name, @"[^A-Za-z0-9]+", "");

            if (!await checkTableExistPredicate(executionContext, providerDefinitionId, cleanName))
                return cleanName;

            // If exists, append count like in windows explorer
            var count = 0;
            string newName;
            do
            {
                count++;
                newName = $"{cleanName}{count}";
            } while (await checkTableExistPredicate(executionContext, providerDefinitionId, newName));

            return newName;
        }
    }
}
