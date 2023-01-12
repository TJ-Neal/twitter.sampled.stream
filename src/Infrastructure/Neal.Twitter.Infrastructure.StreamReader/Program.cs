using Confluent.Kafka;
using LinqToTwitter.OAuth;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Client.Faster.Extensions;
using Neal.Twitter.Client.Kafka.Extensions;
using Neal.Twitter.Client.Simple.Extensions;
using Neal.Twitter.Core.Entities.Configuration;
using Neal.Twitter.Infrastructure.StreamReader.Extensions;
using Neal.Twitter.Infrastructure.StreamReader.Services.TwitterApi.V2;

// Define thread to periodically send log messages for heartbeats
var heartbeatThread = new Thread(new ThreadStart(() =>
{
    while (true)
    {
        Thread.Sleep(TimeSpan.FromMinutes(1));
        Log.Information($"{nameof(TwitterStreamReaderService)} heartbeat good");
    }
}));

try
{
    // Load configuration
    var configuration = new ConfigurationBuilder()
        .AddJsonFile($"appsettings.json", false, true)
        .AddEnvironmentVariables()
        .AddUserSecrets<Program>()
        .Build();

    // Attach Serilog logger
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();

    Log.Information(ApplicationStatusMessages.Started);

    // Load configurations from appsettings for individual services
    var inMemoryCredentialStore = configuration
        .GetSection(nameof(InMemoryCredentialStore))
        .Get<InMemoryCredentialStore>()
            ?? new InMemoryCredentialStore();
    var kafkaProducerConfiguration = configuration
        .GetSection(nameof(WrapperConfiguration<ProducerConfig>))
        .Get<WrapperConfiguration<ProducerConfig>>()
            ?? new WrapperConfiguration<ProducerConfig>();
    var fasterConfiguration = configuration
        .GetSection(nameof(FasterConfiguration))
        .Get<FasterConfiguration>()
            ?? new FasterConfiguration();
    var simpleConfiguration = configuration
        .GetSection(nameof(SimpleConfiguration))
        .Get<SimpleConfiguration>()
            ?? new SimpleConfiguration();

    // Output the enabled state of services
    Log.Information($"Kafka client enabled [{kafkaProducerConfiguration.Enabled}]");
    Log.Information($"Faster client enabled [{fasterConfiguration.Enabled}]");
    Log.Information($"Simple client enabled [{simpleConfiguration.Enabled}]");

    // Configure and build Host Service
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            services
                .Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromSeconds(10))
                .Configure<JsonSerializerOptions>(options =>
                {
                    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.DefaultIgnoreCondition = JsonIgnoreCondition.Always;
                })
                .AddLogging(options =>
                {
                    options.ClearProviders();
                    options.AddSerilog(dispose: true);
                });

            services
                .AddTwitterStreamConsumer(inMemoryCredentialStore)
                .AddKafkaHandlerIfEnabled(kafkaProducerConfiguration)
                .AddFasterRepositoryHandlerIfEnabled(fasterConfiguration)
                .AddSimpleRepositoryHandlerIfEnabled(simpleConfiguration)
                .AddHostedService<TwitterStreamReaderService>(); // Add the hosted service for reading the twitter stream
        })
        .UseSerilog()
        .Build();

    Log.Information(ApplicationStatusMessages.HostedServiceStarting);

    heartbeatThread.Start();

    // Start hosted service execution
    await host.RunAsync();

    Log.Information(ApplicationStatusMessages.HostedServiceFinished);
}
catch (Exception ex)
{
    // Write to console in case log initialization fails
    Console.WriteLine($"{DateTime.UtcNow}Unexpected exception occurred:\n{ex}");
    Log.Fatal(ex, ApplicationStatusMessages.FatalError);
}
// Clean up application and ensure log is flushed
finally
{
    Log.Information(ApplicationStatusMessages.Stopped);
    Log.CloseAndFlush();
}