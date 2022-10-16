using System.Text.RegularExpressions;
using Andreal.Core.Common;
using Andreal.Core.Data.Api;
using Andreal.Core.Data.Json.Arcaea.ArcaeaUnlimited;
using Andreal.Core.Data.Sqlite;
using Andreal.Core.Message;
using Andreal.Core.Model.Arcaea;
using Andreal.Core.UI.ImageGenerator;
using Andreal.Core.Utils;
using Path = Andreal.Core.Common.Path;

namespace Andreal.Core.Executor;

#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8604

[Serializable]
internal class ArcExecutor : ExecutorBase
{
    private static int[] _difs = { 2, 3, 1, 0 };
    public ArcExecutor(MessageInfo info) : base(info) { }

    private (RecordInfo?, PlayerInfo?, TextMessage?) GetValue(ResponseRoot data)
    {
        if (data.Status == 0)
        {
            var content = data.DeserializeContent<UserBestContent>();
            return (new(content.Record), new(content.AccountInfo, User), null);
        }

        return (null, null, ArcaeaUnlimitedApi.GetErrorMessage(RobotReply, data.Status, data.Message));
    }

    private async Task<(RecordInfo?, PlayerInfo?, TextMessage?)> GetUserBest(ArcaeaSong song, int dif)
    {
        if (dif == -1)
        {
            foreach (var i in _difs)
            {
                if (i == 3 && song.Count != 4) continue;

                var data = await ArcaeaUnlimitedApi.UserBest(User!.ArcCode, song.SongID, i);

                if (data.Status == -15) continue;

                return GetValue(data);
            }

            return (null, null, RobotReply.NotPlayedTheSong);
        }

        {
            var data = await ArcaeaUnlimitedApi.UserBest(User!.ArcCode, song.SongID, dif);

            return GetValue(data);
        }
    }

    [CommandPrefix("/arc download")]
    private static MessageChain Download() => "https://dl.arcaea.moe/";

    [CommandPrefix("/arc bind", "绑定arc")]
    private async Task<MessageChain> Bind()
    {
        if (CommandLength != 1) return RobotReply.ParameterLengthError;

        ResponseRoot data;
        if (long.TryParse(Command[0], out var uid))
        {
            if (uid < 3)
            {
                var unbinduser = User ?? new BotUserInfo { Uin = Info.FromQQ, UiVersion = (ImgVersion)new Random().Next(3) };

                unbinduser.ArcCode = 0;

                BotUserInfo.Set(unbinduser);
                return RobotReply.UnBindSuccess;
            }

            var info = await ArcaeaUnlimitedApi.UserInfo(uid);
            data = info.Status == 0 ? info : await ArcaeaUnlimitedApi.UserInfo(Command[0]);
        }
        else
        {
            data = await ArcaeaUnlimitedApi.UserInfo(Command[0]);
        }

        if (data.Status != 0) return ArcaeaUnlimitedApi.GetErrorMessage(RobotReply, data.Status, data.Message);

        var content = data.DeserializeContent<UserInfoContent>();

        var user = User ?? new BotUserInfo { Uin = Info.FromQQ, UiVersion = (ImgVersion)new Random().Next(3) };

        user.ArcCode = content.AccountInfo.Code;

        BotUserInfo.Set(user);

        return
            RobotReply.OnBindSuccess($"{content.AccountInfo.Name} ({(content.AccountInfo.Rating == -1 ? "--" : ((double)content.AccountInfo.Rating / 100).ToString("0.00"))})");
    }

    [CommandPrefix("/arc ycm")]
    private static async Task<MessageChain> GetCars()
    {
        var response = await OtherApi.YcmApi("arc");
        if (response.Code == 404) return "myc";

        return response.Cars.Aggregate("现有车牌:",
                                       (curr, room) => curr +
                                                       $"\n\n{room.RoomID}   {room.AddTime.DateStringFromNow()}\n{Regex.Unescape(room.Description)}");
    }

    [CommandPrefix("/arc car")]
    private async Task<MessageChain> NewCar()
    {
        if (CommandLength < 2) return RobotReply.ParameterLengthError;
        if (Command[0].Length != 6) return RobotReply.ParameterError;
        var comment = string.Join("_", Command.Skip(1));

        if (comment.Length < 4) return "描述信息过短将被视作无意义车牌，请添加更多描述。";

        var response = await OtherApi.AddCarApi("arc", Command[0], comment, User.Uin);

        return (response.Code switch
                {
                    0    => $"车牌 {Command[0]} 创建成功.",
                    1001 => "车牌格式不正确，请重试.",
                    1004 => $"车牌 {Command[0]} 已被创建，不能再次创建.",
                    1005 => "创建车牌次数过多，请两分钟后重试.",
                    _    => null
                })!;
    }

