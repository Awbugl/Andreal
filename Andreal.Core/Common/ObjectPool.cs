using System.Collections.Concurrent;

namespace Andreal.Core.Common;

[Serializable]
internal class ObjectPool<T> where T : new()
{
    private readonly Lazy<ConcurrentBag<T>> _objects = new();


    public T Get() =>
        _objects.Value.TryTake(out var item)
            ? item
            : new();

    public void Return(T value) { _objects.Value.Add(value); }
}
