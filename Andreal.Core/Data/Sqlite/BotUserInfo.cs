using System.Collections.Concurrent;
using Andreal.Core.Common;
using Andreal.Core.Utils;
using SQLite;

namespace Andreal.Core.Data.Sqlite;

[Serializable]
[Table("BotUserInfo")]
internal class BotUserInfo
{
    private static Lazy<ConcurrentDictionary<long, BotUserInfo>> _list
        = new(() => new(SqliteHelper.SelectAll<BotUserInfo>().ToDictionary(i => i.Uin)));

    [PrimaryKey] [Column("QQId")] public long Uin { get; set; }
    [Column("ArcId")] public int ArcCode { get; set; }
    [Column("PjskId")] public long PjskCode { get; set; }
    [Column("IsHide")] public int IsHide { get; set; }
    [Column("IsText")] public int IsText { get; set; }
    [Column("ImgVer")] public ImgVersion UiVersion { get; set; }

    internal static void Set(BotUserInfo user)
    {
        if (_list.Value.ContainsKey(user.Uin))
        {
            _list.Value[user.Uin] = user;
            SqliteHelper.Update(user);
        }
        else
        {
            _list.Value.TryAdd(user.Uin, user);
            SqliteHelper.Insert(user);
        }
    }

    internal static BotUserInfo? Get(long uin) =>
        _list.Value.TryGetValue(uin, out var user)
            ? user
            : null;
}
