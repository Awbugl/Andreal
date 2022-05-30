using System.Text;

namespace Andreal.Core.Utils;

internal static class RandStringHelper
{
    private const string Chars = "0123456789abcdefghijklmnopqrstuvwxyz";
    private static readonly Random Random = new();

    public static string GetRandString(int length = 10)
    {
        var res = new StringBuilder();
        for (var i = 0; i < length; i++) res.Append(Chars[Random.Next(36)]);
        return res.ToString();
    }
}
