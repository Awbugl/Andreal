using System.Timers;
using Timer = System.Timers.Timer;

namespace Andreal.Core.Common;

internal static class BackgroundTask
{
    internal static void Init()
    {
        Timer timer = new(240000);
        timer.Elapsed += Clean;
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    private static void Clean(object? source, ElapsedEventArgs e)
    {
        var time = DateTime.Now.AddMinutes(-2);

        foreach (var j in new DirectoryInfo(Path.TempImageRoot).GetFiles().Where(j => time > j.LastWriteTime))
            j.Delete();
    }
}
