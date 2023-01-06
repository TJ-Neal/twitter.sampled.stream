using Neal.Twitter.Application.Constants.Keys;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Application.Interfaces.TweetRepository;
using Neal.Twitter.Infrastructure.Simple.Repository.Endpoints;
using Neal.Twitter.Infrastructure.Simple.Repository.Services.Repository;
using Serilog;

var builder = WebApplication
    .CreateBuilder(args);

// Attach Serilog logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

Log.Information(ApplicationStatusMessages.Started);

builder
    .Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddMemoryCache()
    .AddSingleton<ITweetRepository, TweetRepository>(); // Single instance of Tweet Repository without persistence

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()
    || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // app.UseHealthChecks(Path.Combine(""));
}

app.UseRouting()
    .UseEndpoints(configuration =>
        configuration.MapRepositoryEndpoints(ApplicationConfigurationKeys.Simple));

await app.RunAsync();