namespace Andreal.Model.Arcaea;

internal interface IBest30Data
{
    internal string Best30Avg { get; }
    internal string Recent10Avg { get; }
    internal List<RecordInfo> Best30List { get; }
}
