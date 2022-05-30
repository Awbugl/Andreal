using Andreal.Core.Common;
using Andreal.Core.Data.Json.Arcaea.ArcaeaLimited;
using Andreal.Core.Data.Sqlite;

namespace Andreal.Core.Model.Arcaea;

[Serializable]
internal class PlayerInfo
{
    private readonly bool _isHide;

    private readonly string _playerCode, _playerName;

    internal PlayerInfo(UserinfoDataItem recentdata, BotUserInfo user)
    {
        _playerCode = user.ArcCode.ToString("D9");
        _playerName = recentdata.DisplayName;
        _isHide = user.IsHide == 0;
        ImgVersion = user.UiVersion;
        Partner = recentdata.Partner.PartnerID;
        IsAwakened = recentdata.Partner.IsAwakened;
        Potential = recentdata.Potential ?? -1;
    }

    public PlayerInfo(Data.Json.Arcaea.ArcaeaUnlimited.AccountInfo accountInfo, BotUserInfo user)
    {
        _playerCode = accountInfo.Code.ToString("D9");
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

    internal string PlayerCode =>
        _isHide
            ? _playerCode
            : "xxxxxxxxx";

    internal ImgVersion ImgVersion { get; }

    internal int Partner { get; }

    internal bool IsAwakened { get; }

    internal short Potential { get; }
}
