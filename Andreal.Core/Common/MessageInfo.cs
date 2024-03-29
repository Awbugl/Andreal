﻿using System.Linq.Expressions;
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

    private static Dictionary<(Type, Func<ExecutorBase, object?>), string[]> _methodPrefixs;

    private static Dictionary<Type, Func<MessageInfo, ExecutorBase>> _ctors;

    static MessageInfo()
    {
        Init();
    }

    internal Bot Bot { get; set; }
    internal uint FromGroup { get; private set; }
    internal uint FromQQ { get; private set; }
    internal string[] CommandWithoutPrefix { get; private set; }
    internal MessageInfoType MessageType { get; private set; }
    private MessageChain? ReplyMessages { get; set; }
    internal MessageStruct Message { get; set; }

    internal Lazy<BotUserInfo?> UserInfo => new(() => BotUserInfo.Get(FromQQ));

    internal static RobotReply RobotReply => GlobalConfig.RobotReply;

    public static void Init(uint master) => _master = master;

    private static MessageBuilder FromMessageChain(MessageChain messages)
    {
        var body = new MessageBuilder();
        foreach (var msg in messages.ToArray())
        {
            switch (msg)
            {
                case null:
                    continue;

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
        }

        return body;
    }

    internal async Task<bool> PermissionCheck() => (await Bot.GetGroupMemberInfo(FromGroup, FromQQ)).Role > RoleType.Member;

    internal bool MasterCheck() => FromQQ == _master;

    private Task<int> SendPrivateMessage(MessageChain messages) => Bot.SendFriendMessage(FromQQ, FromMessageChain(messages));

    private Task<int> SendGroupMessage(MessageChain messages) => Bot.SendGroupMessage(FromGroup, FromMessageChain(messages));

    internal async Task<int> SendMessage(MessageChain? message)
    {
        if (message is null) return -1;
        return FromGroup != 0 && MessageType == MessageInfoType.Group
                   ? await SendGroupMessage(message.Prepend(new ReplyMessage(Message)))
                   : await SendPrivateMessage(message);
    }

    internal async Task<int> SendMessageOnly(MessageChain? message)
    {
        if (message is null) return -1;
        return FromGroup != 0 && MessageType == MessageInfoType.Group ? await SendGroupMessage(message) : await SendPrivateMessage(message);
    }

    public static void Process(
        Bot bot,
        int messageType,
        uint fromGroup,
        uint fromQq,
        MessageStruct message)
        => Task.Run(async () =>
        {
            var rMsg = Replace(message.Chain.ToString());

            foreach (KeyValuePair<(Type, Func<ExecutorBase, object?>), string[]> pair in _methodPrefixs)
            {
                string? match = null;

                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var j in pair.Value)
                {
                    if (rMsg.StartsWith(j, StringComparison.OrdinalIgnoreCase))
                    {
                        match = j;
                        break;
                    }
                }

                if (match == null) continue;

                var (executor, method) = pair.Key;

                var info = new MessageInfo
                           {
                               Bot = bot,
                               MessageType = (MessageInfoType)messageType,
                               CommandWithoutPrefix = rMsg[match.Length..].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries),
                               FromQQ = fromQq,
                               FromGroup = fromGroup,
                               Message = message
                           };

                try
                {
                    info.ReplyMessages = method(_ctors[executor](info)) switch
                                         {
                                             Task<MessageChain> task => await task,
                                             MessageChain chain      => chain,
                                             _                       => null
                                         };
                }
                catch (TargetInvocationException e)
                {
                    info.ReplyMessages = GetErrorMessage(e.InnerException!);
                    ExceptionLogger.Log(e.InnerException);
                }
                catch (AggregateException e)
                {
                    info.ReplyMessages = GetErrorMessage(e.InnerException!);
                    ExceptionLogger.Log(e.InnerException);
                }
                catch (Exception e)
                {
                    info.ReplyMessages = GetErrorMessage(e);
                    ExceptionLogger.Log(e);
                }
                finally
                {
                    if (info.ReplyMessages is not null)
                        try
                        {
                            var result = await info.SendMessage(info.ReplyMessages);

                            if (result != 0)
                            {
                                if (result == 120 && info.MessageType == MessageInfoType.Group)
                                    await bot.GroupLeave(fromGroup);
                                else
                                    await info.SendMessage(RobotReply.SendMessageFailed);
                            }

                            ++BotStatementHelper.ProcessCount;
                        }
                        catch (Exception e)
                        {
                            info.ReplyMessages = GetErrorMessage(e);
                            ExceptionLogger.Log(e);
                        }
                }

                return;
            }
        });

    private static TextMessage GetErrorMessage(Exception e)
        => e switch
           {
               JsonReaderException or HttpRequestException or TaskCanceledException or TimeoutException => RobotReply.OnAPIQueryFailed(e),
               ArgumentException when e.Message == RobotReply.NoSongFound                               => RobotReply.NoSongFound,
               _                                                                                        => RobotReply.OnExceptionOccured(e)
           };

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
        }

        if (rawMessage.StartsWith("/a ", StringComparison.OrdinalIgnoreCase)) rawMessage = "/arc " + rawMessage[3..];

        return string.Join(" ", rawMessage.Split(new char[] { '\n', '\t', '\r' }, StringSplitOptions.RemoveEmptyEntries));
    }

    private static void Init()
    {
        _methodPrefixs = new();
        _ctors = new();

        // ReSharper disable once LoopCanBePartlyConvertedToQuery
        foreach (var type in Assembly.GetExecutingAssembly().DefinedTypes)
        {
            if (type.BaseType == typeof(ExecutorBase))
            {
                if (!_ctors.ContainsKey(type)) _ctors.Add(type, GetExecutorCtor(type));

                foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    var prefixs = (method.GetCustomAttribute(typeof(CommandPrefixAttribute)) as CommandPrefixAttribute)?.Prefixs;
                    if (prefixs != null) _methodPrefixs.Add((type, GetMethodDelegate(method)), prefixs);
                }
            }
        }
    }

    private static Func<MessageInfo, ExecutorBase> GetExecutorCtor(Type type)
    {
        var ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, new[] { typeof(MessageInfo) });
        if (ctor == null) throw new ArgumentException("No valid constructor found.");
        var parameterExpression = Expression.Parameter(typeof(MessageInfo), "argument");
        var newExpression = Expression.New(ctor, parameterExpression);
        return Expression.Lambda<Func<MessageInfo, ExecutorBase>>(newExpression, parameterExpression).Compile();
    }

    private static Func<ExecutorBase, object?> GetMethodDelegate(MethodInfo method)
    {
        var parameterExpression = Expression.Parameter(typeof(ExecutorBase), "argument");
        var convertedExpression = Expression.Convert(parameterExpression, method.DeclaringType!);
        var methodCallExpression = Expression.Call(method.IsStatic ? null : convertedExpression, method);
        return Expression.Lambda<Func<ExecutorBase, object?>>(methodCallExpression, parameterExpression).Compile();
    }
}
