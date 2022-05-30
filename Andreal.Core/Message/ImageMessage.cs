using Andreal.Core.UI;
using Andreal.Core.Utils;
using Path = Andreal.Core.Common.Path;

namespace Andreal.Core.Message;

[Serializable]
public class ImageMessage : IMessage
{
    private readonly string _path;

    private ImageMessage(string path) { _path = path; }

    public override string ToString() => _path;

    internal static ImageMessage FromPath(Path path) => new(path);

    internal static ImageMessage FromUrl(string url, Path? pth = null)
    {
        using var img = WebHelper.GetImage(url);
        pth ??= Path.RandImageFileName();
        img.SaveAsJpgWithQuality(pth);
        return FromPath(pth);
    }

    public static implicit operator ImageMessage(Image value)
    {
        var pth = Path.RandImageFileName();
        value.SaveAsJpgWithQuality(pth);
        value.Dispose();
        return FromPath(pth);
    }

    public static implicit operator ImageMessage(BackGround value)
    {
        var pth = Path.RandImageFileName();
        value.SaveAsJpgWithQuality(pth);
        value.Dispose();
        return FromPath(pth);
    }
}
