using System.Text.RegularExpressions;
using Andreal.Core;
using Andreal.Data.Api;
using Andreal.Data.Json.Pjsk;
using Andreal.Data.Json.Pjsk.PjskProfile;
using Andreal.Data.Sqlite;
using Andreal.Message;
using Andreal.UI;
using Andreal.UI.ImageGenerator;
using Andreal.Utils;
using Path = Andreal.Core.Path;

namespace Andreal.Executor;

#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8604

[Serializable]
internal class PjskExecutor : ExecutorBase
{
    public PjskExecutor(MessageInfo info) : base(info) { }

    [CommandPrefix("/pjsk bind", "绑定pjsk")]
    private async Task<MessageChain> Bind()
    {
        if (CommandLength != 1) return RobotReply.ParameterLengthError;
        if (!long.TryParse(Command[0], out var id)) return RobotReply.ParameterError;

        var pjskProfile = await PjskApi.PjskProfile(id);

        if (pjskProfile?.User is null) return RobotReply.PjskUserBindFailed;

        var user = User ?? new BotUserInfo { Uin = Info.FromQQ };

        user.PjskCode = pjskProfile.User.UserGamedata.UserID;

        BotUserInfo.Set(user);

        return $"{pjskProfile.User.UserGamedata.Name} ({pjskProfile.User.UserGamedata.Rank}) " + RobotReply.BindSuccess;
    }

    [CommandPrefix("/pjsk rand")]
    private MessageChain RandSong()
    {
        switch (CommandLength)
        {
            default: return RobotReply.ParameterLengthError;
            case 0:  return RobotReply.RandSongReply + Model.Pjsk.SongInfo.RandomSong().Songname;
            case 1:
            {
                if (!int.TryParse(Command[0], out var limit) || limit is < 5 or > 40) return RobotReply.ParameterError;

                var (info, dif) = Model.Pjsk.SongInfo.RandomSong(limit);

                return info == null
                    ? RobotReply.ParameterError
                    : RobotReply.RandSongReply + info.NameWithLevel(dif);
            }
            case 2:
            {
                if (!int.TryParse(Command[0], out var lower) || lower is < 5 or > 40) return RobotReply.ParameterError;

                if (!int.TryParse(Command[1], out var upper) || lower > upper || upper > 40)
                    return RobotReply.ParameterError;

                var (info, dif) = Model.Pjsk.SongInfo.RandomSong(lower, upper);
                return info == null
                    ? RobotReply.ParameterError
                    : RobotReply.RandSongReply + info.NameWithLevel(dif);
            }
        }
    }

    [CommandPrefix("/pjsk user")]
    private async Task<MessageChain> Profile()
    {
        if (User == null) return RobotReply.NotBind;
        if (User.PjskCode == 0) return RobotReply.NotBindPjsk;

        // 2020/09/16 10:00:00 (utc+9) so 9am in utc+8
        // and add (ID/1000/1024/4096) seconds
        // thanks to nilcric.

        var registerDate
            = new DateTime(2020, 9, 16, 9, 0, 0, DateTimeKind.Utc).AddSeconds((double)User.PjskCode / 0xFA000000);

        PjskCurrentEventItem? currentEvent = null;
        PjskProfiles? pjskProfile = null;

        await Task.WhenAll(Task.Run(() => pjskProfile = PjskApi.PjskProfile(User.PjskCode).Result),
                           Task.Run(() => currentEvent = PjskApi.PjskCurrentEvent().Result));

        var eventRankings = await PjskApi.PjskUserRanking(User.PjskCode, currentEvent.EventID);
        var userGamedata = pjskProfile.User.UserGamedata;

        return
            $"{userGamedata.Name}  ({userGamedata.Rank})\n\n注册时间 : {registerDate:yyyy/MM/dd hh:mm:ss}\n\n{Capture()}\n\n"
            + $"本期活动：{currentEvent.Name} \n" + (eventRankings == null
                ? ""
                : $"#{eventRankings.Rank}  ({eventRankings.Score}P)\n")
            + $"\n详细信息请使用浏览器查看：\nhttps://profile.pjsekai.moe/#/user/{User.PjskCode}";

        string Capture()
        {
            int maFc = 0, maAp = 0, exFc = 0, exAp = 0;
            var count = Model.Pjsk.SongInfo.Count();
            foreach (var item in pjskProfile.UserMusics.SelectMany(i => i.UserMusicDifficultyStatuses)
                                            .Where(i => i.MusicDifficulty is "master" or "expert"))
            {
                if (!item.UserMusicResults.Any()) continue;

                var ap = false;
                var fc = false;

                foreach (var resultsItem in item.UserMusicResults)
                {
                    ap = ap || resultsItem.FullPerfectFlg;
                    fc = fc || resultsItem.FullComboFlg;
                }

                switch (item.MusicDifficulty)
                {
                    case "master":
                        if (ap) maAp++;
                        if (fc) maFc++;
                        break;

                    case "expert":
                        if (ap) exAp++;
                        if (fc) exFc++;
                        break;
                }
            }

            return $"进度 (AP | FC | All)\nExpert : {exAp} | {exFc} | {count}\nMaster : {maAp} | {maFc} | {count}";
        }
    }

