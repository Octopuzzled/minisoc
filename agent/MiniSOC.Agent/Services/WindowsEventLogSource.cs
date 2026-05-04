using System.Diagnostics.Eventing.Reader;
using MiniSOC.Agent.Models;

namespace MiniSOC.Agent.Services;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class WindowsEventLogSource : IEventSource
{
    private readonly string[] _channels;
    private readonly string[] _levels;

    // Konstruktor — wird aufgerufen bei: new WindowsEventLogSource(channels, levels)
    public WindowsEventLogSource(string[] channels, string[] levels)
    {
        _channels = channels;
        _levels = levels;
    }

    public IEnumerable<Event> GetEvents()
    {
        Dictionary<string, int> levelDict = new Dictionary<string, int>{
            {"Critical", 1},
            {"Error", 2},
            {"Warning", 3},
            {"Information", 4}
        };
       Dictionary<int, MiniSOC.Agent.Models.EventLevel> windowsLevelDict = new Dictionary<int, MiniSOC.Agent.Models.EventLevel>
        {
            {1, MiniSOC.Agent.Models.EventLevel.Critical},
            {2, MiniSOC.Agent.Models.EventLevel.Error},
            {3, MiniSOC.Agent.Models.EventLevel.Warning},
            {4, MiniSOC.Agent.Models.EventLevel.Information}
        };
        var levelInts = _levels.Select(l => levelDict[l]).ToList();
        var levelFilter = $"*[System[{string.Join(" or ", levelInts.Select(l => $"Level={l}"))}]]";
        List<Event> events = new List<Event>();

        foreach (var channel in _channels)
        {
            var query = new EventLogQuery(channel, PathType.LogName, levelFilter);
            using var reader = new EventLogReader(query);
            EventRecord record;
            while ((record = reader.ReadEvent()) != null)
            {
                var timestamp = record.TimeCreated?.ToString("o") ?? DateTime.UtcNow.ToString("o");
                var host = record.MachineName;
                var provider = record.ProviderName;
                var logName = record.LogName;
                var level = record.Level.HasValue 
                    ? windowsLevelDict.GetValueOrDefault((int)record.Level.Value, MiniSOC.Agent.Models.EventLevel.Information)
                    : MiniSOC.Agent.Models.EventLevel.Information;
                var message = record.FormatDescription();

                var evt = new Event
                {
                    Timestamp = timestamp,
                    Host = host,
                    Source = "WindowsEventLog",
                    Channel = logName,
                    Provider = provider,
                    Level = level,
                    Message = message
                };
                events.Add(evt);
            }
        }
        return events;
    }
}