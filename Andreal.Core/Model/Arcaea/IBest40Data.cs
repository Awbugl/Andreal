namespace Andreal.Core.Model.Arcaea;

internal interface IBest40Data
{
    internal string Best30Avg { get; }
    internal string Recent10Avg { get; }
    internal List<RecordInfo> Best30List { get; }
    internal List<RecordInfo>? OverflowList { get; }
}
