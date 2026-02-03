using MiniSOC.Server.Endpoints;
using MiniSOC.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization for enums
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register EventStorageService as Singleton
builder.Services.AddSingleton<IEventStorageService, EventStorageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthEndpoints();
app.MapIngestEndpoints();

app.Run();

// Make Program accessible for integration tests
public partial class Program { }