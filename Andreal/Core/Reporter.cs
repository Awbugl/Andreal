using Andreal.Utils;
using static Andreal.Utils.BotStatementHelper;

namespace Andreal.Core;

internal static class Reporter
{
    internal static void ExceptionReport(Exception? ex, long qqid = -1, string? raw = null)
    {
        try
        {
            if (ex is null) return;
            LastExceptionHelper.Set(ex, qqid);
            if (ex is HttpRequestException)
                ++WebExceptionCount;
            else
            {
                ++ExceptionCount;
                File.AppendAllText(Path.ExceptionReport,
                                   $"{DateTime.Now}\n{ex}\n来源：{qqid}{(raw is null ? "" : $"\t指令：{raw}")}\n\n");
            }
        }
        catch
        {
            Thread.Sleep(2000);
            ExceptionReport(ex, qqid, raw);
        }
    }
}
