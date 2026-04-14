namespace MiniSOC.Server.Services;

public interface IMetricsService
{
    int GetEventCount();
    Dictionary<string, int> GetEventsByLevel();
    Dictionary<string, int> GetEventsByHost();
}