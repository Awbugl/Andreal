using Andreal.Core.Data.Json.Arcaea.ArcaeaLimited;

namespace Andreal.Core.Model.Arcaea;

[Serializable]
internal class LimitedBest30Data : IBest30Data
{
    private double _b30Avg, _r10Avg;

    private List<RecordInfo> _best30List;

    private short _potential;

    internal LimitedBest30Data(Best30 b30data, short potential)
    {
        B30data = b30data;
        _potential = potential;
        _best30List = B30data.Data.Select(i => new RecordInfo(i)).ToList();
        _b30Avg = B30data.Data.Average(i => i.PotentialValue);
        _r10Avg = _potential > 0
            ? (double)_potential / 25 - 3 * _b30Avg
            : -1;
    }

    private Best30 B30data { get; }

    private string Best30Avg => _b30Avg.ToString("0.0000");

    private string Recent10Avg =>
        _r10Avg > 0
            ? _r10Avg.ToString("0.0000")
            : "--";

    string IBest30Data.Best30Avg => Best30Avg;

    string IBest30Data.Recent10Avg => Recent10Avg;

    List<RecordInfo> IBest30Data.Best30List => _best30List;
}
