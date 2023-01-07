using Neal.Twitter.Application.Constants.Api;
using Neal.Twitter.Application.Constants.Messages;
using Neal.Twitter.Application.Interfaces.TweetRepository;
using Neal.Twitter.Infrastructure.Faster.Repository.Services.Repository;
using Neal.Twitter.Infrastructure.Kafka.Tweet.API.Endpoints;
using Serilog;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication
    .CreateBuilder(args);

// Attach Serilog logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

Log.Warning(ApplicationStatusMessages.Started);

builder
    .Services
    .AddSwaggerGen()
    .AddEndpointsApiExplorer()
    .AddProblemDetails()
    .AddMemoryCache()
    .AddScoped<ITweetRepository, FasterTweetRepository>(); // Single instance of Tweet Repository with persistence

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()
    || app.Environment.IsEnvironment("Docker"))
{
    app
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
    // app.UseHealthChecks(Path.Combine(""));
}

app
    .UseRouting()
    .UseEndpoints(configuration =>
        configuration.MapRepositoryEndpoints(typeof(Program).Assembly.GetName().Name ?? ApiStrings.BaseRoute));

await app.RunAsync();