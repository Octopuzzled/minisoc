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
builder.Services.AddSingleton<IEventStorageService, EventStorageService>();
builder.Services.AddSingleton<IDatabaseService, SqliteDatabaseService>();

var app = builder.Build();

// Initialize db
using (var scope = app.Services.CreateScope())
{
    var dbService = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
    dbService.Initialize();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapHealthEndpoints();
app.MapIngestEndpoints();
app.MapEventsEndpoints();

app.Run();

// Make Program accessible for integration tests
public partial class Program { }