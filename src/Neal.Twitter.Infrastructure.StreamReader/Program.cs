using MediatR;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Faster.Client.Extensions;
using Neal.Twitter.Infrastructure.StreamReader.Extensions;
using Neal.Twitter.Infrastructure.StreamReader.Services.TwitterApi.V2;
using Neal.Twitter.Kafka.Client.Extensions;
using Neal.Twitter.Simple.Client.Extensions;

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

    Log.Warning(ApplicationStatusMessages.Started);

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

            var mediatRTypes = new List<Type>();

            services
                .AddTwitterStreamConsumer(configuration) // Add twitter stream classes as configured
                .AddKafkaHandlerIfEnabled(configuration, mediatRTypes)
                .AddFasterRepositoryHandlerIfEnabled(configuration, mediatRTypes)
                .AddSimpleRepositoryHandlerIfEnabled(configuration, mediatRTypes)
                .AddMediatR(mediatRTypes.ToArray())
                .AddHostedService<TwitterStreamReaderService>(); // Add the hosted service for reading the twitter stream
        })
        .UseSerilog()
        .Build();

    Log.Information(ApplicationStatusMessages.HostedServiceStarting);

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