    [CommandPrefix("/arc rand", "随机选曲", "选曲")]
    private async Task<MessageChain> RandSong()
    {
        switch (CommandLength)
        {
            default:
                return RobotReply.ParameterLengthError;

            case 0:
                return RobotReply.RandSongReply + ArcaeaCharts.RandomSong().NameWithPackage;

            case 1:
            {
                var (lower, upper) = Command[0].ConvertToArcaeaRange();
                if (lower < 0) return RobotReply.ParameterError;
                var info = ArcaeaCharts.RandomSong(lower, upper);
                if (info == null) return RobotReply.ParameterError;

                var chain = new MessageChain();
                chain.Append(RobotReply.RandSongReply);
                chain.Append(ImageMessage.FromPath(await Path.ArcaeaSong(info)));
                chain.Append(info.NameWithPackageAndConst);
                return chain;
            }

            case 2:
            {
                var (lower, _) = Command[0].ConvertToArcaeaRange();
                var (_, upper) = Command[1].ConvertToArcaeaRange();
                if (lower < 0 || upper < 0 || lower > upper) return RobotReply.ParameterError;
                var info = ArcaeaCharts.RandomSong(lower, upper);
                if (info == null) return RobotReply.ParameterError;

                var chain = new MessageChain();
                chain.Append(RobotReply.RandSongReply);
                chain.Append(ImageMessage.FromPath(await Path.ArcaeaSong(info)));
                chain.Append(info.NameWithPackageAndConst);
                return chain;
            }
        }
    }

    [CommandPrefix("/arc note", "计算note")]
    private MessageChain CalcNote()
    {
        if (CommandLength < 3) return RobotReply.ParameterLengthError;
        if (!short.TryParse(Command[0], out var far) || !short.TryParse(Command[1], out var lost)) return RobotReply.ParameterError;

        if (!ArcaeaHelper.SongInfoParser(Command.Skip(2), out var song, out var dif, out var errMessage)) return errMessage;

        if (dif == -1) dif = 2;

        if (dif == 3 && song.Count < 4) return RobotReply.NoBydChart;

        var arcsong = song[dif];

        var notes = arcsong.Note;

        if (arcsong.Const <= 0 || notes <= 0) return "此谱面的Note数量暂未被记录。";

        if (far + lost > notes) return RobotReply.ParameterError;

        var perNoteScore = 1e7 / notes;

        return
            $"{arcsong.NameWithPackageAndConst}\n(+{notes})\n在 {far} Far {lost} Lost 时\n理论最高分数约为 {1e7 + notes - far - lost - (int)(perNoteScore * (lost + (double)far / 2))}";
    }

    [CommandPrefix("/arc score", "计算分数")]
    private MessageChain CalcScore()
    {
        if (CommandLength < 2) return RobotReply.ParameterLengthError;

        if (!double.TryParse(Command[0], out var ptt)) return RobotReply.ParameterError;

        if (!ArcaeaHelper.SongInfoParser(Command.Skip(1), out var song, out var dif, out var errMessage)) return errMessage;

        if (dif == -1) dif = 2;

        if (dif == 3 && song.Count < 4) return RobotReply.NoBydChart;

        var arcsong = song[dif];

        var defNum = arcsong.Const;
        var mptt = ptt - defNum;
        if (defNum == 0 || ptt < 0 || mptt > 2) return RobotReply.ParameterError;

        long score;
        if (Math.Abs(mptt - 2) < 1e-4) return "PureMemery.";
        if (mptt >= 1)
            score = (long)(9.8e6 + (mptt - 1) * 2e5);
        else
            score = (long)(9.5e6 + mptt * 3e5);

        return $"{arcsong.NameWithPackageAndConst}\n在Ptt为 {Command[0]} 时\n分数为 {score}";
    }

