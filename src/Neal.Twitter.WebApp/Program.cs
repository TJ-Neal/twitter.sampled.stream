using Neal.Twitter.Application.Constants.Messages;
using Serilog;

var builder = WebApplication
    .CreateBuilder(args);

// Attach Serilog logger
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

Log.Information(ApplicationStatusMessages.Started);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// app.UseAuthorization(); /* Sample application that does not expose any sensitive or proprietary data. */

app.MapRazorPages();

app.Run();