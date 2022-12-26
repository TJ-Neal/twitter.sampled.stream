var builder = WebApplication
    .CreateBuilder(args);

// More options - startup and urls

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // app.UseHealthChecks(Path.Combine(""));
}

await app.RunAsync();