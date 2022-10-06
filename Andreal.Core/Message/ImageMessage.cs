using Andreal.Core.UI;
using Path = Andreal.Core.Common.Path;

namespace Andreal.Core.Message;

[Serializable]
public class ImageMessage : IMessage
{
    private readonly string _path;

    private ImageMessage(string path)
    {
        _path = path;
    }

    public override string ToString() => _path;

    internal static ImageMessage FromPath(Path path) => new(path);

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
