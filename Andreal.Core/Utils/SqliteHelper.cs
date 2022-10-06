using SQLite;
using Path = Andreal.Core.Common.Path;

namespace Andreal.Core.Utils;

internal static class SqliteHelper
{
    private static readonly SQLiteConnection DbConnection;

    static SqliteHelper()
    {
        DbConnection = new(Path.Database,
                           SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache | SQLiteOpenFlags.FullMutex);

        DbConnection.Execute("CREATE TABLE IF NOT EXISTS [BotUserInfo]([QQId] INTEGER PRIMARY KEY NOT NULL, [ArcId] INTEGER DEFAULT(-1), [IsHide] INTEGER DEFAULT(0), [IsText] INTEGER DEFAULT(0), [ImgVer] INTEGER DEFAULT(0));");
    }

    internal static IEnumerable<T> SelectAll<T>() where T : new() => DbConnection.Table<T>();

    internal static void Insert<T>(T obj) where T : new() => DbConnection.Insert(obj);

    internal static void Update<T>(T obj) where T : new() => DbConnection.Update(obj);
}
