using Andreal.Message;
using Path = Andreal.Core.Path;

namespace Andreal.Model.Arcaea;

public class ArcaeaSong : List<ArcaeaChart>, IEquatable<ArcaeaSong>
{
    internal string SongID { get; set; } = null!;

    internal async Task<MessageChain> FullConstString()
    {
        var msg = new MessageChain();

        for (var i = 0; i <= Count; i++)
            if (i == 2 || this[i].JacketOverride)
                msg.Append(ImageMessage.FromPath(await Path.ArcaeaSong(SongID, 3)));

        for (var i = 0; i <= Count; i++) msg.Append("\n" + this[i].ConstString);

        return msg;
    }
    
    internal string NameWithPackage => $"{this[0].NameEn}\n(Package: {this[0].SetFriendly})";
    
    public bool Equals(ArcaeaSong? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return SongID.Equals(other.SongID);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((ArcaeaSong)obj);
    }

    public override int GetHashCode() => SongID.GetHashCode();
}
