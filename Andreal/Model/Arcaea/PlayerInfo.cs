using Andreal.Data.Json.Arcaea.ArcaeaLimited;
using Andreal.Data.Sqlite;

namespace Andreal.Model.Arcaea;

[Serializable]
internal class PlayerInfo
{
    private readonly bool _isHide;

    private readonly string _playerId, _playerName;

    internal PlayerInfo(UserinfoDataItem recentdata, BotUserInfo user)
    {
        _playerId = user.ArcId.ToString("D9");
        _playerName = recentdata.DisplayName;
        _isHide = user.IsHide == 0;
        ImgVersion = user.UiVersion;
        Partner = recentdata.Partner.PartnerId;
        IsAwakened = recentdata.Partner.IsAwakened;
        Potential = recentdata.Potential ?? -1;
    }

    public PlayerInfo(Data.Json.Arcaea.BotArcApi.AccountInfo accountInfo, BotUserInfo user)
    {
        _playerId = accountInfo.Code.ToString("D9");
        _playerName = accountInfo.Name;
        _isHide = user.IsHide == 0;
        ImgVersion = user.UiVersion;
        Partner = accountInfo.Character;
        IsAwakened = accountInfo.IsCharUncapped && !accountInfo.IsCharUncappedOverride;
        Potential = accountInfo.Rating;
    }

    internal string PlayerName =>
        _isHide
            ? _playerName
            : $"{_playerName[0]}......{_playerName[^1]}";

    internal string PlayerId =>
        _isHide
            ? _playerId
            : "xxxxxxxxx";

    internal BotUserInfo.ImgVersion ImgVersion { get; }

    internal int Partner { get; }

    internal bool IsAwakened { get; }

    internal short Potential { get; }
}
