using System.Diagnostics.Eventing.Reader;
using System.Text.Json;
using MiniSOC.Agent.Models;

namespace MiniSOC.Agent.Services;

[System.Runtime.Versioning.SupportedOSPlatform("windows")]
public class WindowsEventLogSource : IEventSource
{
    private readonly string[] _channels;
    private readonly string[] _levels;
    private readonly string? _bookmarkPath;

    // Konstruktor — wird aufgerufen bei: new WindowsEventLogSource(channels, levels)
    public WindowsEventLogSource(string[] channels, string[] levels, string? bookmarkPath = null)
    {
        _channels = channels;
        _levels = levels;
        _bookmarkPath = bookmarkPath;
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

        Dictionary<string, string> bookmarks = new Dictionary<string, string>();
        if (_bookmarkPath != null && File.Exists(_bookmarkPath))
        {
            var json = File.ReadAllText(_bookmarkPath);
            bookmarks = JsonSerializer.Deserialize<Dictionary<string,string>>(json) ?? new Dictionary<string, string>();
        }

        foreach (var channel in _channels)
        {
            var query = new EventLogQuery(channel, PathType.LogName, levelFilter);
            EventBookmark? bookmark = null;
            if (bookmarks.TryGetValue(channel, out var bookmarkXml))
            {
                bookmark = new EventBookmark(bookmarkXml);
            }
            using var reader = new EventLogReader(query, bookmark);
            EventRecord record;
            #pragma warning disable CA1416
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
                if (record.Bookmark != null)
                    bookmarks[channel] = record.Bookmark.BookmarkXml;
            }
        }
        if (_bookmarkPath != null)
        {
            File.WriteAllText(_bookmarkPath, JsonSerializer.Serialize(bookmarks));
        }
        return events;
    }
}