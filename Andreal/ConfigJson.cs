using Konata.Core;
using Konata.Core.Common;
using Konata.Core.Interfaces;
using Newtonsoft.Json;
using Path = Andreal.Core.Path;

namespace Andreal;

public class ConfigJson
{
    [JsonProperty("keystore")] public BotKeyStore? KeyStore { get; set; }
    [JsonProperty("device")] public BotDevice? Device { get; set; }
}

public class AccountInfo
{
    [JsonProperty("uid")] public uint Account { get; set; }
    [JsonProperty("password")] public string Password { get; set; } = "";

    internal Bot GenerateBotInstance()
    {
        var pth = Path.BotConfig(Account);
        if (File.Exists(pth))
        {
            var cfg = JsonConvert.DeserializeObject<ConfigJson>(File.ReadAllText(pth))!;
            return BotFather.Create(BotConfig.Default(), cfg.Device, cfg.KeyStore);
        }
        else
        {
            var bot = BotFather.Create(Account.ToString(), Password, out _, out var device, out var keystore);
            var cfg = new ConfigJson { Device = device, KeyStore = keystore };
            File.WriteAllText(pth, JsonConvert.SerializeObject(cfg));
            return bot;
        }
    }
}

public class ApiConfig
{
    [JsonProperty("url")] public string Url { get; set; } = "";
    [JsonProperty("token")] public string Token { get; set; } = "";
}

public class AndrealSettings
{
    [JsonProperty("friend_add")] public bool FriendAdd { get; set; }
    [JsonProperty("group_add")] public bool GroupAdd { get; set; }

    [JsonProperty("group_inviter_whitelist")]
    public List<uint> GroupInviterWhitelist { get; set; } = new();
}

public class AndrealConfig
{
    [JsonProperty("master")] public uint Master { get; set; }
    [JsonProperty("accounts")] public List<AccountInfo> Accounts { get; set; } = new();
    [JsonProperty("api")] public Dictionary<string, ApiConfig> Api { get; set; } = new();
    [JsonProperty("approve_settings")] public AndrealSettings Settings { get; set; } = new();

    [JsonProperty("enable_handle_message")]
    public bool EnableHandleMessage { get; set; } = true;
}
