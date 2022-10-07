using Andreal.Core.Utils;
using static Andreal.Core.Utils.BotStatementHelper;

namespace Andreal.Core.Common;

public static class ExceptionLogger
{
    public delegate void ExceptionEventHandler(Exception e);

    public static event ExceptionEventHandler? OnExceptionRecorded;

    public static void Log(Exception? ex)
    {
        if (ex is null) return;
        LastExceptionHelper.Set(ex);
        
        switch (ex)
        {
            case HttpRequestException or TaskCanceledException:
                ++WebExceptionCount;
                return;

            case InvalidCastException when ex.Message.Contains("Konata.Core.Events.ProtocolEvent"):
                return;

            default:
                ++ExceptionCount;
                WriteException(ex);
                OnExceptionRecorded?.Invoke(ex);
                return;
        }
    }

    private static void WriteException(Exception ex)
    {
        try
        {
            File.AppendAllText(Path.ExceptionReport, $"{DateTime.Now}\n{ex}\n\n");
        }
        catch
        {
            Thread.Sleep(2000);
            WriteException(ex);
        }
    }
}
