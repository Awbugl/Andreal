using System.Reflection;
using Andreal.Core.Data.Sqlite;
using Andreal.Core.Executor;
using Andreal.Core.Message;
using Andreal.Core.Utils;
using Konata.Core;
using Konata.Core.Common;
using Konata.Core.Interfaces.Api;
using Konata.Core.Message;
using Konata.Core.Message.Model;
using Newtonsoft.Json;
using MessageChain = Andreal.Core.Message.MessageChain;

#pragma warning disable CS8618

namespace Andreal.Core.Common;

[Serializable]
internal class MessageInfo
{
    private static uint _master;

    private static readonly Dictionary<(Type, MethodInfo), string[]> MethodPrefixs = GetMethodPrefixs();

    private static readonly Dictionary<string, string> AbbreviationPairs = new()
                                                                           {
                                                                               { "/a ", "/arc " },
                                                                               { "/o ", "/osu " },
                                                                               { "/p ", "/pjsk " },
                                                                               { "/ar ", "/arc room " }
                                                                           };

    internal Bot Bot { get; set; }
    internal uint FromGroup { get; private set; }
    internal uint FromQQ { get; private set; }
    internal string[] CommandWithoutPrefix { get; private set; }
    internal MessageInfoType MessageType { get; private set; }
    private MessageChain? ReplyMessages { get; set; }
    internal MessageStruct Message { get; set; }

    internal Lazy<BotUserInfo?> UserInfo => new(() => BotUserInfo.Get(FromQQ));

    internal static RobotReply RobotReply => GlobalConfig.RobotReply;

    public static void Init(uint master) { _master = master; }

    private static MessageBuilder FromMessageChain(MessageChain messages)
    {
        var body = new MessageBuilder();
        foreach (var msg in messages.ToArray())
            switch (msg)
            {
                case null: continue;

                case ReplyMessage reply:
                    body.Add(ReplyChain.Create(reply.Message));
                    break;

                case ImageMessage img:
                    body.Add(ImageChain.CreateFromFile(img.ToString()));
                    break;

                default:
                    if (!string.IsNullOrWhiteSpace(msg.ToString())) body.Text(msg.ToString());
                    break;
            }

        return body;
    }

    internal async Task<bool> PermissionCheck() =>
        (await Bot.GetGroupMemberInfo(FromGroup, FromQQ)).Role > RoleType.Member;

    internal bool MasterCheck() => FromQQ == _master;

    private object SendPrivateMessage(MessageChain messages) =>
        Bot.SendFriendMessage(FromQQ, FromMessageChain(messages));

    private object SendGroupMessage(MessageChain messages) =>
        Bot.SendGroupMessage(FromGroup, FromMessageChain(messages));

    internal void SendMessage(MessageChain? message)
    {
        if (message is null) return;
        _ = FromGroup != 0 && MessageType == MessageInfoType.Group
            ? SendGroupMessage(message.Prepend(new ReplyMessage(Message)))
            : SendPrivateMessage(message);
    }

    internal void SendMessageOnly(MessageChain? message)
    {
        if (message is null) return;
        _ = FromGroup != 0 && MessageType == MessageInfoType.Group
            ? SendGroupMessage(message)
            : SendPrivateMessage(message);
    }

    public static void Process(Bot bot, int messageType, uint fromGroup, uint fromQq, MessageStruct message)
    {
        Task.Run(() =>
                 {
                     var rMsg = Replace(message.Chain.ToString());

                     foreach (var pair in MethodPrefixs)
                     {
                         var match
                             = pair.Value.FirstOrDefault(j => rMsg.StartsWith(j, StringComparison.OrdinalIgnoreCase));

                         if (match == default) continue;

                         var (executor, method) = pair.Key;
                         var info = new MessageInfo
                                    {
                                        Bot = bot,
                                        MessageType = (MessageInfoType)messageType,
                                        CommandWithoutPrefix
                                            = rMsg[match.Length..]
                                                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                                        FromQQ = fromQq,
                                        FromGroup = fromGroup,
                                        Message = message
                                    };

                         try
                         {
                             var result = method.Invoke(Activator.CreateInstance(executor, info), null);
                             info.ReplyMessages = result as MessageChain ?? (result as Task<MessageChain>)?.Result;
                         }
                         catch (TargetInvocationException e)
                         {
                             info.ReplyMessages = e.InnerException switch
                                                  {
                                                      JsonReaderException or HttpRequestException
                                                          or TaskCanceledException
                                                          or TimeoutException =>
                                                          RobotReply.OnAPIQueryFailed(e.InnerException),
                                                      _ => RobotReply.OnExceptionOccured(e.InnerException!)
                                                  };
                             Reporter.ExceptionReport(e.InnerException);
                         }
                         catch (AggregateException e)
                         {
                             info.ReplyMessages = e.InnerException switch
                                                  {
                                                      JsonReaderException or HttpRequestException
                                                          or TaskCanceledException
                                                          or TimeoutException =>
                                                          RobotReply.OnAPIQueryFailed(e.InnerException),
                                                      _ => RobotReply.OnExceptionOccured(e.InnerException!)
                                                  };
                             Reporter.ExceptionReport(e.InnerException);
                         }
                         catch (Exception e)
                         {
                             info.ReplyMessages = RobotReply.OnExceptionOccured(e);
                             Reporter.ExceptionReport(e);
                         }
                         finally
                         {
                             try
                             {
                                 info.SendMessage(info.ReplyMessages);
                                 ++BotStatementHelper.ProcessCount;
                             }
                             catch (Exception e)
                             {
                                 Reporter.ExceptionReport(e);
                                 try
                                 {
                                     info.SendMessage(RobotReply.SendMessageFailed);
                                 }
                                 catch (Exception ex)
                                 {
                                     Reporter.ExceptionReport(ex);
                                 }
                             }
                         }

                         return;
                     }
                 });
    }

    private static string Replace(string rawMessage)
    {
        rawMessage = rawMessage.Trim();
        switch (rawMessage)
        {
            case "查分":
            case "查最近":
            case "查":
            case "/a":
            case "/arc":
                return "/arc info";
            case "/o":
            case "/osu":
                return "/osu mode";
        }

        foreach (var (key, value) in
                 AbbreviationPairs.Where(i => rawMessage.StartsWith(i.Key, StringComparison.OrdinalIgnoreCase)))
            rawMessage = value + rawMessage[key.Length..];

        return string.Join(" ",
                           rawMessage.Split(new char[] { '\n', '\t', '\r' }, StringSplitOptions.RemoveEmptyEntries));
    }

    private static Dictionary<(Type, MethodInfo), string[]> GetMethodPrefixs()
    {
        var ls = new Dictionary<(Type, MethodInfo), string[]>();

        foreach (var type in Assembly.GetExecutingAssembly().DefinedTypes
                                     .Where(i => i.BaseType == typeof(ExecutorBase)))
            foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic
                                                   | BindingFlags.Public))
            {
                var prefixs = (method.GetCustomAttribute(typeof(CommandPrefixAttribute)) as CommandPrefixAttribute)
                    ?.Prefixs;
                if (prefixs != null) ls.Add((type, method), prefixs);
            }

        return ls;
    }
}
