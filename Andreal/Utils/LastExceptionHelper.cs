namespace Andreal.Utils;

internal static class LastExceptionHelper
{
    private static (DateTime, Exception, long) _lastException = (DateTime.Now, new ArgumentNullException(), -1);

    internal static string GetDetails() => _lastException.Item2 + "\n\n来源：" + _lastException.Item3;

    internal static string Get() => $"{_lastException.Item1}\n {_lastException.Item2.GetType().FullName}";

    internal static void Set(Exception ex, long fromQq = -1) { _lastException = (DateTime.Now, ex, fromQq); }
}
