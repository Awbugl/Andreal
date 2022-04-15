using Andreal.Data.Json.Arcaea.BotArcApi;

namespace Andreal.Model.Arcaea;

internal class Best30Data : IBest30Data
{
    internal Best30Data(UserBestsContent b30data) { B30data = b30data; }

    private UserBestsContent B30data { get; }

    private string Best30Avg => B30data.Best30Avg.ToString("0.0000");

    private string Recent10Avg =>
        B30data.Recent10Avg > 0
            ? B30data.Recent10Avg.ToString("0.0000")
            : "--";

    private List<RecordInfo> Best30List => B30data.Best30List.Select(i => new RecordInfo(i)).ToList();

    string IBest30Data.Best30Avg => Best30Avg;

    string IBest30Data.Recent10Avg => Recent10Avg;

    List<RecordInfo> IBest30Data.Best30List => Best30List;
}
