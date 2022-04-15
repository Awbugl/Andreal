namespace Andreal.Utils;

internal static class RandomHelper
{
    private static readonly Random Random = new();

    internal static T GetRandomItem<T>(this T[] ls) => ls[Random.Next(ls.Length)];

    internal static T GetRandomItem<T>(this IEnumerable<T> ls) => ls.ToArray().GetRandomItem();
}
