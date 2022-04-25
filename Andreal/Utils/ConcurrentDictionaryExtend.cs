using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Andreal.Utils;

internal static class ConcurrentDictionaryExtend
{
    public static bool TryAddOrInsert<TKey, TValue, TList>(this ConcurrentDictionary<TKey, TList> dict, TKey key,
                                                           TValue value) where TList : List<TValue>, new()
                                                                         where TKey : notnull
    {
        if (!dict.ContainsKey(key)) return dict.TryAdd(key, new() { value });

        dict[key].Add(value);
        return true;
    }

    public static void ForAllItems<TKey, TValue, TList>(this ConcurrentDictionary<TKey, TList> dict,
                                                        Action<TKey, TValue> action)
        where TList : List<TValue>, new() where TKey : notnull
    {
        foreach (var (key, values) in dict)
            foreach (var value in values)
                action(key, value);
    }

    public static void ForAllItems<TKey, TValue, TList>(this ConcurrentDictionary<TKey, TList> dict,
                                                        Action<TKey> keyaction, Action<TKey, TValue> kvaction)
        where TList : List<TValue>, new() where TKey : notnull
    {
        foreach (var (key, values) in dict)
        {
            keyaction(key);
            foreach (var value in values) kvaction(key, value);
        }
    }

    public static bool TryTakeKey<TKey, TValue, TList>(this ConcurrentDictionary<TKey, TList> dict,
                                                       Func<TValue, bool> action,
                                                       [MaybeNullWhen(false)] out TKey result)
        where TList : List<TValue>, new() where TKey : notnull
    {
        foreach (var (key, values) in dict)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var value in values)
                if (action(value))
                {
                    result = key;
                    return true;
                }
        }

        result = default;
        return false;
    }

    public static bool TryTakeValues<TKey, TValue, TList>(this ConcurrentDictionary<TKey, TList> dict,
                                                          Func<TValue, bool> action,
                                                          [MaybeNullWhen(false)] out TList result)
        where TList : List<TValue>, new() where TKey : notnull
    {
        // ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (var (_, values) in dict)
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var value in values)
                if (action(value))
                {
                    result = values;
                    return true;
                }
        }

        result = default;
        return false;
    }

    public static bool TryTakeValue<TKey, TValue, TList>(this ConcurrentDictionary<TKey, TList> dict,
                                                         Func<TValue, bool> action,
                                                         [MaybeNullWhen(false)] out TValue result)
        where TList : List<TValue>, new() where TKey : notnull
    {
        // ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (var (_, values) in dict)
        {
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var value in values)
                if (action(value))
                {
                    result = value;
                    return true;
                }
        }

        result = default;
        return false;
    }
}
