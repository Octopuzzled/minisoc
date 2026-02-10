using MiniSOC.Agent.Clients;
using MiniSOC.Agent.Services;

// ============================================
// MiniSOC Agent - Event Collection & Transmission
// ============================================

Console.WriteLine("MiniSOC Agent starting...\n");

// 1. Create event source (dummy for development)
var source = new DummyEventSource();
Console.WriteLine("✓ Event source initialized (DummyEventSource)");

// 2. Collect events
var events = source.GetEvents();
var eventList = events.ToList();
Console.WriteLine($"✓ Collected {eventList.Count} event(s)\n");

// 3. Create sender (HTTP to server)
var sender = new HttpEventSender();
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