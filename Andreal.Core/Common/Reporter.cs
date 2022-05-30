using Andreal.Core.Utils;
using static Andreal.Core.Utils.BotStatementHelper;

namespace Andreal.Core.Common;

public static class Reporter
{
    public delegate void ExceptionEventHandler(Exception e);

    public static event ExceptionEventHandler? OnExceptionRecorded;

    public static void ExceptionReport(Exception? ex)
    {
        try
        {
            if (ex is null) return;
            LastExceptionHelper.Set(ex);
            if (ex is HttpRequestException or TaskCanceledException)
                ++WebExceptionCount;
            else
            {
                ++ExceptionCount;
                File.AppendAllText(Path.ExceptionReport, $"{DateTime.Now}\n{ex}\n\n");
                OnExceptionRecorded?.Invoke(ex);
            }
        }
        catch
        {
            Thread.Sleep(2000);
            ExceptionReport(ex);
        }
    }
}
