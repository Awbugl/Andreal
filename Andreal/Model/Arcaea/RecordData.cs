using Andreal.Data.Api;
using Andreal.Data.Json.Arcaea.ArcaeaUnlimited;
using Andreal.Data.Sqlite;
using Andreal.Message;
using Andreal.UI.ImageGenerator;

namespace Andreal.Model.Arcaea;

[Serializable]
internal class RecordData
{
    internal RecordData(PlayerInfo playerInfo, RecordInfo recordInfo, BotUserInfo userInfo)
    {
        PlayerInfo = playerInfo;
        RecordInfo = recordInfo;
        UserInfo = userInfo;
    }

    private PlayerInfo PlayerInfo { get; }
    private RecordInfo RecordInfo { get; }
    private BotUserInfo UserInfo { get; }

    private TextMessage RecordTextResult =>
        $"{PlayerInfo.PlayerName}({PlayerInfo.PlayerCode}) 的最近记录\n曲名：{RecordInfo.SongName(50)} {RecordInfo.SongInfo.ConstString}\n分数：{RecordInfo.Score}\nPure:{RecordInfo.Pure} (+{RecordInfo.MaxPure})  Far:{RecordInfo.Far}  Lost:{RecordInfo.Lost}\n单曲PTT：{RecordInfo.Rating}\n时间：{RecordInfo.TimeStr}";

    internal async Task<MessageChain> GetResult()
    {
        if (UserInfo.IsText == 1) return RecordTextResult;
        var imageGenerator = new ArcRecordImageGenerator(PlayerInfo, RecordInfo);

        switch (UserInfo.UiVersion)
        {
            case BotUserInfo.ImgVersion.ImgV1: return await imageGenerator.Version1();
            case BotUserInfo.ImgVersion.ImgV3: return await imageGenerator.Version3();
            case BotUserInfo.ImgVersion.ImgV4:
            {
                var b30data = await ArcaeaUnlimitedApi.UserBest30(UserInfo.ArcId);
                return b30data!.Status != 0
                    ? await imageGenerator.Version3()
                    : await imageGenerator.Version4(new Best30Data(b30data.DeserializeContent<UserBestsContent>()));
            }
            default: return await imageGenerator.Version2();
        }
    }
}