    [CommandPrefix("/arc ptt", "计算ptt")]
    private MessageChain CalcPtt()
    {
        if (CommandLength < 2) return RobotReply.ParameterLengthError;

        if (!double.TryParse(Command[0], out var score) || score is < 0 or > 10009999) return RobotReply.ParameterError;

        if (!ArcaeaHelper.SongInfoParser(Command.Skip(1), out var song, out var dif, out var errMessage)) return errMessage;

        if (dif == -1) dif = 2;

        if (dif == 3 && song.Count < 4) return RobotReply.NoBydChart;

        var arcsong = song[dif];

        var defNum = arcsong.Const;
        var ptt = score switch
                  {
                      >= 1e7   => defNum + 2,
                      >= 9.8e6 => defNum + 1 + (score - 9.8e6) / 2e5,
                      _        => defNum + (score - 9.5e6) / 3e5 > 0 ? defNum + (score - 9.5e6) / 3e5 : 0
                  };

        return $"{arcsong.NameWithPackageAndConst}\n在分数为 {Command[0]} 时\nPtt为 {ptt:0.0000}";
    }

    [CommandPrefix("/arc const", "查定数")]
    private async Task<MessageChain> Const()
    {
        if (CommandLength == 0) return RobotReply.ParameterLengthError;

        if (!ArcaeaHelper.SongInfoParser(Command, out var song, out _, out var errMessage)) return errMessage;

        if (song.SongID is "last" or "lasteternity")
        {
            var lastsong = ArcaeaCharts.QueryByID("last");
            var lasteternitysong = ArcaeaCharts.QueryByID("lasteternity");
            var msg = new MessageChain();

            for (var i = 0; i < lastsong.Count; i++)
                if (i == 2 || lastsong[i].JacketOverride)
                    msg.Append(ImageMessage.FromPath(await Path.ArcaeaSong(lastsong[i])));

            msg.Append(ImageMessage.FromPath(await Path.ArcaeaSong(lasteternitysong[3])));

            msg.Append(lastsong.NameWithPackage);

            foreach (var t in lastsong)
            {
                msg.Append("\n" + t.ConstString);
                if (t.AudioOverride) msg.Append($"  ({t.NameEn})");
            }

            msg.Append("\n" + lasteternitysong[3].ConstString);
            msg.Append($"  ({lasteternitysong[3].NameEn})");

            return msg;
        }

        return await song.FullConstString();
    }

    [CommandPrefix("/arc b30", "/arc r10", "/b30", "查b30")]
    private async Task<MessageChain> Best30()
    {
        if (User == null) return RobotReply.NotBind;
        if (User.ArcCode < 2) return RobotReply.NotBindArc;


        IBest30Data b30data;
        PlayerInfo playerInfo;

        if (Command.Length > 0 && Command[0] == "official")
        {
            if (!ArcaeaLimitedApi.Available) return null!;
            await Info.SendMessage(RobotReply.BestsQuerying);

            playerInfo = new((await ArcaeaLimitedApi.Userinfo(User.ArcCode))!, User);
            b30data = new LimitedBest30Data((await ArcaeaLimitedApi.Userbest30(User.ArcCode))!, playerInfo.Potential);
        }
        else
        {
            await Info.SendMessage(RobotReply.BestsQuerying);
            var data = await ArcaeaUnlimitedApi.UserBest30(User.ArcCode);
            if (data.Status != 0) return ArcaeaUnlimitedApi.GetErrorMessage(RobotReply, data.Status, data.Message);
            var content = data.DeserializeContent<UserBestsContent>();
            b30data = new Best30Data(content);
            playerInfo = new(content.AccountInfo, User);
        }

        return await new ArcBest30ImageGenerator(b30data, playerInfo).Generate();
    }

    [CommandPrefix("/arc b40", "查b40")]
    private async Task<MessageChain> Best40()
    {
        if (User == null) return RobotReply.NotBind;
        if (User.ArcCode < 2) return RobotReply.NotBindArc;

        await Info.SendMessage(RobotReply.BestsQuerying);
        var data = await ArcaeaUnlimitedApi.UserBest40(User.ArcCode);
        if (data.Status != 0) return ArcaeaUnlimitedApi.GetErrorMessage(RobotReply, data.Status, data.Message);

        var content = data.DeserializeContent<UserBestsContent>();
        IBest40Data b40data = new Best40Data(content);
        PlayerInfo playerInfo = new(content.AccountInfo, User);

        return await new ArcBest40ImageGenerator(b40data, playerInfo).Generate();
    }

    [CommandPrefix("/arc floor", "查地板")]
    private Task<MessageChain> Floor() => FloorOrCeil(true);

    [CommandPrefix("/arc ceil", "查天花板")]
    private Task<MessageChain> Ceil() => FloorOrCeil(false);

