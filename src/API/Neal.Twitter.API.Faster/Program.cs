using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Neal.Twitter.API.Faster.Endpoints;
using Neal.Twitter.Application.Constants;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Application.Interfaces.TweetRepository;
using Neal.Twitter.Infrastructure.Faster.Repository.Services.Repository;
using Serilog;
using static System.Net.Mime.MediaTypeNames;

try
{
    var builder = WebApplication
        .CreateBuilder(args);

    builder
        .Logging
        .ClearProviders()
        .AddSerilog();

    // Attach Serilog logger
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();

    Log.Information(ApplicationStatusMessages.Started);

    if (!builder.Environment.IsProduction())
    {
        builder
            .Services
            .AddCors(options =>
            {
                options.AddPolicy("LocalhostPolicy",
                    builder => builder
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .SetIsOriginAllowed((host) =>
                            host.Contains("localhost")
                            || host.Contains("::1")
                            || host.Contains("127.0.0.1")
                            || host.Contains("0.0.0.0")) // Allow localhost addresses
                        .AllowAnyHeader());
            });
    }

    builder
        .Services
        .AddEndpointsApiExplorer()
        .AddSwaggerGen()
        .AddProblemDetails()
        .AddMemoryCache()
        .AddSingleton<ITweetRepository, FasterTweetRepository>() // Single instance of Tweet Repository with persistence
        .AddHealthChecks();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsProduction())
    {
        app
            .UseCors("LocalhostPolicy")
            .UseDeveloperExceptionPage()
            .UseSwagger()
            .UseSwaggerUI()
            .UseStatusCodePages(Text.Plain, "Server returned status code: {0}");
    }
    else
    {
        app
            .UseExceptionHandler("/Error")
            .UseHsts();
    }

    app
        .UseHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => false
        })
        .UseRouting()
        .UseEndpoints(configuration =>
            configuration.MapRepositoryEndpoints(Names.FasterApi));

    await app.RunAsync();
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