    [CommandPrefix("/pjsk event")]
    private async Task<MessageChain> Event()
    {
        PjskCurrentEventItem? currentEvent = null;
        Dictionary<int, int>? predict = null;

        await Task.WhenAll(Task.Run(async () => currentEvent = await PjskApi.PjskCurrentEvent()),
                           Task.Run(async () => predict = await PjskApi.PjskCurrentEventPredict()));

        var eventRankings = await PjskApi.PjskUserRanking(User.PjskCode, currentEvent.EventID);
        var generator = new PjskCurrentEventImageGenerator(currentEvent, eventRankings, predict);
        return User.IsText == 1
            ? generator.TextMessage
            : await generator.Generate();
    }

    [CommandPrefix("/pjsk ycm")]
    private static async Task<MessageChain> GetCars()
    {
        var response = await OtherApi.YcmApi("pjsk");
        if (response.Code == 404) return "myc";

        return response.Cars.Aggregate("现有车牌:",
                                       (curr, room) =>
                                           curr
                                           + $"\n\n{room.RoomID}   {room.AddTime.DateStringFromNow()}\n{Regex.Unescape(room.Description)}");
    }

    [CommandPrefix("/pjsk car")]
    private async Task<MessageChain> NewCar()
    {
        if (CommandLength < 2) return RobotReply.ParameterLengthError;
        if (Command[0].Length != 5 || int.TryParse(Command[0], out var id)) return RobotReply.ParameterError;
        if (id is 11451 or 14514) return "恶臭车牌(";
        var comment = string.Join("_", Command.Skip(1));

        if (!comment.Contains("大分") && !comment.Contains("自由") && !comment.Contains("q4") && !comment.Contains("q3")
            && !comment.Contains("q2") && !comment.Contains("q1") && !comment.Contains('m') && !comment.Contains("18w")
            && !comment.Contains("15w") && !comment.Contains("12w") && comment.Length < 4)
            return "描述信息过短将被视作无意义车牌，请添加更多描述。";

        var response = await OtherApi.AddCarApi("pjsk", Command[0], comment, User.Uin);

        return (response.Code switch
                {
                    0    => $"车牌 {Command[0]} 创建成功.",
                    1001 => "车牌格式不正确，请重试.",
                    1004 => $"车牌 {Command[0]} 已被创建，不能再次创建.",
                    1005 => "创建车牌次数过多，请两分钟后重试.",
                    _    => null
                })!;
    }

    [CommandPrefix("/pjsk info")]
    private MessageChain SongInfo()
    {
        if (CommandLength == 0) return RobotReply.ParameterLengthError;

        var (status, result) = PjskHelper.SongNameConverter(Command);
        return status == 0
            ? result[0].FullString()
            : PjskHelper.GetSongAliasErrorMessage(RobotReply, status, result);
    }

    [CommandPrefix("/pjsk chart")]
    private MessageChain Chart()
    {
        if (CommandLength == 0) return RobotReply.ParameterLengthError;
        var (status, result) = PjskHelper.SongNameConverter(Command);
        if (status != 0) return PjskHelper.GetSongAliasErrorMessage(RobotReply, status, result);
        var sid = result[0].SongID;
        return $"Expert谱面：\n{ChartImage(sid, "expert")}\nMaster谱面：\n{ChartImage(sid, "master")}";

        ImageMessage ChartImage(string songId, string dif)
        {
            var pth = Path.PjskChart(songId, dif);
            if (!pth.FileExists)
                Image.ImageExtend
                     .PngWithWhiteBg(WebHelper.DownloadImage($"https://minio.dnaroma.eu/sekai-assets/music/charts/{songId.PadLeft(4, '0')}/{dif}.png"),
                                     pth);
            return ImageMessage.FromPath(pth);
        }
    }

    [CommandPrefix("/pjsk alias")]
    private MessageChain Alias()
    {
        if (CommandLength == 0) return RobotReply.ParameterLengthError;

        var (status, result) = PjskHelper.SongNameConverter(Command);
        if (status != 0) return PjskHelper.GetSongAliasErrorMessage(RobotReply, status, result);
        var pjsksong = result[0];
        var alias = pjsksong.Alias.ToArray();

        return $"{pjsksong.Songname}\n" + (alias.Length > 0
            ? $"在数据库中的别名列表：\n{alias.Aggregate((i, j) => i + "\n" + j)}"
            : "该曲目暂无别名收录。");
    }
}
