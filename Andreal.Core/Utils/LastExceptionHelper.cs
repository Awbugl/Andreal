namespace Andreal.Core.Utils;

internal static class LastExceptionHelper
{
    private static (DateTime Time, Exception Exception) _lastException = (DateTime.Now, new ArgumentNullException());

    internal static string GetDetails() => _lastException.Exception.ToString();

    internal static string Get() => $"{_lastException.Item1}\n {_lastException.Exception.GetType().FullName}";

    internal static void Set(Exception ex) => _lastException = (DateTime.Now, ex);
}
