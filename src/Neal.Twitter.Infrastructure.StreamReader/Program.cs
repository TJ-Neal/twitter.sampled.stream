using Confluent.Kafka.DependencyInjection;
using LinqToTwitter.OAuth;
using MediatR;
using Neal.Twitter.Application.Constants;
using Neal.Twitter.Application.Constants.Configuration;
using Neal.Twitter.Application.Events.Notifications;
using Neal.Twitter.Application.Interfaces.LinqToTwitter;
using Neal.Twitter.Application.Wrappers.LinqToTwitter;
using Neal.Twitter.Infrastructure.StreamReader.Services.TwitterApi.V2;

try
{
    // Load configuration
    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
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
            var kafkaConfig = configuration
                .GetSection($"{SectionKeys.Kafka}")
                .AsEnumerable(makePathsRelative: true)
                .ToList();

            if (!kafkaConfig.Any(value => string.Equals(value.Key, KafkaValueKeys.BootstrapServers, StringComparison.Ordinal)))
            {
                throw new KeyNotFoundException($"Key {KafkaValueKeys.BootstrapServers} was not found and is required.");
            }

            // Add configurations for the services
            services
                .Configure<HostOptions>(options =>
                {
                    options.ShutdownTimeout = TimeSpan.FromSeconds(10);
                })
                .Configure<JsonSerializerOptions>(options =>
                {
                    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                });

            // Create application authorizer and authenticate with Twitter for TwitterContext initialization.
            var twitterCredentialStore = new InMemoryCredentialStore
            {
                ConsumerKey = configuration
                        .GetValue<string>($"{SectionKeys.Twitter}:{SectionKeys.ConsumerKey}"),
                ConsumerSecret = configuration
                        .GetValue<string>($"{SectionKeys.Twitter}:{SectionKeys.ConsumerSecret}"),
            };

            services
                .AddSingleton(typeof(ILinqToTwitterWrapper), typeof(LinqToTwitterWrapper))
                .AddSingleton(_ => new ApplicationOnlyAuthorizer { CredentialStore = twitterCredentialStore }); // Add implementations as singletons. Hosted Service is a singleton.

            // Add additional services to the services collection
            services
                .AddLogging(options =>
                {
                    options.ClearProviders();
                    options.AddSerilog(dispose: true);
                })
                .AddHostedService<TwitterStreamReaderService>()
                .AddKafkaClient(kafkaConfig.Select(option =>
                    new KeyValuePair<string, string>(
                        option.Key,
                        option.Value ?? string.Empty)))
                .AddMediatR(
                    typeof(Program).Assembly,
                    typeof(TweetReceivedNotification).Assembly); // Register Core project events/handlers
        })
        .UseSerilog()
        .Build();

    // Start hosted service execution
    await host.RunAsync();
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