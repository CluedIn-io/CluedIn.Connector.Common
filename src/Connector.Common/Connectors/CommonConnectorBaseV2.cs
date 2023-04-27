using CluedIn.Core;
using CluedIn.Core.Connectors;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CluedIn.Connector.Common.Connectors
{
    public abstract class CommonConnectorBaseV2<TConnector, TClient> : ConnectorBaseV2
        where TConnector : CommonConnectorBaseV2<TConnector, TClient>
    {
        protected readonly TClient _client;
        protected readonly ILogger<TConnector> _logger;

        protected CommonConnectorBaseV2(ILogger<TConnector> logger, TClient client, Guid providerId, bool supportsRetrievingLatestEntityPersistInfo)
            : base(providerId, supportsRetrievingLatestEntityPersistInfo)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        protected string GetCodesContainerName(string containerName) => $"{containerName}Codes";
        protected string GetIncomingEdgesContainerName(string containerName) => $"{containerName}IncomingEdges";
        protected string GetIncomingEdgesPropertiesContainerName(string containerName) => $"{containerName}IncomingEdgesProperties";
        protected string GetOutgoingEdgesContainerName(string containerName) => $"{containerName}OutgoingEdges";
        protected string GetOutgoingEdgesPropertiesContainerName(string containerName) => $"{containerName}OutgoingEdgesProperties";

        /// <summary>
        ///     Strip non-alpha numeric characters
        /// </summary>
        public override Task<string> GetValidMappingDestinationPropertyName(ExecutionContext executionContext, Guid connectorProviderDefinitionId, string containerName)
        {
            return Task.FromResult(Regex.Replace(containerName, @"[^A-Za-z0-9]+", ""));
        }

        protected virtual async Task<string> GetValidContainerName(ExecutionContext executionContext, Guid connectorProviderDefinitionId, string containerName, Func<ExecutionContext, Guid, string, Task<bool>> checkTableExistPredicate)
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
