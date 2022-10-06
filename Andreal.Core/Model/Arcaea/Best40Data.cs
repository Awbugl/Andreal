using Andreal.Core.Data.Json.Arcaea.ArcaeaUnlimited;

namespace Andreal.Core.Model.Arcaea;

internal class Best40Data : IBest40Data
{
    internal Best40Data(UserBestsContent b40data)
    {
        B40data = b40data;
    }

    private UserBestsContent B40data { get; }

    private string Best30Avg => B40data.Best30Avg.ToString("0.0000");

    private string Recent10Avg => B40data.Recent10Avg > 0 ? B40data.Recent10Avg.ToString("0.0000") : "--";

    private List<RecordInfo> Best30List => B40data.Best30List.Select(i => new RecordInfo(i)).ToList();

    private List<RecordInfo>? OverflowList => B40data.OverflowList?.Select(i => new RecordInfo(i)).ToList();

    string IBest40Data.Best30Avg => Best30Avg;

    string IBest40Data.Recent10Avg => Recent10Avg;

    List<RecordInfo> IBest40Data.Best30List => Best30List;

    List<RecordInfo>? IBest40Data.OverflowList => OverflowList;
}
