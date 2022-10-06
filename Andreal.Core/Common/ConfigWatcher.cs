namespace Andreal.Core.Common;

internal static class ConfigWatcher
{
    private const int TimeoutMillis = 2000;
    private static readonly FileSystemWatcher Watcher = new(Path.AndreaConfigRoot);
    private static readonly List<string> TmpFiles = new();

    internal static void Init()
    {
        Watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite;
        Watcher.Filter = "*.json";
        Watcher.EnableRaisingEvents = true;

        Timer timer = new(OnTimer, null, Timeout.Infinite, Timeout.Infinite);
        Watcher.Changed += (_, e) =>
        {
            TmpFiles.Add(e.Name!);
            timer.Change(TimeoutMillis, Timeout.Infinite);
        };
    }

    private static void OnTimer(object? _)
    {
        lock (TmpFiles)
        {
            foreach (var file in TmpFiles) GlobalConfig.Init(file);
            TmpFiles.Clear();
        }
    }
}
