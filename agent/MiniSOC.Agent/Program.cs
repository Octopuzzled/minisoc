using Microsoft.Extensions.Configuration;
using MiniSOC.Agent.Clients;
using MiniSOC.Agent.Services;

// ============================================
// MiniSOC Agent - Event Collection & Transmission
// ============================================

Console.WriteLine("MiniSOC Agent starting...\n");

// Load configuration from appsettings.json
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

// Read agent settings with fallback defaults
var serverUrl = config["Agent:ServerUrl"] ?? "http://localhost:5152";
int.TryParse(config["Agent:IntervalSeconds"], out int intervalSeconds);
if (intervalSeconds == 0) intervalSeconds = 30;
var channels = config.GetSection("Agent:Channels").Get<string[]>();
var levels = config.GetSection("Agent:Levels").Get<string[]>();

// 1. Create event source (dummy for development; replace with WindowsEventLogSource for production)
var source = new DummyEventSource();
Console.WriteLine("✓ Event source initialized (DummyEventSource)");

// 2. Collect events
var events = source.GetEvents();
var eventList = events.ToList();
Console.WriteLine($"✓ Collected {eventList.Count} event(s)\n");

// 3. Create sender using configured server URL
var sender = new HttpEventSender(serverUrl);
Console.WriteLine("✓ Event sender initialized (HTTP)\n");

// 4. Send events to server
var success = await sender.SendEventsAsync(eventList);

// 5. Report result
Console.WriteLine();
if (success)
{
    Console.WriteLine("========================================");
    Console.WriteLine("✓ Agent completed successfully");
    Console.WriteLine("========================================");
}
else
{
    Console.WriteLine("========================================");
    Console.WriteLine("✗ Agent completed with errors");
    Console.WriteLine("========================================");
    Environment.Exit(1); // Exit with error code
}