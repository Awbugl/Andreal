using System.Collections.Concurrent;
using Andreal.Utils;
using SQLite;

namespace Andreal.Data.Sqlite;

[Serializable]
[Table("BotUserInfo")]
internal class BotUserInfo
{
    private static Lazy<ConcurrentDictionary<long, BotUserInfo>> _list
        = new(() => new(SqliteHelper.SelectAll<BotUserInfo>().ToDictionary(i => i.QqId)));

    [PrimaryKey] [Column("QQId")] public long QqId { get; set; }
    [Column("ArcId")] public int ArcId { get; set; }
    [Column("PjskId")] public long PjskId { get; set; }
    [Column("IsHide")] public int IsHide { get; set; }
    [Column("IsText")] public int IsText { get; set; }
    [Column("ImgVer")] public ImgVersion UiVersion { get; set; }

    internal static void Set(BotUserInfo user)
    {
        if (_list.Value.ContainsKey(user.QqId))
        {
            _list.Value[user.QqId] = user;
            SqliteHelper.Update(user);
        }
        else
        {
            _list.Value.TryAdd(user.QqId, user);
            SqliteHelper.Insert(user);
        }
    }

    internal static BotUserInfo? Get(long qqid) =>
        _list.Value.TryGetValue(qqid, out var user)
            ? user
            : null;

    internal enum ImgVersion
    {
        ImgV1 = 0,
        ImgV2 = 1,
        ImgV3 = 2,
        ImgV4 = 3
    }
}
