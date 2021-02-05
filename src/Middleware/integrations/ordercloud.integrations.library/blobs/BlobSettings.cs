namespace ordercloud.integrations.library
{
    public class BlobSettings
    {
        public string ConnectionString { get; set; }
        public string ContainerNameQueue { get; set; }
        public string ContainerNameCache { get; set; }
        public string ContainerNameExchangeRates { get; set; }
        public string HostUrl { get; set; }
        public string EnvironmentString { get; set; }
    }
}
