using Konata.Core;
using Konata.Core.Common;
using Konata.Core.Interfaces;
using Newtonsoft.Json;

namespace Andreal.Core.Common;

public class ConfigJson
{
    [JsonProperty("keystore")] public BotKeyStore? KeyStore { get; set; }
    [JsonProperty("device")] public BotDevice? Device { get; set; }
}

public class AccountInfo
{
    [JsonProperty("uid")] public uint Account { get; set; }
    [JsonProperty("password")] public string Password { get; set; } = "";

    public Bot GenerateBotInstance(BotConfig config)
    {
        var pth = Path.BotConfig(Account);
        if (File.Exists(pth))
        {
            var cfg = JsonConvert.DeserializeObject<ConfigJson>(File.ReadAllText(pth))!;
            return BotFather.Create(config, cfg.Device, cfg.KeyStore);
        }
        else
        {
            var keystore = new BotKeyStore(Account.ToString(), Password);
            var device = BotDevice.Default();
            var bot = BotFather.Create(config, device, keystore);
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
    [JsonProperty("protocol")] public OicqProtocol Protocol { get; set; }

    [JsonProperty("enable_handle_message")]
    public bool EnableHandleMessage { get; set; } = true;

    [JsonProperty("accounts")] public List<AccountInfo> Accounts { get; set; } = new();
    [JsonProperty("api")] public Dictionary<string, ApiConfig> Api { get; set; } = new();
    [JsonProperty("approve_settings")] public AndrealSettings Settings { get; set; } = new();
}
