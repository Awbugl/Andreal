using System.Text;
using Andreal.Core;
using Konata.Core;
using Konata.Core.Common;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Interfaces.Api;
using Net.Codecrete.QrCodeGenerator;
using Newtonsoft.Json;
using Path = Andreal.Core.Path;

namespace Andreal;

internal static class Program
{
    private static readonly AndrealConfig Config
        = JsonConvert.DeserializeObject<AndrealConfig>(File.ReadAllText(Path.Config))!;

    private static IEnumerable<Bot> _bots = null!;

    public static async Task Main()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
                                                      {
                                                          Reporter.ExceptionReport(args.ExceptionObject as Exception);
                                                          Console.ForegroundColor = ConsoleColor.Red;
                                                          Console.WriteLine("A fatal error occurred!");
                                                          Console.WriteLine(args.ExceptionObject as Exception);
                                                          Console.WriteLine("Press any key to exit...");
                                                          Console.ReadKey();
                                                          Environment.Exit(0);
                                                      };

        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine(@" _  __              _        ");
        Console.WriteLine(@"| |/ /___ _ _  __ _| |_ __ _ ");
        Console.WriteLine(@"| ' </ _ | ' \/ _` |  _/ _` |");
        Console.WriteLine(@"|_|\_\___|_||_\__,_\__|\__,_|");

        Console.WriteLine("Konata. Konata Project 2022\nProject Andreal v0.2.8 on Konata\n\n");

        External.Initialize(Config);

        _bots = Config.Accounts.Select(i => i.GenerateBotInstance());

        foreach (var bot in _bots)
        {
            if (Config.EnableHandleMessage)
            {
                bot.OnGroupMessage += OnGroupMessage;
                bot.OnFriendMessage += OnFriendMessage;
                bot.OnGroupInvite += OnGroupInvite;
                bot.OnFriendRequest += OnFriendRequest;
            }

            bot.OnCaptcha += OnCaptcha;
            bot.OnLog += OnLog;
            bot.OnBotOffline += OnBotOffline;

            // Login the bot ans update the keystore
            if (await bot.Login()) UpdateKeystore(bot.Uin, bot.KeyStore);
        }

        await Task.Delay(-1);
    }

    private static void OnBotOffline(Bot b, BotOfflineEvent e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"BotOfflineType: {e.Type.ToString()}\n{e.EventTime:yyyy/MM/dd:HH:mm:ss}\n{e.EventMessage}");
        Console.ResetColor();
    }

    private static void OnGroupMessage(Bot b, GroupMessageEvent e)
    {
        if (e.MemberUin != b.Uin) External.Process(b, 1, e.GroupUin, e.MemberUin, e.Message);
    }

    private static void OnFriendMessage(Bot b, FriendMessageEvent e)
    {
        External.Process(b, 0, 0, e.FriendUin, e.Message);
    }

    private static void OnFriendRequest(Bot b, FriendRequestEvent e)
    {
        if (Config.Settings.FriendAdd) b.ApproveFriendRequest(e.ReqUin, e.Token);
    }

    private static void OnGroupInvite(Bot b, GroupInviteEvent e)
    {
        if (Config.Settings.GroupAdd || e.InviterUin == Config.Master
                                     || Config.Settings.GroupInviterWhitelist.Contains(e.InviterUin))
            b.ApproveGroupInvitation(e.GroupUin, e.InviterUin, e.Token);
    }

    private static void OnCaptcha(Bot b, CaptchaEvent e)
    {
        switch (e.Type)
        {
            case CaptchaEvent.CaptchaType.Sms:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("需要短信验证码：");
                Console.WriteLine($"We have sent a sms to your phone {e.Phone}.\nPlease enter the sms code:");
                Console.WriteLine();
                Console.ResetColor();
                b.SubmitSmsCode(Console.ReadLine());
                break;

            case CaptchaEvent.CaptchaType.Slider:

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("需要滑块验证，请使用验证助手扫码验证：");
                Console.WriteLine("若显示错误，请将控制台的字体更换为新宋体。");
                Console.WriteLine(e.SliderUrl);
                Console.WriteLine();
                Console.ResetColor();

                var qrCode = QrCode.EncodeText(e.SliderUrl, QrCode.Ecc.Low);

                // Print the qrcode to console
                for (var y = 0; y < qrCode.Size + 2; y += 2)
                {
                    for (var x = 0; x < qrCode.Size + 2; ++x)
                    {
                        var bgColor = qrCode.GetModule(x - 1, y - 1)
                            ? ConsoleColor.Black
                            : ConsoleColor.White;

                        var fgColor = qrCode.GetModule(x - 1, y) || y > qrCode.Size
                            ? ConsoleColor.Black
                            : ConsoleColor.White;

                        Console.ForegroundColor = fgColor;
                        Console.BackgroundColor = bgColor;
                        Console.Write("▄");
                        Console.ResetColor();
                    }

                    Console.WriteLine();
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please enter the ticket:");
                Console.ResetColor();

                b.SubmitSliderTicket(Console.ReadLine());
                break;

            default:
            case CaptchaEvent.CaptchaType.Unknown:
                break;
        }
    }

    private static void OnLog(Bot b, LogEvent e)
    {
        if (e.Tag == "PacketComponent") return;
        Console.WriteLine(e.EventMessage);
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
