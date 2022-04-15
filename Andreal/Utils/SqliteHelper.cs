using SQLite;
using Path = Andreal.Core.Path;

namespace Andreal.Utils;

internal static class SqliteHelper
{
    private static readonly Lazy<SQLiteConnection> DbConnection
        = new(() => new(Path.Database, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex));

    internal static IEnumerable<T> SelectAll<T>() where T : new() => DbConnection.Value.Table<T>();

    internal static void Insert<T>(T obj) where T : new() { DbConnection.Value.Insert(obj); }

    internal static void Update<T>(T obj) where T : new() { DbConnection.Value.Update(obj); }
}
