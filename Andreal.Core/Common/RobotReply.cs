using Andreal.Core.Message;
using Andreal.Core.Utils;
using Newtonsoft.Json;

namespace Andreal.Core.Common;

public class RobotReply
{
    [JsonProperty("NotBind")]
    public string NotBind { get; set; } = "";

    [JsonProperty("NotBindArc")]
    public string NotBindArc { get; set; } = "";

    [JsonProperty("ParameterLengthError")]
    public string ParameterLengthError { get; set; } = "";

    [JsonProperty("ParameterError")]
    public string ParameterError { get; set; } = "";

    [JsonProperty("ConfigNotFound")]
    public string ConfigNotFound { get; set; } = "";

    [JsonProperty("NotPlayedTheSong")]
    public string NotPlayedTheSong { get; set; } = "";

    [JsonProperty("ArcUidNotFound")]
    public string ArcUidNotFound { get; set; } = "";

    [JsonProperty("TooManyArcUid")]
    public string TooManyArcUid { get; set; } = "";

    [JsonProperty("NoSongFound")]
    public string NoSongFound { get; set; } = "";

    [JsonProperty("NoBydChart")]
    public string NoBydChart { get; set; } = "";

    [JsonProperty("NotPlayedTheChart")]
    public string NotPlayedTheChart { get; set; } = "";

    [JsonProperty("UnBindSuccess")]
    public string UnBindSuccess { get; set; } = "";

    [JsonProperty("GotShadowBanned")]
    public string GotShadowBanned { get; set; } = "";

    [JsonProperty("APIQueryFailed")]
    public string APIQueryFailed { get; set; } = "";

    [JsonProperty("TooManySongFound")]
    public string TooManySongFound { get; set; } = "";

    [JsonProperty("JrrpResult")]
    public string JrrpResult { get; set; } = "";

    [JsonProperty("SendMessageFailed")]
    public string SendMessageFailed { get; set; } = "";

    [JsonProperty("BindSuccess")]
    public string BindSuccess { get; set; } = "";

    [JsonProperty("HelpMessage")]
    public string HelpMessage { get; set; } = "";

    [JsonProperty("GroupLeave")]
    public string GroupLeave { get; set; } = "";

    [JsonProperty("BelowTheThreshold")]
    public string BelowTheThreshold { get; set; } = "";

    [JsonProperty("NeedUpdateAUA")]
    public string NeedUpdateAUA { get; set; } = "";

    [JsonProperty("InvalidSessionInfo")]
    public string InvalidSessionInfo { get; set; } = "";

    [JsonProperty("SessionExpired")]
    public string SessionExpired { get; set; } = "";

    [JsonProperty("SessionQuerying")]
    public string SessionQuerying { get; set; } = "";

    [JsonProperty("SessionWaitingForAccount")]
    public string SessionWaitingForAccount { get; set; } = "";

    [JsonProperty("DuplicateBestsRequests")]
    public string DuplicateBestsRequests { get; set; } = "";

    [JsonProperty("UserBestsSession")]
    public string UserBestsSession { get; set; } = "";

    [JsonProperty("ExceptionOccured")]
    public string ExceptionOccured { get; set; } = "";

    internal TextMessage OnExceptionOccured(Exception ex) => ExceptionOccured.Replace("$exception$", ex.Message);
    internal TextMessage OnAPIQueryFailed(Exception ex) => APIQueryFailed.Replace("$exception$", ex.Message);

    internal TextMessage OnAPIQueryFailed(int status, string message) => APIQueryFailed.Replace("$exception$", $"{status}: {message}");

    internal TextMessage OnJrrpResult(string value) => JrrpResult.Replace("$jrrp$", value);
    internal TextMessage OnBindSuccess(string value) => BindSuccess.Replace("$info$", value);

    internal TextMessage OnUserBestsSession(string value) => UserBestsSession.Replace("$session$", value);

    internal TextMessage OnSessionQuerying(string value) => SessionQuerying.Replace("$count$", value);

    internal TextMessage OnSessionWaitingForAccount(string value) => SessionWaitingForAccount.Replace("$count$", value);

    internal string GetRandomAIReply(string songname, string artist)
        => GlobalConfig.RandomReply.GetRandomItem().Replace("$songname$", songname).Replace("$artist$", artist);
}
