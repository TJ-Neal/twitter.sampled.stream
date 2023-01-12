using Confluent.Kafka;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Client.Kafka.Interfaces;
using Neal.Twitter.Client.Kafka.Wrappers;
using Neal.Twitter.Core.Entities.Configuration;
using Neal.Twitter.Infrastructure.KafkaReader.Services.Kafka;

// Define thread to periodically send warning messages for heartbeats
var heartbeatThread = new Thread(new ThreadStart(() =>
{
    while (true)
    {
        Thread.Sleep(TimeSpan.FromMinutes(1));
        Log.Information($"{nameof(KafkaConsumerService)} heartbeat good");
    }
}));

try
{
    // Load configuration
    var configuration = new ConfigurationBuilder()
        .AddJsonFile($"appsettings.json", true, true)
        .AddEnvironmentVariables()
        .AddUserSecrets<Program>()
        .Build();

    // Attach Serilog logger
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();

    Log.Information(ApplicationStatusMessages.Started);

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

            var wrapperConfiguration = configuration
                .GetSection(nameof(WrapperConfiguration<ConsumerConfig>))
                .Get<WrapperConfiguration<ConsumerConfig>>()
                    ?? new WrapperConfiguration<ConsumerConfig>();

            services
                .AddHttpClient()
                .AddSingleton(wrapperConfiguration)
                .AddSingleton<IKafkaConsumerWrapper<string, string>, KafkaConsumerWrapper>()
                .AddHostedService<KafkaConsumerService>(); // Add the hosted service for reading the twitter stream
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
finally
{
    Log.Information(ApplicationStatusMessages.Stopped);
    Log.CloseAndFlush();
}