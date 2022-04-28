using Newtonsoft.Json;

namespace Andreal.Data.Json.Pjsk.PjskProfile;

public class UserAreaItemsItem
{
    [JsonProperty("areaItemId")] public int AreaItemID { get; set; }

    [JsonProperty("level")] public int Level { get; set; }
}
