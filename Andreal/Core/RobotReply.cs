using Andreal.Message;
using Newtonsoft.Json;

namespace Andreal.Core;

public class RobotReply
{
    internal readonly Func<Exception, TextMessage> ExceptionOccured = ex => ex switch
                                                                            {
                                                                                AggregateException exception =>
                                                                                    $"发生了未预料的错误，请稍后重试。\n({exception.InnerException!.Message})",
                                                                                _ => $"发生了未预料的错误，请稍后重试。\n({ex.Message})"
                                                                            };

    [JsonProperty("NotBind")] public string NotBind { get; set; } = "";
    [JsonProperty("NotBindArc")] public string NotBindArc { get; set; } = "";
    [JsonProperty("NotBindPjsk")] public string NotBindPjsk { get; set; } = "";
    [JsonProperty("ParameterLengthError")] public string ParameterLengthError { get; set; } = "";
    [JsonProperty("ParameterError")] public string ParameterError { get; set; } = "";
    [JsonProperty("ConfigNotFound")] public string ConfigNotFound { get; set; } = "";
    [JsonProperty("NotPlayedTheSong")] public string NotPlayedTheSong { get; set; } = "";
    [JsonProperty("Best30Querying")] public string Best30Querying { get; set; } = "";
    [JsonProperty("PjskUserBindFailed")] public string PjskUserBindFailed { get; set; } = "";
    [JsonProperty("ArcUidNotFound")] public string ArcUidNotFound { get; set; } = "";
    [JsonProperty("TooManyArcUid")] public string TooManyArcUid { get; set; } = "";
    [JsonProperty("NoSongFound")] public string? NoSongFound { get; set; } = "";
    [JsonProperty("NoBydChart")] public string NoBydChart { get; set; } = "";
    [JsonProperty("NotPlayedTheChart")] public string NotPlayedTheChart { get; set; } = "";
    [JsonProperty("UnBindSuccess")] public string UnBindSuccess { get; set; } = "";
    [JsonProperty("GotShadowBanned")] public string GotShadowBanned { get; set; } = "";
    [JsonProperty("APIQueryFailed")] public string APIQueryFailed { get; set; } = "";
    [JsonProperty("TooManySongFound")] public string TooManySongFound { get; set; } = "";
    [JsonProperty("JrrpResult")] public string JrrpResult { get; set; } = "";
    [JsonProperty("SendMessageFailed")] public string SendMessageFailed { get; set; } = "";
    [JsonProperty("BindSuccess")] public string BindSuccess { get; set; } = "";
    [JsonProperty("HelpMessage")] public string HelpMessage { get; set; } = "";
    [JsonProperty("RandSongReply")] public string RandSongReply { get; set; } = "";
    [JsonProperty("GroupLeave")] public string GroupLeave { get; set; } = "";
}
