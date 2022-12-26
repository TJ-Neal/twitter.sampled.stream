namespace Neal.Twitter.Core.Entities.Kafka;

public class BrokerProducerSettings
{
    #region Properties

    public string BootstrapServers { get; set; } = string.Empty;

    public bool EnableIdemotence { get; set; }

    public string Topic { get; set; } = string.Empty;

    #endregion Properties
}