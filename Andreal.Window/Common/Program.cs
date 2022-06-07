using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Andreal.Core.Common;
using Andreal.Core.Utils;
using Andreal.Window.UI;
using Konata.Core;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using Newtonsoft.Json;
using Path = Andreal.Core.Common.Path;


namespace Andreal.Window.Common;

internal static class Program
{
    internal static readonly ObservableCollection<ExceptionLog> Exceptions = new();
    internal static readonly ObservableCollection<MessageLog> Messages = new();
    internal static readonly ObservableCollection<AccountLog> Accounts = new();

    internal static AndrealConfig Config = JsonConvert.DeserializeObject<AndrealConfig>(File.ReadAllText(Path.Config))!;

    private static readonly ConcurrentDictionary<uint, string> BotFriendList = new();

    private static BotConfig _botConfig = BotConfig.Default();

    internal static void Add<T>(ObservableCollection<T> collection, T obj)
    {
        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, collection.Add, obj);
    }

    private static void Remove<T>(ObservableCollection<T> collection, T obj)
    {
        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, collection.Remove, obj);
    }

    internal static void RemoveAt<T>(ObservableCollection<T> collection)
    {
        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, collection.RemoveAt, 0);
    }


    internal static async Task<(bool Success, WtLoginEvent.Type Type)> OnLogin(string uin, string password)
    {
        var info = new AccountInfo() { Account = uint.Parse(uin), Password = password };

        var bot = info.GenerateBotInstance(_botConfig);

        Init(bot);

        Add(Accounts, new(bot, info.Account, "登录中", ""));

        var loginresult = await bot.Login();

        if (loginresult.Success)
        {
            Config.Accounts.Add(info);
            await File.WriteAllTextAsync(Path.Config, JsonConvert.SerializeObject(Config));
        }

        await OnLogin(bot, loginresult);

        return loginresult;
    }

    internal static async Task OnLogin(Bot bot, (bool Success, WtLoginEvent.Type Type) loginresult)
    {
        var log = Accounts.First(i => i.Bot == bot);
        if (loginresult.Success)
        {
            UpdateKeystore(bot.Uin, bot.KeyStore);
            log.Nick = bot.Name;

            foreach (var friend in await bot.GetFriendList(true))
                if (!BotFriendList.ContainsKey(friend.Uin))
                    BotFriendList.TryAdd(friend.Uin, friend.Name);
        }
        else
        {
            log.State = "登陆失败";
            log.Message = Translate(loginresult.Type);
        }
    }

    public static async Task OnRemove(AccountLog log)
    {
        if (log.Bot.IsOnline()) await log.Bot.Logout();
        log.Bot!.Dispose();
        var info = Config.Accounts.Find(i => i.Account == log.Robot);
        Config.Accounts.Remove(info!);
        await File.WriteAllTextAsync(Path.Config, JsonConvert.SerializeObject(Config));
        Remove(Accounts, log);
    }

    private static string Translate(WtLoginEvent.Type type)
    {
        return type switch
               {
                   WtLoginEvent.Type.Unknown              => "未知原因",
                   WtLoginEvent.Type.CheckSms             => "需要短信验证",
                   WtLoginEvent.Type.CheckSlider          => "需要滑块验证",
                   WtLoginEvent.Type.VerifyDeviceLock     => "需要设备锁验证",
                   WtLoginEvent.Type.LoginDenied          => "登录被其他设备拒绝",
                   WtLoginEvent.Type.InvalidUinOrPassword => "QQ号或密码不正确",
                   WtLoginEvent.Type.HighRiskEnvironment  => "高风险环境，被TX禁止登录",
                   WtLoginEvent.Type.InvalidSmsCode       => "短信验证码不正确",
                   WtLoginEvent.Type.TokenExpired         => "快速登录Token已过期",
                   _                                      => ""
               };
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
            var bot = info.GenerateBotInstance(_botConfig);
            Init(bot);
            Add(Accounts, new(bot, info.Account, "登录中", ""));
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
                log.State = "网络掉线，重新连接中...";
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

        if (Messages.Count > 100) RemoveAt(Messages);

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
                FromQQ
                    = $"{(BotFriendList.ContainsKey(e.FriendUin) ? BotFriendList[e.FriendUin] : "")} ({e.FriendUin})",
                FromGroup = "-1 (私聊)",
                Time = e.EventTime,
                Message = e.Chain.ToString(),
                Robot = $"{b.Name} ({b.Uin})"
            });

        if (Messages.Count > 100) RemoveAt(Messages);

        if (Config.EnableHandleMessage) External.Process(b, 0, 0, e.FriendUin, e.Message);
    }

    private static async void OnFriendRequest(Bot b, FriendRequestEvent e)
    {
        if (Config.EnableHandleMessage)
            if (Config.Settings.FriendAdd)
                await b.ApproveFriendRequest(e.ReqUin, e.Token);
    }

    private static async void OnGroupInvite(Bot b, GroupInviteEvent e)
    {
        if (Config.EnableHandleMessage)
            if (Config.Settings.GroupAdd || e.InviterUin == Config.Master
                                         || Config.Settings.GroupInviterWhitelist.Contains(e.InviterUin))
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
                                                          window.Show();
                                                      });
                break;

            case CaptchaEvent.CaptchaType.Slider:
                Application.Current.Dispatcher.Invoke(() =>
                                                      {
                                                          var window = new SliderVerify(b, e.SliderUrl);
                                                          window.Show();
                                                      });
                break;

            default:
            case CaptchaEvent.CaptchaType.Unknown:
                break;
        }
    }

    /// <summary>
    ///     Update keystore
    /// </summary>
    /// <returns></returns>
    private static void UpdateKeystore(uint qqid, BotKeyStore keystore)
    {
        var pth = Path.BotConfig(qqid);
        if (!File.Exists(pth)) return;
        var cfg = JsonConvert.DeserializeObject<ConfigJson>(File.ReadAllText(pth))!;
        cfg = new() { Device = cfg.Device, KeyStore = keystore };
        File.WriteAllText(pth, JsonConvert.SerializeObject(cfg));
    }
}
