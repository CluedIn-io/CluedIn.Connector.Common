namespace Connector.Common
{
    // TODO: ROK: some of these properties are not common and need to be in specific connectors
    public static class CommonConfigurationNames
    {
        public const string Host = nameof(Host);
        public const string DatabaseName = nameof(DatabaseName);
        public const string Username = nameof(Username);
        public const string Password = nameof(Password);
        public const string PortNumber = nameof(PortNumber);
        public const string Schema = nameof(Schema);
        public const string SSLMode = nameof(SSLMode);
        public const string Account = nameof(Account);
        public const string Role = nameof(Role);
        public const string Warehouse = nameof(Warehouse);
        public const string Url = nameof(Url);
        public const string AccountName = nameof(AccountName);
        public const string AccountKey = nameof(AccountKey);
        public const string FileSystemName = nameof(FileSystemName);
        public const string DirectoryName = nameof(DirectoryName);
    }
}
