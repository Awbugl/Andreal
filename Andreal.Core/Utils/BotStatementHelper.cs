using Andreal.Core.Message;

namespace Andreal.Core.Utils;

public static class BotStatementHelper
{
    private static readonly DateTime Time = DateTime.Now;

    internal static ulong ProcessCount = 0, ExceptionCount = 0, WebExceptionCount = 0;

    public static ulong GroupMessageCount = 0, PrivateMessageCount = 0;

    internal static TextMessage Statement =>
        $"状态简要:\n已运行: {DateTime.Now - Time:d\\.hh\\:mm\\:ss} day(s)\n群聊消息: {GroupMessageCount}\n私聊消息: {PrivateMessageCount}\n发送消息: {ProcessCount}\n记录异常: {ExceptionCount} + [Web] {WebExceptionCount}\n最近异常: \n{LastExceptionHelper.Get()}";

    public static string Status =>
        $"运行时间: {DateTime.Now - Time:d\\.hh\\:mm\\:ss} day(s)  群聊消息: {GroupMessageCount}  私聊消息: {PrivateMessageCount}  发送消息: {ProcessCount} 记录异常: {ExceptionCount} + [Web] {WebExceptionCount}";
}
