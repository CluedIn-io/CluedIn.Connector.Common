using CluedIn.Core;
using CluedIn.Core.Crawling;
using CluedIn.Core.Data.Relational;
using CluedIn.Core.Providers;
using CluedIn.Core.Webhooks;
using CluedIn.Providers.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connector.Common
{
    public abstract class ConnectorProviderBase<TConnectorProvider> : ProviderBase, IExtendedProviderMetadata
        where TConnectorProvider : ConnectorProviderBase<TConnectorProvider>
    {
        private readonly IConfigurationConstants _configurationConstants;
        private readonly ILogger<TConnectorProvider> _logger;

        protected ConnectorProviderBase([NotNull] ApplicationContext appContext,
            [NotNull] IConfigurationConstants configurationConstants, [NotNull] ILogger<TConnectorProvider> logger)
            : base(appContext, configurationConstants.CreateProviderMetadata())
        {
            _configurationConstants = configurationConstants;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Names for configuration parts to compose provider display name on UI.
        /// </summary>
        protected virtual IEnumerable<string> ProviderNameParts => Enumerable.Empty<string>();

        public string Icon => _configurationConstants.Icon;
        public string Domain => _configurationConstants.Domain;
        public string About => _configurationConstants.About;
        public AuthMethods AuthMethods => _configurationConstants.AuthMethods;
        public IEnumerable<Control> Properties => _configurationConstants.Properties;
        public Guide Guide => _configurationConstants.Guide;
        public new IntegrationType Type => _configurationConstants.Type;

        /// <summary>
        ///     Usually called first from Crawler methods. Don't have much impact.
        /// </summary>
        public override Task<CrawlJobData> GetCrawlJobData(
            ProviderUpdateContext context,
            IDictionary<string, object> configuration,
            Guid organizationId,
            Guid userId,
            Guid providerDefinitionId)
        {
            // WARNING: The log output can contain sensitive information
            _logger.LogDebug($"GetCrawlJobData config input: {JsonConvert.SerializeObject(configuration)}");
            return Task.FromResult<CrawlJobData>(new CrawlJobDataWrapper(configuration));
        }

        /// <summary>
        ///     Called second from Crawler methods on creating export target.
        ///     accountDisplay could create suffix after connector name on UI.
        /// </summary>
        public override Task<AccountInformation> GetAccountInformation(ExecutionContext context,
            [NotNull] CrawlJobData jobData, Guid organizationId, Guid userId, Guid providerDefinitionId)
        {
            _logger.LogDebug($"GetAccountInformation CrawlJobData input: {JsonConvert.SerializeObject(jobData)}");

            if (!(jobData is CrawlJobDataWrapper dataWrapper))
                return Task.FromResult(new AccountInformation(string.Empty, string.Empty));

            var displayNameBuilder = new StringBuilder();
            var configurations = dataWrapper.Configurations;
            foreach (var key in ProviderNameParts)
                if (configurations.TryGetValue(key, out var value) && value != null)
                    displayNameBuilder.Append(value).Append(" ");

            return Task.FromResult(new AccountInformation(string.Empty, displayNameBuilder.ToString()));
        }

        /// <summary>
        ///     Usually called second from Crawler methods. Don't have much impact.
        /// </summary>
        public override Task<IDictionary<string, object>> GetHelperConfiguration(
            ProviderUpdateContext context,
            [NotNull] CrawlJobData jobData,
            Guid organizationId,
            Guid userId,
            Guid providerDefinitionId)
        {
            _logger.LogDebug($"GetHelperConfiguration CrawlJobData input: {JsonConvert.SerializeObject(jobData)}");

            if (jobData is CrawlJobDataWrapper dataWrapper)
                return Task.FromResult(dataWrapper.Configurations);

            return Task.FromResult<IDictionary<string, object>>(new Dictionary<string, object>());
        }

        #region NotImplementedCrawlerMembers

        public override string Schedule(DateTimeOffset relativeDateTime, bool webHooksEnabled)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> TestAuthentication(ProviderUpdateContext context,
            IDictionary<string, object> configuration, Guid organizationId, Guid userId, Guid providerDefinitionId)
        {
            throw new NotImplementedException();
        }

        public override Task<ExpectedStatistics> FetchUnSyncedEntityStatistics(ExecutionContext context,
            IDictionary<string, object> configuration, Guid organizationId, Guid userId, Guid providerDefinitionId)
        {
            throw new NotImplementedException();
        }

        public override Task<IDictionary<string, object>> GetHelperConfiguration(ProviderUpdateContext context,
            CrawlJobData jobData, Guid organizationId, Guid userId, Guid providerDefinitionId, string folderId)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<WebHookSignature>> CreateWebHook(ExecutionContext context,
            [NotNull] CrawlJobData jobData, [NotNull] IWebhookDefinition webhookDefinition,
            [NotNull] IDictionary<string, object> config)
        {
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<WebhookDefinition>> GetWebHooks(ExecutionContext context)
        {
            throw new NotImplementedException();
        }

        public override Task DeleteWebHook(ExecutionContext context, [NotNull] CrawlJobData jobData,
            [NotNull] IWebhookDefinition webhookDefinition)
        {
            throw new NotImplementedException();
        }

        public override Task<CrawlLimit> GetRemainingApiAllowance(ExecutionContext context,
            [NotNull] CrawlJobData jobData, Guid organizationId, Guid userId, Guid providerDefinitionId)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> WebhookManagementEndpoints([NotNull] IEnumerable<string> ids)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
