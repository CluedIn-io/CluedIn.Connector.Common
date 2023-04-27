using CluedIn.Core;
using CluedIn.Core.Connectors;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Connectors
{
    public abstract class CommonTransactionalConnectorBaseV2 : ConnectorBaseV2
    {
        protected CommonTransactionalConnectorBaseV2(Guid providerId, bool supportsRetrievingLatestEntityPersistInfo) : base(providerId, supportsRetrievingLatestEntityPersistInfo)
        {
        }

        protected string GetCodesContainerName(string containerName) => $"{containerName}Codes";
        protected string GetIncomingEdgesContainerName(string containerName) => $"{containerName}IncomingEdges";
        protected string GetIncomingEdgesPropertiesContainerName(string containerName) => $"{containerName}IncomingEdgesProperties";
        protected string GetOutgoingEdgesContainerName(string containerName) => $"{containerName}OutgoingEdges";
        protected string GetOutgoingEdgesPropertiesContainerName(string containerName) => $"{containerName}OutgoingEdgesProperties";

        /// <summary>
        /// Strip non-alpha numeric characters
        /// </summary>
        public override Task<string> GetValidMappingDestinationPropertyName(
            ExecutionContext executionContext,
            Guid connectorProviderDefinitionId,
            string name)
        {
            return Task.FromResult(Regex.Replace(name, @"[^A-Za-z0-9]+", ""));
        }

        protected virtual async Task<string> GetValidContainerName(
            ExecutionContext executionContext,
            Guid connectorProviderDefinitionId,
            string containerName,
            Func<ExecutionContext, Guid, string, Task<bool>> checkTableExistPredicate)
        {
            // Strip non-alpha numeric characters
            var cleanName = Regex.Replace(containerName, @"[^A-Za-z0-9]+", "");

            if (!await checkTableExistPredicate(executionContext, connectorProviderDefinitionId, cleanName))
                return cleanName;

            // If exists, append count like in windows explorer
            var count = 0;
            string newName;
            do
            {
                count++;
                newName = $"{cleanName}{count}";
            } while (await checkTableExistPredicate(executionContext, connectorProviderDefinitionId, newName));

            return newName;
        }
    }
}