    private async Task<MessageChain> FloorOrCeil(bool isFloor)
    {
        if (User == null) return RobotReply.NotBind;
        if (User.ArcCode < 2) return RobotReply.NotBindArc;

        var data = await ArcaeaUnlimitedApi.UserBest30(User.ArcCode);
        if (data.Status != 0) return ArcaeaUnlimitedApi.GetErrorMessage(RobotReply, data.Status, data.Message);
        var content = data.DeserializeContent<UserBestsContent>();
        IBest30Data b30data = new Best30Data(content);

        return await new ArcRecord5ImageGenerator(b30data, isFloor).Generate();
    }

    private async Task<MessageChain> Recent()
    {
        var data = await ArcaeaUnlimitedApi.UserInfo(User.ArcCode);
        if (data.Status != 0) return ArcaeaUnlimitedApi.GetErrorMessage(RobotReply, data.Status, data.Message);
        var content = data.DeserializeContent<UserInfoContent>();

        if (content.RecentScore.Count == 0) return RobotReply.NotPlayedTheSong;
        RecordInfo recordInfo = new(content.RecentScore[0]);
        PlayerInfo playerInfo = new(content.AccountInfo, User);

        return await new RecordData(playerInfo, recordInfo, User).GetResult();
    }

    [CommandPrefix("/arc info", "查分 ", "查 ")]
    private async Task<MessageChain> Search()
    {
        if (User == null) return RobotReply.NotBind;
        if (User.ArcCode < 2) return RobotReply.NotBindArc;

        if (CommandLength == 0) return await Recent();
        if (Command[0] == "b30") return await Best30();

        if (!ArcaeaHelper.SongInfoParser(Command, out var song, out var dif, out var errMessage)) return errMessage;

        var (recordInfo, playerInfo, exceptionInformation) = await GetUserBest(song, dif);
        if (recordInfo == null) return exceptionInformation;

        return await new RecordData(playerInfo, recordInfo, User).GetResult();
    }

    [CommandPrefix("/arc level")]
    private async Task<MessageChain> Level()
    {
        if (CommandLength != 1) return RobotReply.ParameterLengthError;
        if (!double.TryParse(Command[0], out var num) || num > 12) return RobotReply.ParameterError;
        if (num < 8) return (TextMessage)"暂不支持查询8.0以下的定数。";
        ArcaeaChart[] list = ArcaeaCharts.GetByConst(num).OrderBy(i => i.NameEn).ToArray();
        if (list.Length == 0) return (TextMessage)"此定数在当前版本不存在对应谱面。";
        return await new ArcSongLevelListImageGenerator(list).Generate();
    }

    [CommandPrefix("/arc ui")]
    private MessageChain UiConfig()
    {
        if (CommandLength != 1) return RobotReply.ParameterLengthError;

        if (User == null) return RobotReply.NotBind;

        if (!int.TryParse(Command[0], out var version)) return RobotReply.ParameterError;

        (User.UiVersion, string imgversion) = version switch
                                              {
                                                  2 => (ImgVersion.ImgV2, "ImgV2"),
                                                  3 => (ImgVersion.ImgV3, "ImgV3"),
                                                  4 => (ImgVersion.ImgV4, "ImgV4"),
                                                  _ => (ImgVersion.ImgV1, "ImgV1")
                                              };
        BotUserInfo.Set(User);
        return $"Arc查分显示样式已更改为{imgversion}。";
    }

    [CommandPrefix("/arc alias", "查别名")]
    private MessageChain Alias()
    {
        if (CommandLength == 0) return RobotReply.ParameterLengthError;

        if (!ArcaeaHelper.SongInfoParser(Command, out var song, out _, out var errMessage)) return errMessage;

        List<string> alias = ArcaeaCharts.GetSongAlias(song.SongID);

        return alias.Count > 0
                   ? $"{song.NameWithPackage}\n在数据库中的别名列表：\n{alias.Aggregate((i, j) => i + "\n" + j)}"
                   : $"{song.NameWithPackage}\n该曲目暂无别名收录。";
    }

    [CommandPrefix("/arc chart", "查谱面")]
    private async Task<MessageChain> Chart()
    {
        if (CommandLength == 0) return RobotReply.ParameterLengthError;

        if (!ArcaeaHelper.SongInfoParser(Command, out var song, out var dif, out var errMessage)) return errMessage;

        if (dif == -1) dif = 2;

        if (dif == 3 && song.Count < 4) return RobotReply.NoBydChart;

        return ImageMessage.FromPath(await Path.ArcaeaChartPreview(song[dif]));
    }
}
