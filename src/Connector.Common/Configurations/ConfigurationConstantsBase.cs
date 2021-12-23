using CluedIn.Core.Data.Relational;
using CluedIn.Core.Net.Mail;
using CluedIn.Core.Providers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CluedIn.Connector.Common.Configurations
{
    public abstract class ConfigurationConstantsBase : IConfigurationConstants
    {
        protected ConfigurationConstantsBase(Guid providerId, string providerName, string componentName, string icon,
            string domain, string about, AuthMethods authMethods, string guideDetails,
            IEnumerable<Control> properties = null,
            IntegrationType integrationType = IntegrationType.Connector,
            string guideInstructions = "Provide authentication instructions here, if applicable",
            string featureCategory = "Connectivity",
            string featureDescription = "Expenses and Invoices against customers")
        {
            ProviderId = providerId;
            ProviderName = providerName;
            ComponentName = componentName;
            Icon = icon;
            Domain = domain;
            About = about;
            AuthMethods = authMethods;
            GuideDetails = guideDetails;
            Properties = properties ?? Enumerable.Empty<Control>();
            Type = integrationType;
            GuideInstructions = guideInstructions;
            FeatureCategory = featureCategory;
            FeatureDescription = featureDescription;
        }

        private string GuideDetails { get; }
        private string GuideInstructions { get; }
        private string ProviderName { get; }
        private string ComponentName { get; }
        private string FeatureCategory { get; }
        private string FeatureDescription { get; }

        private ComponentEmailDetails ComponentEmailDetails => new ComponentEmailDetails
        {
            Features = new Dictionary<string, string> {{FeatureCategory, FeatureDescription}},
            Icon = ProviderIconFactory.CreateConnectorUri(ProviderId),
            ProviderName = ProviderName,
            ProviderId = ProviderId
        };

        public Guid ProviderId { get; }
        public string Icon { get; }
        public string Domain { get; }
        public string About { get; }
        public AuthMethods AuthMethods { get; }
        public IEnumerable<Control> Properties { get; }
        public IntegrationType Type { get; }

        public Guide Guide => new Guide
        {
            Instructions = GuideInstructions, Value = new List<string> {About}, Details = GuideDetails
        };

        public IProviderMetadata CreateProviderMetadata()
        {
            return new ProviderMetadata
            {
                Id = ProviderId,
                Name = ProviderName,
                ComponentName = ComponentName,
                Type = Type.ToString(),
                ComponentEmailDetails = ComponentEmailDetails
            };
        }
    }
}
