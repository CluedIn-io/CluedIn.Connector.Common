using CluedIn.Core;
using CluedIn.Core.Connectors;
using CluedIn.Core.DataStore;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Connector.Common
{
    public abstract class CommonConnectorBase<TConnector, TClient> : ConnectorBase
        where TConnector : CommonConnectorBase<TConnector, TClient>
    {
        protected readonly TClient _client;
        protected readonly ICommonServiceHolder _commonServiceHolder;
        protected readonly ILogger<TConnector> _logger;

        protected CommonConnectorBase(IConfigurationRepository repository, ILogger<TConnector> logger, TClient client,
            ICommonServiceHolder commonServiceHolder, Guid providerId) : base(repository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _commonServiceHolder = commonServiceHolder ?? throw new ArgumentNullException(nameof(commonServiceHolder));

            ProviderId = providerId;
        }

        /// <summary>
        ///     Strip non-alpha numeric characters
        /// </summary>
        public override Task<string> GetValidDataTypeName(ExecutionContext executionContext, Guid providerDefinitionId,
            string name)
        {
            return Task.FromResult(Regex.Replace(name, @"[^A-Za-z0-9]+", ""));
        }
    }
}
