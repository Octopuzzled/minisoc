namespace MiniSOC.Server.Services;

public interface IMetricsService
{
    int GetEventCount();
    Dictionary<string, int> GetEventsByLevel();
    Dictionary<string, int> GetEventsByHost();

    List<TrendBucket> GetEventsLast24h();
    List<TrendBucket> GetEventsLast7d();
}