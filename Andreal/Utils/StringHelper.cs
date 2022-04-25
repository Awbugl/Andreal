using System.Text.RegularExpressions;

namespace Andreal.Utils;

internal static class StringHelper
{
    private static readonly Regex Reg = new(@"\s|\(|\)|（|）", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    internal static bool Contains(string? raw, string? seed) =>
        seed != null && raw != null
                     && Reg.Replace(raw, "").Contains(Reg.Replace(seed, ""), StringComparison.OrdinalIgnoreCase);

    internal static bool Equals(string? raw, string? seed) =>
        seed != null && raw != null
                     && string.Equals(Reg.Replace(raw, ""), Reg.Replace(seed, ""), StringComparison.OrdinalIgnoreCase);
}
