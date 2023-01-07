using Neal.Twitter.Application.Constants.Keys;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Application.Interfaces.TweetRepository;
using Neal.Twitter.Infrastructure.Faster.API.Endpoints;
using Neal.Twitter.Infrastructure.Faster.Repository.Services.Repository;
using Serilog;

var builder = WebApplication
    .CreateBuilder(args);

// Attach Serilog logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

Log.Warning(ApplicationStatusMessages.Started);

builder
    .Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddMemoryCache()
    .AddSingleton<ITweetRepository, FasterTweetRepository>() // Single instance of Tweet Repository with persistence
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

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

app
    .UseRouting()
    .UseEndpoints(configuration => configuration.MapRepositoryEndpoints(ApplicationConfigurationKeys.Faster));

await app.RunAsync();