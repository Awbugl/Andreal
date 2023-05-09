using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Andreal.Core.Common;
using Andreal.Core.Utils;
using Andreal.Window.UI;
using Konata.Core;
using Konata.Core.Common;
using Konata.Core.Components.Logics.Model;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces;
using Konata.Core.Interfaces.Api;
using Newtonsoft.Json;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;
using Path = Andreal.Core.Common.Path;

namespace Andreal.Window.Common;

internal static class Program
{
    //TODO: Update Version
    internal const string Version = "v0.5.6";

    internal static readonly ObservableCollection<ExceptionLog> Exceptions = new();
    internal static readonly ObservableCollection<MessageLog> Messages = new();
    internal static readonly ObservableCollection<AccountLog> Accounts = new();

    internal static AndrealConfig Config = JsonConvert.DeserializeObject<AndrealConfig>(File.ReadAllText(Path.Config))!;

    private static readonly ConcurrentDictionary<uint, string> BotFriendList = new();
    private static readonly ConcurrentDictionary<uint, ConfigJson> BotInfos = new();

    private static BotConfig _botConfig = BotConfig.Default();

    internal static void Add<T>(ObservableCollection<T> collection, T obj)
        => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, collection.Add, obj);

    private static void Remove<T>(ObservableCollection<T> collection, T obj)
        => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, collection.Remove, obj);

    internal static void RemoveFirst<T>(ObservableCollection<T> collection)
        => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, collection.RemoveAt, 0);

    internal static async Task OnPreLogin(uint uin, string password, bool retry = true)
    {
        var info = new AccountInfo { Account = uin, Password = password };

        var bot = GenerateBotInstance(info);

        Init(bot);

        var log = new AccountLog(bot, info.Account, "登录中", "", BotInfos[info.Account].Protocol);
        Add(Accounts, log);

        var loginresult = await bot.Login();

        if (loginresult)
        {
            Config.Accounts.Add(info);
            await File.WriteAllTextAsync(Path.Config, JsonConvert.SerializeObject(Config));
        }
        else if (retry)
        {
            switch (loginresult.EventType)
            {
                case WtLoginEvent.Type.Unknown:
                case WtLoginEvent.Type.LoginDenied:
                case WtLoginEvent.Type.HighRiskOfEnvironment:
                {
                    Remove(Accounts, log);
                    await OnPreLogin(uin, password, false);
                    return;
                }
            }
        }

        await OnLogin(bot, loginresult);
    }

    internal static async Task OnLogin(Bot bot, WtLoginResult loginresult)
    {
        var log = Accounts.First(i => i.Bot == bot);

        if (loginresult)
        {
            UpdateKeystore(bot.Uin, bot.KeyStore);
            log.Nick = bot.Name;

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var friend in await bot.GetFriendList(true))
            {
                if (!BotFriendList.ContainsKey(friend.Uin)) BotFriendList.TryAdd(friend.Uin, friend.Name);
            }
        }
        else
        {
            log.State = "登录失败";
            log.Message = loginresult.EventMessage;
            MessageBox.Show($"QQ {log.Robot} \n登录失败，请重新登录！\n原因：{loginresult.EventMessage}  错误代码：{loginresult.ResultCode}", "登录失败！");

            switch (loginresult.EventType)
            {
                case WtLoginEvent.Type.InvalidUinOrPassword:
                    await OnRemove(log);
                    break;
            }
        }
    }

    public static async Task OnRemove(AccountLog log)
    {
        if (log.Bot?.IsOnline() == true) await log.Bot.Logout();

        var info = Config.Accounts.Find(i => i.Account == log.Robot);
        Config.Accounts.Remove(info!);
        await File.WriteAllTextAsync(Path.Config, JsonConvert.SerializeObject(Config));

        log.Bot?.Dispose();
        Remove(Accounts, log);

        if (MessageBox.Show("是否删除此账号的模拟登录信息？\n删除后不可恢复！", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes) RemoveBotInfo(log.Robot);
    }

    private static void RemoveBotInfo(uint bot)
    {
        var pth = Path.BotConfig(bot);
        if (File.Exists(pth)) File.Delete(pth);
        BotInfos.TryRemove(bot, out _);
    }

    private static void Init(Bot bot)
    {
        bot.OnGroupMessage += OnGroupMessage;
        bot.OnFriendMessage += OnFriendMessage;
        bot.OnGroupInvite += OnGroupInvite;
        bot.OnFriendRequest += OnFriendRequest;
        bot.OnCaptcha += OnCaptcha;
        bot.OnBotOnline += OnBotOnline;
        bot.OnBotOffline += OnBotOffline;
        bot.OnGroupMute += OnBotGroupMute;
        bot.OnGroupMessageBlocked += OnGroupMessageBlocked;
    }

    internal static async Task ProgramInit()
    {
        _botConfig = new()
                     {
                         TryReconnect = true,
                         CustomHost = null,
                         HighwayChunkSize = 4096,
                         EnableAudio = false,
                         DefaultTimeout = 6000,
                         Protocol = Config.Protocol
                     };

        External.Initialize(Config);

        foreach (var info in Config.Accounts)
        {
            var bot = GenerateBotInstance(info);
            Init(bot);
            Add(Accounts, new(bot, info.Account, "登录中", "", BotInfos[info.Account].Protocol));
            var loginresult = await bot.Login();
            await OnLogin(bot, loginresult);
        }
    }

    private static void OnBotOnline(Bot b, BotOnlineEvent e)
    {
        var log = Accounts.First(i => i.Bot == b);
        log.State = "在线";
        log.Message = $"登录时间: {e.EventTime}";
    }

    private static void OnBotOffline(Bot b, BotOfflineEvent e)
    {
        var log = Accounts.First(i => i.Bot == b);

        switch (e.Type)
        {
            case BotOfflineEvent.OfflineType.NetworkDown:
                log.State = "网络连接失败，重新连接中...";
                log.Message = $"掉线时间: {e.EventTime}";
                break;

            case BotOfflineEvent.OfflineType.ServerKickOff:
                log.State = "强制下线";
                log.Message = $"{e.EventTime} : {e.EventMessage}";
                break;


            case BotOfflineEvent.OfflineType.UserLoggedOut:
                log.State = "手动离线";
                break;
        }
    }

    private static void OnGroupMessageBlocked(Bot b, GroupMessageBlockedEvent e)
    {
        var log = Accounts.First(i => i.Bot == b);

        if (log.State == "在线")
        {
            log.State = "在线（发送群消息受限）";
            log.Message += "请手动登录Bot账号并访问 https://accounts.qq.com/safe/message/unlock?lock_info=5_5 解除限制。";
        }
    }

    private static void OnGroupMessage(Bot b, GroupMessageEvent e)
    {
        ++BotStatementHelper.GroupMessageCount;

        Add(Messages,
            new()
            {
                FromGroup = $"{e.GroupName} ({e.GroupUin})",
                FromQQ = $"{e.MemberCard} ({e.MemberUin})",
                Time = e.EventTime,
                Message = e.Chain.ToString(),
                Robot = $"{b.Name} ({b.Uin})"
            });

        if (Messages.Count > 100) RemoveFirst(Messages);

        if (Config.EnableHandleMessage)
            if (e.MemberUin != b.Uin)
                External.Process(b, 1, e.GroupUin, e.MemberUin, e.Message);
    }

    private static void OnFriendMessage(Bot b, FriendMessageEvent e)
    {
        ++BotStatementHelper.PrivateMessageCount;
        Add(Messages,
            new()
            {
                FromQQ = $"{(BotFriendList.ContainsKey(e.FriendUin) ? BotFriendList[e.FriendUin] : "")} ({e.FriendUin})",
                FromGroup = "-1 (私聊)",
                Time = e.EventTime,
                Message = e.Chain.ToString(),
                Robot = $"{b.Name} ({b.Uin})"
            });

        if (Messages.Count > 100) RemoveFirst(Messages);

        if (Config.EnableHandleMessage) External.Process(b, 0, 0, e.FriendUin, e.Message);
    }

    private static async void OnFriendRequest(Bot b, FriendRequestEvent e)
    {
        if (Config is { EnableHandleMessage: true, Settings.FriendAdd: true }) await b.ApproveFriendRequest(e.ReqUin, e.Token);
    }

    private static async void OnGroupInvite(Bot b, GroupInviteEvent e)
    {
        if (Config.EnableHandleMessage)
            if (Config.Settings.GroupAdd || e.InviterUin == Config.Master || Config.Settings.GroupInviterWhitelist.Contains(e.InviterUin))
                await b.ApproveGroupInvitation(e.GroupUin, e.InviterUin, e.Token);
    }

    private static async void OnBotGroupMute(Bot b, GroupMuteMemberEvent e)
    {
        if (e.MemberUin == b.Uin) await b.GroupLeave(e.GroupUin);
    }

    private static void OnCaptcha(Bot b, CaptchaEvent e)
    {
        switch (e.Type)
        {
            case CaptchaEvent.CaptchaType.Sms:
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var window = new SmsCodeVerify(b, e.Phone);
                    window.ShowDialog();
                });
                break;

            case CaptchaEvent.CaptchaType.Slider:
                Application.Current.Dispatcher.Invoke(() =>
                {
                    System.Windows.Window window = Config.SliderType == SliderType.Helper
                                                       ? new SliderSubmit(b, e.SliderUrl)
                                                       : new SliderVerify(b, e.SliderUrl);
                    window.ShowDialog();
                });
                break;

            default:
            case CaptchaEvent.CaptchaType.Unknown:
                break;
        }
    }

    private static void UpdateKeystore(uint qqid, BotKeyStore keystore)
    {
        var pth = Path.BotConfig(qqid);
        var cfg = new ConfigJson { KeyStore = keystore, Protocol = BotInfos[qqid].Protocol, Device = BotInfos[qqid].Device };
        File.WriteAllText(pth, JsonConvert.SerializeObject(cfg));
    }

    private static Bot GenerateBotInstance(AccountInfo info)
    {
        var pth = Path.BotConfig(info.Account);

        var cfg = File.Exists(pth)
                      ? JsonConvert.DeserializeObject<ConfigJson>(File.ReadAllText(pth))!
                      : new() { Protocol = Config.Protocol, Device = BotDevice.Default(), KeyStore = new(info.Account.ToString(), info.Password) };

        BotInfos[info.Account] = cfg;
        return BotFather.Create(_botConfig, cfg.Device, cfg.KeyStore);
    }
}
