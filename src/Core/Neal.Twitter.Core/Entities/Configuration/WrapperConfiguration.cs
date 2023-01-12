using Confluent.Kafka;

namespace Neal.Twitter.Core.Entities.Configuration;

public class WrapperConfiguration<TClientConfig> where TClientConfig : ClientConfig
{
    public bool Enabled { get; set; }

    public string? Topic { get; set; }

    public string? BaseUrl { get; set; }

    public TClientConfig? ClientConfig { get; set; }
}