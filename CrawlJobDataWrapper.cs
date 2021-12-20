using CluedIn.Core.Crawling;
using System.Collections.Generic;

namespace Connector.Common
{
    /// <summary>
    ///     Used to connect and  wrap configurations to CrawlJobData. Because crawler methods are designed to pass around
    ///     children of CrawlJobData.
    ///     Target methods are: ProviderBase.GetCrawlJobData, ProviderBase.GetAccountInformation and
    ///     ProviderBase.GetHelperConfiguration etc.
    /// </summary>
    public class CrawlJobDataWrapper : CrawlJobData
    {
        public CrawlJobDataWrapper(IDictionary<string, object> configurations)
        {
            Configurations = configurations;
        }

        public IDictionary<string, object> Configurations { get; }
    }
}
