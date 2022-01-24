using CluedIn.Connector.Common.Configurations;
using Xunit;

namespace CluedIn.Connector.Common.Unit.Tests
{
    public class CommonConfigurationNamesTests
    {
        [Fact]
        public void ToDisplayNameWorks()
        {
            Assert.Equal("Host", CommonConfigurationNames.Host.ToDisplayName());
            Assert.Equal("Database Name", CommonConfigurationNames.DatabaseName.ToDisplayName());
            Assert.Equal("Username", CommonConfigurationNames.Username.ToDisplayName());
            Assert.Equal("Password", CommonConfigurationNames.Password.ToDisplayName());
            Assert.Equal("Port Number", CommonConfigurationNames.PortNumber.ToDisplayName());
            Assert.Equal("Schema", CommonConfigurationNames.Schema.ToDisplayName());
        }
    }
}
